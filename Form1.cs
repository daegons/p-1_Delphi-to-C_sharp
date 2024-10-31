using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        private int FChIdx = 1; // 초기값 설정, io보드 채널번호?

        private int FDebCount = 0; // 디버그 메시지 카운터 초기화

        
        private const ushort REG_mA = 0x0200; //REG_mA = $0200;

        private int FMeasCount = 0;

        private const int CALI_BUFF_MAX = 5; // 버퍼 크기에 맞게 설정

        public Form1()
        {
            InitializeComponent();
            InitializePorts();
            InitializeTimers();
            LoadAvailablePorts(); // 콤보박스에 사용 가능한 포트를 로드
            fDebug = new DebugForm(); // DebugForm 인스턴스 생성
            // 설정 파일 초기화
            string configFilePath = Path.Combine(Application.StartupPath, "Setup.ini");
            FSetup = new IniFile(configFilePath);
            ledOnImage = Properties.Resources.LedOn;   // 켜진 LED 이미지
            ledOffImage = Properties.Resources.LedOff; // 꺼진 LED 이미지

            LoadPortSettings(); // 포트 설정을 불러오기
            Button_Start.Enabled = false; // Start 버튼 비활성화
        }

        private void LoadPortSettings()
        {
            // 설정 파일에서 포트 이름을 불러와 콤보박스에 설정
            string port1 = FSetup.ReadString("ComPort", "Port1", "");
            string port2 = FSetup.ReadString("ComPort", "Port2", "");

            if (!string.IsNullOrEmpty(port1) && ComboBox_ComPort1.Items.Contains(port1))
            {
                ComboBox_ComPort1.SelectedItem = port1;
                ComPort1.PortName = port1;
            }

            if (!string.IsNullOrEmpty(port2) && ComboBox_ComPort2.Items.Contains(port2))
            {
                ComboBox_ComPort2.SelectedItem = port2;
                ComPort2.PortName = port2;
            }
        }
        private void EnableStartButton()
        {
            Button_Start.Enabled = true; // Start 버튼 활성화
        }
        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            ComboBox_ComPort1.Items.AddRange(ports);
            ComboBox_ComPort2.Items.AddRange(ports);
        }
        private void ComboBox_ComPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 포트가 닫혀 있는 경우에만 설정 가능
            if (!ComPort1.IsOpen)
            {
                ComPort1.PortName = ComboBox_ComPort1.SelectedItem.ToString();
                EnableStartButton();
            }
        }

        private void ComboBox_ComPort2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 포트가 닫혀 있는 경우에만 설정 가능
            if (!ComPort2.IsOpen)
            {
                ComPort2.PortName = ComboBox_ComPort2.SelectedItem.ToString();
                EnableStartButton();
            }
        }
        private void InitializePorts()
        {
            ComPort1.DataReceived += ComPort1_DataReceived;
            ComPort2.DataReceived += ComPort2_DataReceived;
        }
        private void ComPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FCom1_RxStr += ComPort1.ReadExisting();
            // 포트1 데이터 수신 처리
            string data = ComPort1.ReadExisting();
            Invoke(new MethodInvoker(() => MessageBox.Show("포트1 수신 데이터: " + data)));
        }

        private void ComPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = ComPort2.ReadExisting();
            if (!string.IsNullOrEmpty(data))
            {
                FCom2_RxStr += data;
                Console.WriteLine($"[ComPort2 DataReceived] 수신 데이터: {data}");
            }
            else
            {
                Console.WriteLine("[ComPort2 DataReceived] 데이터가 없습니다.");
            }

            // RunTestPres를 통해 실시간 UI 업데이트를 시도
            Invoke(new Action(() => RunTestPres()));
        }
        private void InitializeTimers()
        {
            //Timer_Com1Rx.Interval = FCom1_RxDelay;
            //Timer_Com1Rx.Tick += Timer_Com1Rx_Tick;
            //Timer_Com1Tx.Interval = FCom1_TxDelay;
            //Timer_Com1Tx.Tick += Timer_Com1Tx_Tick;
            //Timer_Com2Rx.Interval = FCom2_RxDelay;
            //Timer_Com2Rx.Tick += Timer_Com2Rx_Tick;
            //Timer_Com2Tx.Interval = FCom2_TxDelay;
            //Timer_Com2Tx.Tick += Timer_Com2Tx_Tick;
            //Timer_RunProcess.Tick += Timer_RunProcess_Tick;


            // FCom1_RxDelay 및 FCom1_TxDelay가 0일 경우 최소 1로 설정
            Timer_Com1Rx.Interval = FCom1_RxDelay > 0 ? FCom1_RxDelay : 1;
            Timer_Com1Rx.Tick += Timer_Com1Rx_Tick;

            Timer_Com1Tx.Interval = FCom1_TxDelay > 0 ? FCom1_TxDelay : 1;
            Timer_Com1Tx.Tick += Timer_Com1Tx_Tick;

            // FCom2_RxDelay 및 FCom2_TxDelay가 0일 경우 최소 1로 설정
            Timer_Com2Rx.Interval = FCom2_RxDelay > 0 ? FCom2_RxDelay : 1;
            Timer_Com2Rx.Tick += Timer_Com2Rx_Tick;

            Timer_Com2Tx.Interval = FCom2_TxDelay > 0 ? FCom2_TxDelay : 1;
            Timer_Com2Tx.Tick += Timer_Com2Tx_Tick;

            // Timer_RunProcess의 Interval을 원하는 기본값(예: 1000ms)으로 설정
            Timer_RunProcess.Interval = 1000; // 예시로 1초로 설정
            Timer_RunProcess.Tick += Timer_RunProcess_Tick;
            //////////////////////////////////////////////////////
            Timer_Com2Rx.Interval = 100; // 100ms마다 확인
            Timer_Com2Rx.Tick += (s, e) =>
            {
                if (ComPort2.IsOpen && ComPort2.BytesToRead > 0)
                {
                    string data = ComPort2.ReadExisting();
                    if (!string.IsNullOrEmpty(data))
                    {
                        FCom2_RxStr += data;
                        Console.WriteLine($"[Timer Com2Rx] 데이터 수신: {data}");
                        RunTestPres();
                    }
                }
            };
            Timer_Com2Rx.Start();
            ////////////////////////////////////////////////////////
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
                    Panel_MeasValue.Text = STR_NON_VALUE;
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
            // 선택된 포트를 설정 파일에 저장
            if (ComboBox_ComPort1.SelectedItem != null)
            {
                FSetup.WriteString("ComPort", "Port1", ComboBox_ComPort1.SelectedItem.ToString());
            }

            if (ComboBox_ComPort2.SelectedItem != null)
            {
                FSetup.WriteString("ComPort", "Port2", ComboBox_ComPort2.SelectedItem.ToString());
            }

            MessageBox.Show("포트 설정이 저장되었습니다.");
        }

        private void ToggleAutoLoop(bool isEnabled)
        {
            Button_Start.Enabled = !isEnabled;
            Button_Stop.Enabled = isEnabled;
            //CheckBox_AutoLoop.Checked = isEnabled;
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
            Panel_MeasValue.Text = measVal <= 3.0 ? STR_NON_VALUE : measVal.ToString("0.000");
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
            Panel_MeasValue.Text = measValue;
            Label_MeasError.Text = measError;

            Panel_MeasValue.Parent.BackColor = backgroundColor;
            Label_RefValue.Parent.BackColor = backgroundColor;
            Label_MeasError.Parent.BackColor = backgroundColor;
            Label_RefError.Parent.BackColor = backgroundColor;
        }

        // CRC 검사를 위한 메서드
        private bool CheckCRC_MODBUS(string data)
        {
            // Modbus CRC 계산 로직
            ushort computedCRC = Calculate_CRC_MODBUS(data.Substring(0, data.Length - 2));
            ushort receivedCRC = BitConverter.ToUInt16(Encoding.ASCII.GetBytes(data.Substring(data.Length - 2)), 0);

            return computedCRC == receivedCRC;
        }

        // Modbus CRC 계산 메서드
        private ushort Calculate_CRC_MODBUS(string data)
        {
            ushort crc = 0xFFFF;

            foreach (byte b in Encoding.ASCII.GetBytes(data))
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    bool lsbSet = (crc & 0x0001) != 0;
                    crc >>= 1;
                    if (lsbSet) crc ^= 0xA001;
                }
            }

            return crc;
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
        // 다중 읽기 명령 응답 처리
        private void DispMultiRead_MODBUS(string rxStr)
        {
            // 수신된 다중 읽기 데이터를 디버그 로그에 출력
            fDebug.WriteLine("Received Multi-Read MODBUS data: " + rxStr);

            // 필요한 추가 데이터 파싱 및 표시 작업을 여기에 작성
        }

        // 단일 쓰기 명령 응답 처리
        private void DispSingleWrite_MODBUS(string rxStr)
        {
            // 수신된 단일 쓰기 데이터를 디버그 로그에 출력
            fDebug.WriteLine("Received Single-Write MODBUS data: " + rxStr);

            // 필요한 추가 데이터 파싱 및 표시 작업을 여기에 작성
        }

        // 다중 쓰기 명령 응답 처리
        private void DispMultiWrite_MODBUS(string rxStr)
        {
            // 수신된 다중 쓰기 데이터를 디버그 로그에 출력
            fDebug.WriteLine("Received Multi-Write MODBUS data: " + rxStr);

            // 필요한 추가 데이터 파싱 및 표시 작업을 여기에 작성
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
            Panel_MeasValue.Text = value.ToString("0.0");
        }

        private void Com1_TxStr(string command)
        {
            if (ComPort1.IsOpen)
            {
                ComPort1.Write(command);  // 명령을 ComPort1로 전송
                ToggleLED(LED_Tx1);       // 전송 LED를 토글
            }
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

            // 차압 검증 모드가 활성화된 경우 RunTestPres 호출
            if (FRunMode == RunMode.rmTestPre)
            {
                RunTestPres(); // 차압 검증 모드에서 실시간 차압 표시
            }
            else
            {
                ExecuteRunMode(); // 다른 모드 실행
            }

            // StatusStrip의 ToolStripStatusLabel에 Step과 Time을 표시
            toolStripStatusLabelStep.Text = "Step: " + FRunStep;
            toolStripStatusLabelTime.Text = "Time: " + FTimerSec;
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
                default:
                    MessageBox.Show("작업을 선택하세요.");
                    break;
            }
        }

        // 기준 전류값을 설정하는 메서드
        private void SetRef_mA(int refLevel)
        {
            double referenceValue;

            // refLevel이 0일 경우 4mA, 1일 경우 19.5mA를 설정
            if (refLevel == 0)
                referenceValue = 4.0;
            else if (refLevel == 1)
                referenceValue = 19.5;
            else
                throw new ArgumentOutOfRangeException("refLevel", "Invalid reference level");

            // 기준 전류값을 표시할 레이블이나 다른 UI 요소에 설정
            Label_RefValue.Text = referenceValue.ToString("0.0");
        }
        private void Button_Measure_Click(object sender, EventArgs e)
        {
            FRunMode = RunMode.rmNon;
            ToggleAutoLoop(true); // 루프 모드 활성화
            EnableStartButton(); // Start 버튼 활성화
            MessageBox.Show("측정 모드 시작");
        }
        private void Button_OutputCheck_Click(object sender, EventArgs e)
        {
            FRunMode = RunMode.rmTestOut;
            ToggleAutoLoop(true); // 루프 모드 활성화
            EnableStartButton(); // Start 버튼 활성화
            MessageBox.Show("출력 검증 모드 시작");
        }
        private void Button_OutputCalibrate_Click(object sender, EventArgs e)
        {
            FRunMode = RunMode.rmSetOut;
            ToggleAutoLoop(true); // 루프 모드 활성화
            EnableStartButton(); // Start 버튼 활성화
            MessageBox.Show("출력 교정 모드 시작");
        }

        private void Button_PressureCheck_Click(object sender, EventArgs e)
        {
            FRunMode = RunMode.rmTestPre;
            ToggleAutoLoop(true); // 루프 모드 활성화
            EnableStartButton(); // Start 버튼 활성화
            MessageBox.Show("차압 검증 모드 시작");
        }

        private void Button_PressureCalibrate_Click(object sender, EventArgs e)
        {
            FRunMode = RunMode.rmSetPre;
            ToggleAutoLoop(true); // 루프 모드 활성화
            EnableStartButton(); // Start 버튼 활성화
            MessageBox.Show("차압 교정 모드 시작");
        }

        private void Button_Stop_Click(object sender, EventArgs e)
        {
            ToggleAutoLoop(false);
            Timer_RunProcess.Stop();

            if (ComPort1.IsOpen) ComPort1.Close();
            if (ComPort2.IsOpen) ComPort2.Close();

            ComboBox_ComPort1.Enabled = true;
            ComboBox_ComPort2.Enabled = true;

            MessageBox.Show("통신 종료");
        }

        private void RunTestOut()
        {
            if (Panel_Ref == null || Panel_MeasValue == null)
            {
                MessageBox.Show("UI 요소가 초기화되지 않았습니다.");
                return;
            }

            string strVal = Panel_MeasValue.Text;
            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                switch (FRunStep)
                {
                    case 0:
                        if (FTimerSec >= 2)
                        {
                            if (Panel_Ref?.Tag != null && (int)Panel_Ref.Tag != 0)
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
                        // 추가적인 단계를 여기에 추가할 수 있습니다.
                        break;
                }
            }
            else
            {
                if (Panel_Ref?.Tag != null && (int)Panel_Ref.Tag != 0)
                {
                    SetRef_mA(0);
                }
                FRunStep = 0;
                FTimerSec = 0;
            }
        }
        private void RunSetupOut()
        {
            if (Panel_Ref == null || Panel_MeasValue == null || Label_RefError == null)
            {
                MessageBox.Show("필수 UI 요소가 초기화되지 않았습니다.");
                return;
            }

            string strVal = Panel_MeasValue.Text;
            if (strVal != STR_NON_VALUE && !string.IsNullOrEmpty(strVal))
            {
                // refError를 switch문 외부에 선언하고 초기화
                double refError = double.TryParse(Label_RefError.Text, out refError) ? refError : 0;

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
                            if (Panel_Ref.Tag?.ToString() != "0")
                            {
                                SetRef_mA(0);  // Set low reference value
                            }
                            FRunStep++;
                            FTimerSec = 0;
                            Com2_TxStr("#T0");  // Output 4mA
                        }
                        break;

                    case 2:
                        // 이후 단계 로직 작성
                        break;
                }
            }
            else
            {
                if (Panel_Ref.Tag?.ToString() != "0")
                {
                    SetRef_mA(0);
                }
                FRunStep = 0;
                FTimerSec = 0;
            }
        }

        private void RunTestPres()
        {
            if (!string.IsNullOrEmpty(FCom2_RxStr))
            {   //테스트
                Console.WriteLine($"[RunTestPres] Received FCom2_RxStr: {FCom2_RxStr}");
                Panel_MeasValue.Text = FCom2_RxStr;
                //테스트

                // FCom2_RxStr을 이용하여 수신된 차압 데이터를 변환
                if (double.TryParse(FCom2_RxStr, out double pressureValue))
                {
                    // 차압 값 표시
                    Panel_MeasValue.Text = pressureValue.ToString("0.0");

                    // Panel_Message 및 Label_Message 업데이트
                    Label_Message.Text = $"현재 차압 값: {pressureValue} mH2O";
                    Panel_Message.BackColor = pressureValue > 0 ? Color.Lime : Color.Yellow;
                }
                else
                {
                    // 데이터가 유효하지 않은 경우 기본값 설정
                    Panel_MeasValue.Text = STR_NON_VALUE;
                    Label_Message.Text = "차압 데이터 없음";
                    Panel_Message.BackColor = Color.Gray;
                }

                FCom2_RxStr = ""; // 처리 후 데이터 초기화
            }
            else
            {
                // 데이터 수신 대기 상태 표시
                Panel_MeasValue.Text = STR_NON_VALUE;
                Label_Message.Text = "차압 검증 모드 대기 중";
                Panel_Message.BackColor = Color.Yellow;
            }
        }

        private void RunSetupPres()
        {
            string strVal = Panel_MeasValue.Text;

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

        private void Button_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ComPort1.IsOpen) ComPort1.Open();
                if (!ComPort2.IsOpen) ComPort2.Open(); // ComPort2가 열리는지 확인

                if (ComPort2.IsOpen)
                {
                    Debug.WriteLine("ComPort2가 성공적으로 열렸습니다.");
                }
                else
                {
                    Debug.WriteLine("ComPort2를 열 수 없습니다.");
                }

                ComboBox_ComPort1.Enabled = false;
                ComboBox_ComPort2.Enabled = false;

                ToggleAutoLoop(true);
                Timer_RunProcess.Start();

                ExecuteRunMode();
                MessageBox.Show("통신 시작 및 선택된 작업 실행 중...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("포트를 열 수 없습니다: " + ex.Message);
            }
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

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern int GetPrivateProfileString(
        string section, string key, string defaultValue, StringBuilder returnValue, int size, string filePath);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool WritePrivateProfileString(
        string section, string key, string value, string filePath);

    public void WriteString(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value, filePath);
    }

    public string ReadString(string section, string key, string defaultValue)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(section, key, defaultValue, temp, 255, filePath);
        return temp.ToString();
    }
}



