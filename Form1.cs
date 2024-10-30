using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace p_1_Delphi_to_C_sharp
{
    public partial class Form1 : Form
    {
        private const int ADC_CH_MAX = 8;
        private const double LOW_REF_mA = 4.0;
        private const double HIGH_REF_mA = 19.5;

        private SerialPort ComPort1 = new SerialPort();
        private SerialPort ComPort2 = new SerialPort();
        private Timer Timer_Com1Rx = new Timer();
        private Timer Timer_Com1Tx = new Timer();
        private Timer Timer_Com2Rx = new Timer();
        private Timer Timer_Com2Tx = new Timer();
        private Timer Timer_RunProcess = new Timer();

        private int FCom1_RxDelay, FCom1_TxDelay, FCom2_RxDelay, FCom2_TxDelay;
        private int FTimerSec, FRunStep;
        private double FPresHighLevel;
        private string FCom1_RxStr = "", FCom2_RxStr = "";
        private string FCom1_TxCmd = "", FCom2_TxCmd = "";
        private double[] FBuff_mA = new double[ADC_CH_MAX];
        private double[] FBuff_mH2O = new double[ADC_CH_MAX];
        private double[] FMeasLow = new double[5];
        private double[] FMeasHigh = new double[5];

        private const string STR_NON_VALUE = "---"; // 표시할 기본값으로 설정
        private enum RunMode
        {
            rmNon,
            rmTestOut,
            rmSetOut,
            rmTestPre,
            rmSetPre
        }
        // FRunMode 변수 선언 및 초기화
        private RunMode FRunMode = RunMode.rmNon;  // 초기값을 rmNon으로 설정

        private IniFile FSetup;

        private DebugForm fDebug; // 디버그 창을 참조하는 변수

        // 전역 변수를 선언하여 LED 이미지 준비 (Form1의 생성자에서 초기화해도 좋음)
        private Image ledOnImage;   // 켜진 LED 이미지
        private Image ledOffImage;  // 꺼진 LED 이미지

        public Form1()
        {
            InitializeComponent();
            InitializePorts();
            InitializeTimers();
            fDebug = new DebugForm(); // DebugForm 인스턴스 생성
            // 설정 파일 초기화
            string configFilePath = Path.Combine(Application.StartupPath, "Setup.ini");
            FSetup = new IniFile(configFilePath);
            ledOnImage = Properties.Resources.LedOn;   // 켜진 LED 이미지
            ledOffImage = Properties.Resources.LedOff; // 꺼진 LED 이미지
        }

        private void InitializePorts()
        {
            ComPort1.DataReceived += ComPort1_DataReceived;
            ComPort2.DataReceived += ComPort2_DataReceived;
        }
        private void ComPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FCom1_RxStr += ComPort1.ReadExisting();
        }

        private void ComPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FCom2_RxStr += ComPort2.ReadExisting();
        }
        private void InitializeTimers()
        {
            Timer_Com1Rx.Interval = FCom1_RxDelay;
            Timer_Com1Rx.Tick += Timer_Com1Rx_Tick;
            Timer_Com1Tx.Interval = FCom1_TxDelay;
            Timer_Com1Tx.Tick += Timer_Com1Tx_Tick;

            Timer_Com2Rx.Interval = FCom2_RxDelay;
            Timer_Com2Rx.Tick += Timer_Com2Rx_Tick;
            Timer_Com2Tx.Interval = FCom2_TxDelay;
            Timer_Com2Tx.Tick += Timer_Com2Tx_Tick;

            Timer_RunProcess.Tick += Timer_RunProcess_Tick;
        }

        private void SetAnalogZero() => SendCommandWithParam(Edit_AOffset, "#O1");
        private void SetAnalogGain() => SendCommandWithParam(Edit_AGain, "#O2");
        private void SetAnalogAll() => SendCommandWithParams(Edit_AOffset, Edit_AGain, "#O3");
        private void SetPressureGain() => SendCommandWithParam(Edit_PGain, "#P2");
        private void SetPressureAll() => SendCommandWithParams(Edit_POffset, Edit_PGain, "#P3");

        private void Timer_Com2Tx_Tick(object sender, EventArgs e)
        {
            Timer_Com2Tx.Enabled = false;
            string txStr;

            // `RunMode`가 압력 검증 또는 설정 모드일 때
            if (FRunMode == RunMode.rmTestPre || FRunMode == RunMode.rmSetPre)
            {
                Timer_Com2Rx.Tag = (int)Timer_Com2Rx.Tag + 1;
                if ((int)Timer_Com2Rx.Tag > 2)
                {
                    Timer_Com2Rx.Tag = 0;
                    Label_MeasValue.Text = STR_NON_VALUE;
                }
            }

            if (Timer_Com2Rx.Enabled)
            {
                Timer_Com2Tx.Interval = 10;
                Timer_Com2Tx.Enabled = true;
            }
            else
            {
                // 명령 전송을 위한 문자열을 결정
                txStr = string.IsNullOrEmpty(FCom2_TxCmd) ? "#G" : FCom2_TxCmd;
                FCom2_TxCmd = string.Empty;

                // 명령 전송
                Com2_TxStr(txStr);

                // 타이머 설정
                Timer_Com2Rx.Interval = FCom2_RxDelay;
                Timer_Com2Rx.Enabled = true;
                Timer_Com2Tx.Interval = FCom2_TxDelay;
                Timer_Com2Tx.Enabled = CheckBox_AutoLoop.Checked;
            }
        }

        private void Com2_TxStr(string command)
        {
            if (ComPort2.IsOpen)
            {
                ComPort2.Write(command);  // 명령을 ComPort2로 전송
                FCom2_TxCmd = command;
                ToggleLED(LED); // 전송 LED 토글 (이전 코드에 맞춰 사용 중인 PictureBox 컨트롤이 LED 표시로 설정되어 있다고 가정)
            }
        }

        private void Com2_SendCmd(string command)
        {
            // 명령을 ComPort2로 전송합니다.
            Com2_TxStr(command);
        }

        private void SendCommandWithParam(TextBox param, string command)
        {
            if (float.TryParse(param.Text, out float value))
            {
                string txStr = command + GetFloatToStr(value);
                Com2_SendCmd(txStr);
            }
        }

        private void SendCommandWithParams(TextBox param1, TextBox param2, string command)
        {
            if (float.TryParse(param1.Text, out float value1) && float.TryParse(param2.Text, out float value2))
            {
                string txStr = command + GetFloatToStr(value1) + GetFloatToStr(value2);
                Com2_SendCmd(txStr);
            }
        }

        private void Button_AReset_Click(object sender, EventArgs e) => Com2_SendCmd("#O0");
        private void Button_PReset_Click(object sender, EventArgs e) => Com2_SendCmd("#P0");
        private void Button_PZero_Click(object sender, EventArgs e) => Com2_SendCmd("#P1");
        private void Button_SetOut20mA_Click(object sender, EventArgs e) => Com2_SendCmd("#T1");
        private void Button_SetOut4mA_Click(object sender, EventArgs e) => Com2_SendCmd("#T0");

        private void Button_SaveConfig_Click(object sender, EventArgs e)
        {
            FSetup.WriteString("ComPort", "Port1", ComPort1.PortName);
            FSetup.WriteString("ComPort", "RxDelay1", Edit_RxDelay1.Text);
            FSetup.WriteString("ComPort", "TxDelay1", Edit_TxDelay1.Text);
            FSetup.WriteString("ComPort", "Port2", ComboBox_ComPort2.Text);
            FSetup.WriteString("ComPort", "RxDelay2", Edit_RxDelay2.Text);
            FSetup.WriteString("ComPort", "TxDelay2", Edit_TxDelay2.Text);
        }

        private void ToggleAutoLoop(bool isEnabled)
        {
            Button_Start.Enabled = !isEnabled;
            Button_Stop.Enabled = isEnabled;
            CheckBox_AutoLoop.Checked = isEnabled;
        }

        private void Button_Debug_Click(object sender, EventArgs e) => fDebug.Show();

        // 예시: Form1에서 디버그 메시지 출력(debugform.cs참조)
        private void SomeMethod()
        {
            fDebug.WriteLine("디버그 정보: 작업이 성공적으로 완료되었습니다.");
        }

        // PictureBox의 LED 상태를 토글하는 메서드
        private void ToggleLED(PictureBox led)
        {
            // 현재 이미지에 따라 토글
            if (led.Image == ledOnImage)
            {
                led.Image = ledOffImage; // 끄기
            }
            else
            {
                led.Image = ledOnImage;  // 켜기
            }
        }

        private string GetFloatToStr(float value)
        {
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff);
            return Encoding.Default.GetString(buff);
        }

        private void DispMeasMode()
        {
            double measVal = FBuff_mA[FChIdx];
            Label_MeasValue.Text = measVal <= 3.0 ? STR_NON_VALUE : measVal.ToString("0.000");
        }

        private void DispTestOutMode()
        {
            double value = FBuff_mA[FChIdx];
            double errVal, refVal, refErr;
            Color color;

            if (value <= 3.0)
            {
                SetDisplayValues(STR_NON_VALUE, STR_NON_VALUE, SystemColors.Control);
            }
            else
            {
                refVal = double.Parse(Label_RefValue.Text);
                errVal = refVal - value;
                refErr = double.Parse(Label_RefError.Text);

                color = Math.Abs(errVal) > refErr ? Color.Red : Color.Lime;
                SetDisplayValues(value.ToString("0.000"), errVal.ToString("0.000"), color);
            }
        }

        private void SetDisplayValues(string measValue, string measError, Color backgroundColor)
        {
            Label_MeasValue.Text = measValue;
            Label_MeasError.Text = measError;

            Label_MeasValue.Parent.BackColor = backgroundColor;
            Label_RefValue.Parent.BackColor = backgroundColor;
            Label_MeasError.Parent.BackColor = backgroundColor;
            Label_RefError.Parent.BackColor = backgroundColor;
        }

        private void Timer_Com1Rx_Tick(object sender, EventArgs e)
        {
            Timer_Com1Rx.Enabled = false;

            if (string.IsNullOrEmpty(FCom1_RxStr)) return;

            ToggleLED(LED_Rx1);
            string rxStr = FCom1_RxStr;
            FCom1_RxStr = string.Empty;

            if (CheckBox_RxHexView.Checked)
            {
                fDebug.WriteMsgHex("Rx Hex: ", rxStr);
            }

            if (CheckCRC_MODBUS(rxStr))
            {
                HandleModbusResponse(rxStr);
            }
            else
            {
                fDebug.WriteMsgStr("Error --", "CRC [" + rxStr.Length + "] " + FDebCount);
            }
            FDebCount = 0;
        }

        private void HandleModbusResponse(string rxStr)
        {
            switch ((byte)rxStr[2])
            {
                case 0x03:
                    DispMultiRead_MODBUS(rxStr);
                    break;
                case 0x06:
                    DispSingleWrite_MODBUS(rxStr);
                    break;
                case 0x10:
                    DispMultiWrite_MODBUS(rxStr);
                    break;
            }
        }

        private void Timer_Com2Rx_Tick(object sender, EventArgs e)
        {
            Timer_Com2Rx.Enabled = false;

            if (string.IsNullOrEmpty(FCom2_RxStr)) return;

            ToggleLED(LED_Rx2);
            string rxStr = FCom2_RxStr;
            FCom2_RxStr = string.Empty;

            if (rxStr[0] == '=')
            {
                switch (rxStr[1])
                {
                    case 'G':
                        double value = double.Parse(rxStr.Substring(2).Trim());
                        DispTestPressMode(value);
                        Timer_Com2Rx.Tag = 0;
                        break;
                    default:
                        fDebug.WriteStr("COM2 RX: " + rxStr.Trim());
                        break;
                }
            }
        }

        private void DispTestPressMode(double value)
        {
            Label_MeasValue.Text = value.ToString("0.0");
        }

        private void Timer_Com1Tx_Tick(object sender, EventArgs e)
        {
            Timer_Com1Tx.Enabled = false;

            if (Timer_Com1Rx.Enabled)
            {
                Timer_Com1Tx.Interval = 10;
                Timer_Com1Tx.Enabled = true;
            }
            else
            {
                string txStr = string.IsNullOrEmpty(FCom1_TxCmd) ? GenerateModbusReadCommand() : FCom1_TxCmd;
                Com1_TxStr(txStr);
                FCom1_TxCmd = string.Empty;

                Timer_Com1Rx.Interval = FCom1_RxDelay;
                Timer_Com1Rx.Enabled = true;
                Timer_Com1Tx.Interval = FCom1_TxDelay;
                Timer_Com1Tx.Enabled = CheckBox_AutoLoop.Checked;
            }
        }

        private string GenerateModbusReadCommand()
        {
            ushort readMem = REG_mA;
            string txStr = $"{(char)0x01}{(char)0x03}{(char)(readMem >> 8)}{(char)readMem}{(char)0x00}{(char)0x08}";

            ushort crc16 = Calculate_CRC_MODBUS(txStr);
            return txStr + $"{(char)crc16}{(char)(crc16 >> 8)}";
        }

        private void Timer_RunProcess_Tick(object sender, EventArgs e)
        {
            FTimerSec += 1;
            ExecuteRunMode();
            StatusBar.Panels[5].Text = "Step: " + FRunStep;
            StatusBar.Panels[6].Text = "Time: " + FTimerSec;
        }


        private void ExecuteRunMode()
        {
            switch (FRunMode)
            {
                case RunMode.rmNon:
                    break;
                case RunMode.rmTestOut:
                    RunTestOut();
                    break;
                case RunMode.rmSetOut:
                    RunSetupOut();
                    break;
                case RunMode.rmTestPre:
                    RunTestPres();
                    break;
                case RunMode.rmSetPre:
                    RunSetupPres();
                    break;
            }
        }
        private void RunTestOut()
        {
            string strVal = Label_MeasValue.Text;
            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                switch (FRunStep)
                {
                    case 0:
                        if (FTimerSec >= 2)
                        {
                            if ((int)Panel_Ref.Tag != 0)
                            {
                                SetRef_mA(0);
                            }

                            FRunStep++;
                            FTimerSec = 0;
                            Com2_TxStr("#T0");  // Output 4mA
                        }
                        break;

                    case 1:
                        if (Panel_MeasValue.BackColor == Color.Red)
                        {
                            FTimerSec = 0;
                        }
                        else
                        {
                            if (FTimerSec >= 5)
                            {
                                FRunStep++;
                                FTimerSec = 0;
                                Com2_TxStr("#T1");  // Output 19.5mA
                                SetRef_mA(1);       // Set reference value
                            }
                        }
                        break;

                    case 2:
                        // Future steps can be added here
                        break;
                }
            }
            else
            {
                if ((int)Panel_Ref.Tag != 0)
                {
                    SetRef_mA(0);
                }
                FRunStep = 0;
                FTimerSec = 0;
            }
        }
        private void RunSetupOut()
        {
            string strVal = Label_MeasValue.Text;
            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                switch (FRunStep)
                {
                    case 0:
                        if (FTimerSec >= 2)
                        {
                            Com2_TxStr("#O0");  // Initialize calibration for 4-20mA
                            FRunStep++;
                            FTimerSec = 0;

                            FMeasCount = 0;
                            for (int i = 0; i < CALI_BUFF_MAX; i++)
                            {
                                FMeasLow[i] = 0;
                                FMeasHigh[i] = 0;
                            }
                        }
                        break;

                    case 1:
                        if (FTimerSec >= 2)
                        {
                            if (Panel_Ref.Tag.ToString() != "0")
                            {
                                SetRef_mA(0);  // Set low reference value
                            }
                            FRunStep++;
                            FTimerSec = 0;
                            Com2_TxStr("#T0");  // Output 4mA
                        }
                        break;

                    case 2:
                        // Save measurement data
                        for (int i = 1; i < FMeasLow.Length; i++)
                        {
                            FMeasLow[i - 1] = FMeasLow[i];
                        }
                        FMeasLow[FMeasLow.Length - 1] = double.Parse(strVal);

                        // Check if buffer is filled
                        if (FMeasCount < FMeasLow.Length)
                        {
                            FMeasCount++;
                        }
                        else
                        {
                            // Analyze 4mA data
                            double refError = double.Parse(Label_RefError.Text);
                            for (int i = 0; i < FMeasLow.Length; i++)
                            {
                                for (int j = 0; j < FMeasLow.Length; j++)
                                {
                                    double value = Math.Abs(FMeasLow[i] - FMeasLow[j]);
                                    if (value > refError) return;
                                }
                            }

                            // Proceed to the next step
                            FMeasCount = 0;
                            FTimerSec = 0;
                            FRunStep++;
                            Com2_TxStr("#T1");  // Output 19.5mA
                            SetRef_mA(1);  // Set high reference value
                        }
                        break;

                    case 3:
                        // Save 20mA measurement data
                        for (int i = 1; i < FMeasHigh.Length; i++)
                        {
                            FMeasHigh[i - 1] = FMeasHigh[i];
                        }
                        FMeasHigh[FMeasHigh.Length - 1] = double.Parse(strVal);

                        // Check if buffer is filled
                        if (FMeasCount < FMeasHigh.Length)
                        {
                            FMeasCount++;
                        }
                        else
                        {
                            // Analyze 20mA data
                            refError = double.Parse(Label_RefError.Text);
                            for (int i = 0; i < FMeasHigh.Length; i++)
                            {
                                for (int j = 0; j < FMeasHigh.Length; j++)
                                {
                                    double value = Math.Abs(FMeasHigh[i] - FMeasHigh[j]);
                                    if (value > refError) return;
                                }
                            }

                            // Calculate calibration offset and gain
                            double measLow = FMeasLow[CALI_BUFF_MAX - 1];
                            double measHigh = FMeasHigh[CALI_BUFF_MAX - 1];
                            float gain = (float)((HIGH_REF_mA - LOW_REF_mA) / (measHigh - measLow));
                            float offset = (float)(LOW_REF_mA - (measLow * gain));

                            string txStr = "#O3" + GetFloatToStr(offset) + GetFloatToStr(gain);
                            Com2_TxStr(txStr);  // Send calibration values
                            FRunStep++;
                        }
                        break;

                    case 4:
                        // Final calibration step, if needed
                        break;
                }
            }
            else
            {
                if (Panel_Ref.Tag.ToString() != "0")
                {
                    SetRef_mA(0);
                }
                FRunStep = 0;
                FTimerSec = 0;
            }
        }
        private void RunTestPres()
        {
            string strVal = Label_MeasValue.Text;

            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                switch (FRunStep)
                {
                    case 0:
                        Label_Message.Text = "차압 검증 중...";
                        break;
                }
            }
            else
            {
                FRunStep = 0;
                FTimerSec = 0;
            }
        }
        private void RunSetupPres()
        {
            string strVal = Label_MeasValue.Text;

            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                switch (FRunStep)
                {
                    case 0:
                        if (FTimerSec >= 2)
                        {
                            Label_Message.Text = "차압 교정 시작..!";
                            Com2_SendCmd("#P0");  // 교정값 초기화
                            FRunStep++;
                            FTimerSec = 0;
                        }
                        break;

                    case 1:
                        if (FTimerSec >= 2)
                        {
                            Label_Message.Text = "영점 조정 중...!";
                            Com2_SendCmd("#P1");  // 영점 조정 명령
                            FRunStep++;
                            FTimerSec = 0;
                        }
                        break;

                    case 2:
                        if (FTimerSec >= 2)
                        {
                            Label_Message.Text = "교정값 설정 중...!";
                            Button_PSetValue.Enabled = true;
                            FRunStep++;
                            FTimerSec = 0;
                        }
                        break;

                    case 3:
                        // 수동 교정 대기 상태
                        break;

                    case 4:
                        Label_Message.Text = "교정 완료...";
                        Panel_Message.BackColor = Color.Lime;
                        FRunStep++;
                        FTimerSec = 0;
                        break;

                    case 5:
                        // 교정 완료 상태
                        break;
                }
            }
            else
            {
                Label_Message.Text = "압력 교정 모드";
                Panel_Message.BackColor = Color.Yellow;
                Button_PSetValue.Enabled = false;
                FRunStep = 0;
                FTimerSec = 0;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }

    
}







public class IniFile
{
    private string filePath;

    public IniFile(string path)
    {
        filePath = path;
    }

    public void WriteString(string section, string key, string value)
    {
        // 섹션과 키에 따라 설정 값을 파일에 저장하는 코드 작성
        // 예시: WinAPI WritePrivateProfileString 사용 가능
    }

    public string ReadString(string section, string key, string defaultValue)
    {
        // 설정 파일에서 값을 읽어 반환하는 코드 작성
        return defaultValue; // 예시를 위한 기본값 반환
    }
}



