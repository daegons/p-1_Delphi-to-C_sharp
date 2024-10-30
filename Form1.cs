using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace p_1_Delphi_to_C_sharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }















        private void Button_AReset_Click(object sender, EventArgs e)
        {
            // A Reset 명령 전송
            Com2_SendCmd("#O0");
        }
        private void Button_AZero_Click(object sender, EventArgs e)
        {
            // A Offset 값으로 Zero 설정
            if (!string.IsNullOrEmpty(Edit_AOffset.Text))
            {
                float offset = float.Parse(Edit_AOffset.Text);
                string tx_str = "#O1" + GetFloatToStr(offset);
                Com2_SendCmd(tx_str);
            }
        }
        private void Button_AGain_Click(object sender, EventArgs e)
        {
            // A Gain 값 설정
            if (!string.IsNullOrEmpty(Edit_AGain.Text))
            {
                float gain = float.Parse(Edit_AGain.Text);
                string tx_str = "#O2" + GetFloatToStr(gain);
                Com2_SendCmd(tx_str);
            }
        }
        private void Button_ASetAll_Click(object sender, EventArgs e)
        {
            // A Offset 및 Gain 값 설정
            if (!string.IsNullOrEmpty(Edit_AOffset.Text) && !string.IsNullOrEmpty(Edit_AGain.Text))
            {
                float offset = float.Parse(Edit_AOffset.Text);
                float gain = float.Parse(Edit_AGain.Text);
                string tx_str = "#O3" + GetFloatToStr(offset) + GetFloatToStr(gain);
                Com2_SendCmd(tx_str);
            }
        }
        private void Button_Debug_Click(object sender, EventArgs e)
        {
            // 디버그 폼 표시
            fDebug.Show();
        }
        private void Button_PReset_Click(object sender, EventArgs e)
        {
            // P Reset 명령 전송
            Com2_SendCmd("#P0");
        }
        private void Button_PZero_Click(object sender, EventArgs e)
        {
            // P Zero 명령 전송
            Com2_SendCmd("#P1");
        }
        private void Button_PGain_Click(object sender, EventArgs e)
        {
            // 차압 교정값 설정
            if (!string.IsNullOrEmpty(Edit_PGain.Text))
            {
                try
                {
                    float gain = float.Parse(Edit_PGain.Text);
                    string tx_str = "#P2" + GetFloatToStr(gain);
                    Com2_SendCmd(tx_str);
                }
                catch
                {
                    // 예외 발생 시 처리
                }
            }
        }
        private void Button_PSetAll_Click(object sender, EventArgs e)
        {
            // 차압 교정값(Offset, Gain) 설정
            if (!string.IsNullOrEmpty(Edit_POffset.Text) && !string.IsNullOrEmpty(Edit_PGain.Text))
            {
                try
                {
                    float offset = float.Parse(Edit_POffset.Text);
                    float gain = float.Parse(Edit_PGain.Text);
                    string tx_str = "#P3" + GetFloatToStr(offset) + GetFloatToStr(gain);
                    Com2_SendCmd(tx_str);
                }
                catch
                {
                    // 예외 발생 시 처리
                }
            }
        }
        private void Button_PSetValue_Click(object sender, EventArgs e)
        {
            if (Button_PSetValue.Enabled)
            {
                // 수동으로 교정 설정
                float refValue = float.Parse(Edit_PSetValue.Text);
                float meas = float.Parse(Label_MeasValue.Text);
                float gain = refValue / meas;
                string tx_str = "#P2" + GetFloatToStr(gain);
                Com2_SendCmd(tx_str);

                // 설정값을 INI 파일에 저장
                FSetup.WriteString("TestPress", "SetPres", Edit_PSetValue.Text);
                Button_PSetValue.Enabled = false;
                FRunStep++;
            }
        }
        private void Button_Exit_Click(object sender, EventArgs e)
        {
            // 폼 닫기
            this.Close();
        }
        // 교정 설정 정보 읽기 버튼
        private void Button_GetInfo_Click(object sender, EventArgs e)
        {
            // 교정 설정 정보를 읽기 위해 '#I' 명령을 전송
            Com2_SendCmd("#I");
        }
        // 작동 모드 시작 버튼
        private void Button_Start_Click(object sender, EventArgs e)
        {
            if (Button_Start.Enabled)
            {
                Button_Start.Enabled = false;
                Button_Stop.Enabled = true;

                CheckBox_AutoLoop.Checked = true;  // AutoLoop 시작
            }
        }
        // 작동 모드 정지 버튼
        private void Button_Stop_Click(object sender, EventArgs e)
        {
            if (Button_Stop.Enabled)
            {
                Button_Stop.Enabled = false;
                Button_Start.Enabled = true;

                CheckBox_AutoLoop.Checked = false;  // AutoLoop 중지
            }
        }
        // 설정 저장 버튼
        private void Button_SaveConfig_Click(object sender, EventArgs e)
        {
            FSetup.WriteString("ComPort", "Port1", ComPort1.PortName);
            FSetup.WriteString("ComPort", "RxDelay1", Edit_RxDelay1.Text);
            FSetup.WriteString("ComPort", "TxDelay1", Edit_TxDelay1.Text);

            FSetup.WriteString("ComPort", "Port2", ComboBox_ComPort2.Text);
            FSetup.WriteString("ComPort", "RxDelay2", Edit_RxDelay2.Text);
            FSetup.WriteString("ComPort", "TxDelay2", Edit_TxDelay2.Text);
        }
        // 20mA 출력 설정 버튼
        private void Button_SetOut20mA_Click(object sender, EventArgs e)
        {
            Com2_SendCmd("#T1");  // 20mA 출력 명령 전송
        }
        // 4mA 출력 설정 버튼
        private void Button_SetOut4mA_Click(object sender, EventArgs e)
        {
            Com2_SendCmd("#T0");  // 4mA 출력 명령 전송
        }
        // 기준 값 저장 버튼
        private void Com1_MODBUS_MWrite(ushort reg, ushort[] ary_data)
        {
            int count = ary_data.Length;
            string str = ((char)FDevAddr).ToString() + ((char)0x10).ToString() + ((char)(reg >> 8)).ToString() + ((char)reg).ToString();
            str += ((char)(count >> 8)).ToString() + ((char)count).ToString() + ((char)(count * 2)).ToString();

            // 데이터를 문자열로 변환하여 추가
            for (int i = 0; i < count; i++)
            {
                str += ((char)(ary_data[i] >> 8)).ToString() + ((char)ary_data[i]).ToString();
            }

            // CRC 계산
            ushort crc = Calculate_CRC_MODBUS(str);
            string tx_str = str + ((char)crc).ToString() + ((char)(crc >> 8)).ToString(); // <<< crc 반대로 추가

            // AutoLoop 설정에 따라 명령을 설정하거나 즉시 전송
            if (CheckBox_AutoLoop.Checked)
            {
                FCom1_TxCmd = tx_str;
            }
            else
            {
                Com1_TxStr(tx_str);
            }
        }
        // 단일 레지스터 쓰기
        private void Com1_MODBUS_SWrite(byte addr, ushort reg, ushort data)
        {
            string str = ((char)addr).ToString() + ((char)0x06).ToString() + ((char)(reg >> 8)).ToString() + ((char)reg).ToString() +
                         ((char)(data >> 8)).ToString() + ((char)data).ToString();

            // CRC 계산
            ushort crc = Calculate_CRC_MODBUS(str);
            string tx_str = str + ((char)crc).ToString() + ((char)(crc >> 8)).ToString(); // <<< crc 반대로 추가

            // AutoLoop 설정에 따라 명령을 설정하거나 즉시 전송
            if (CheckBox_AutoLoop.Checked)
            {
                FCom1_TxCmd = tx_str;
            }
            else
            {
                Com1_TxStr(tx_str);
            }
        }
        // 다중 레지스터 읽기
        private void Com1_MODBUS_Read(byte addr, ushort reg, ushort count)
        {
            string str = ((char)addr).ToString() + ((char)0x03).ToString() + ((char)(reg >> 8)).ToString() + ((char)reg).ToString() +
                         ((char)(count >> 8)).ToString() + ((char)count).ToString();

            // CRC 계산
            ushort crc = Calculate_CRC_MODBUS(str);
            string tx_str = str + ((char)crc).ToString() + ((char)(crc >> 8)).ToString(); // <<< crc 반대로 추가

            // AutoLoop 설정에 따라 명령을 설정하거나 즉시 전송
            if (CheckBox_AutoLoop.Checked)
            {
                FCom1_TxCmd = tx_str;
            }
            else
            {
                Com1_TxStr(tx_str);
            }
        }
        private void Com1_TxStr(string str)
        {
            // ComPort1이 연결되어 있을 때, 전달된 문자열을 전송하고 LED 상태를 토글
            if (ComPort1.IsOpen)
            {
                ComPort1.Write(str);
                FCom1_TxStr = str;
                LED_Tx1.Value = !LED_Tx1.Value;
            }
        }
        private void Com2_SendCmd(string str)
        {
            // Timer_Com2Tx가 활성화되어 있을 때 전송 명령 저장, 아니면 즉시 전송
            if (Timer_Com2Tx.Enabled)
            {
                FCom2_TxCmd = str;
            }
            else
            {
                Com2_TxStr(str);
            }
        }
        private void Com2_TxStr(string str)
        {
            // ComPort2가 연결되어 있을 때, 문자열을 전송하고 LED 상태를 토글
            if (ComPort2.IsOpen)
            {
                ComPort2.Write(str);
                LED_Tx2.Value = !LED_Tx2.Value;
            }
        }
        private void ComboBox_ComPort1DropDown(object sender, EventArgs e)
        {
            // ComboBox_ComPort1이 열릴 때 사용 가능한 포트 목록을 추가
            var strList = new List<string>();
            EnumComPorts(strList);  // 사용 가능한 포트를 가져오는 메서드 호출
            ((ComboBox)sender).Items.Clear();
            ((ComboBox)sender).Items.AddRange(strList.ToArray());
        }
        private void ComboBox_ComPort1KeyDown(object sender, KeyEventArgs e)
        {
            // ComboBox_ComPort1에서 키 입력을 막기 위해 KeyDown 이벤트 무효화
            e.SuppressKeyPress = true;
        }
        private void ComboBox_ComPort1Select(object sender, EventArgs e)
        {
            // ComboBox_ComPort1에서 항목 선택 시, Tag가 0일 경우 ComPort1의 포트 설정
            if ((int)ComboBox_ComPort1.Tag == 0)
            {
                ComPort1.PortName = ComboBox_ComPort1.Text;
            }
        }
        private void ComboBox_ComPort2Select(object sender, EventArgs e)
        {
            // ComboBox_ComPort2에서 항목 선택 시, Tag가 0일 경우 ComPort2의 포트 설정
            if ((int)ComboBox_ComPort2.Tag == 0)
            {
                ComPort2.PortName = ComboBox_ComPort2.Text;
            }
        }
        private void ComPort1AfterClose(object sender, EventArgs e)
        {
            // ComPort1이 닫힐 때 상태 표시줄에 'CLOSE' 표시
            StatusBar.Panels[1].Text = "CLOSE";
        }
        private void ComPort1AfterOpen(object sender, EventArgs e)
        {
            // ComPort1이 열릴 때 상태 표시줄에 포트 이름과 보드레이트 표시
            StatusBar.Panels[1].Text = $"{ComPort1.PortName}/{ComPort1.BaudRate}";
        }
        private void ComPort1RxChar(object sender, SerialDataReceivedEventArgs e)
        {
            // 데이터 수신 시 타이머 비활성화 후 수신 데이터를 FCom1_RxStr에 추가
            Timer_Com1Rx.Enabled = false;

            // ComPort1로부터 데이터 읽기
            string receivedData = ComPort1.ReadExisting();
            FCom1_RxStr += receivedData;
            FDebCount += receivedData.Length;

            // 타이머 지연 시간 설정 후 활성화
            Timer_Com1Rx.Interval = FCom1_RxDelay;
            Timer_Com1Rx.Enabled = true;
        }
        private void ComPort2AfterClose(object sender, EventArgs e)
        {
            // ComPort2가 닫힐 때 상태 표시줄에 'CLOSE' 표시
            StatusBar.Panels[3].Text = "CLOSE";
        }
        private void ComPort2AfterOpen(object sender, EventArgs e)
        {
            // ComPort2가 열릴 때 상태 표시줄에 포트 이름과 보드레이트 표시
            StatusBar.Panels[3].Text = $"{ComPort2.PortName}/{ComPort2.BaudRate}";
        }
        private void ComPort2RxChar(object sender, SerialDataReceivedEventArgs e)
        {
            // 데이터 수신 시 타이머 비활성화 후 수신 데이터를 추가
            Timer_Com2Rx.Enabled = false;

            // ComPort2로부터 데이터 읽기
            string receivedData = ComPort2.ReadExisting();
            FCom2_RxStr += receivedData;

            // 타이머 지연 시간 설정 후 활성화
            Timer_Com2Rx.Interval = FCom2_RxDelay;
            Timer_Com2Rx.Enabled = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 폼이 닫힐 때 포트들을 안전하게 닫고 설정 파일 해제
            try
            {
                if (ComPort1.IsOpen) ComPort1.Close();
                if (ComPort2.IsOpen) ComPort2.Close();
                FSetup.Dispose();
            }
            catch
            {
                // 예외 발생 시 추가 조치는 하지 않음
            }
        }
        private bool CheckComPort(string port)
        {
            // 주어진 포트가 사용 가능한 포트 목록에 있는지 확인
            var portList = System.IO.Ports.SerialPort.GetPortNames();
            if (!string.IsNullOrEmpty(port))
            {
                foreach (string availablePort in portList)
                {
                    if (availablePort.Trim() == port)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void ComPortInit()
        {
            // ComPort1 초기화
            ComPort1.PortName = "";
            ComboBox_ComPort1.Text = "";
            string port1 = FSetup.ReadString("ComPort", "Port1", "");

            if (CheckComPort(port1))
            {
                ComboBox_ComPort1.Text = port1;
                ComPort1.PortName = port1;
                ComPort1.Open(); // ComPort1을 엽니다.
            }

            // Rx/Tx 지연 시간을 설정 파일에서 읽고 저장
            Edit_RxDelay1.Text = FSetup.ReadString("ComPort", "RxDelay1", "100");
            Edit_TxDelay1.Text = FSetup.ReadString("ComPort", "TxDelay1", "500");
            FCom1_RxDelay = int.Parse(Edit_RxDelay1.Text);
            FCom1_TxDelay = int.Parse(Edit_TxDelay1.Text);

            // ComPort2 초기화
            ComPort2.PortName = "";
            ComboBox_ComPort2.Text = "";
            string port2 = FSetup.ReadString("ComPort", "Port2", "");

            // ComPort1과 다른 포트인지 확인하고, 사용 가능하면 열기
            if (port2 != ComPort1.PortName && CheckComPort(port2))
            {
                ComboBox_ComPort2.Text = port2;
                ComPort2.PortName = port2;
                ComPort2.Open(); // ComPort2를 엽니다.
            }

            // ComPort2의 Rx/Tx 지연 시간 설정
            Edit_RxDelay2.Text = FSetup.ReadString("ComPort", "RxDelay2", "100");
            Edit_TxDelay2.Text = FSetup.ReadString("ComPort", "TxDelay2", "500");
            FCom2_RxDelay = int.Parse(Edit_RxDelay2.Text);
            FCom2_TxDelay = int.Parse(Edit_TxDelay2.Text);
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            // 폼이 표시될 때의 초기화 코드. 비워둠.
        }
        private string GetFloatToStr(float value)
        {
            // float 값을 문자열로 변환하여 반환
            byte[] buff = BitConverter.GetBytes(value);
            Array.Reverse(buff); // 바이트 순서 반전 (Delphi에서의 Move 함수 대체)

            // 바이트 배열을 문자열로 변환
            return Encoding.Default.GetString(buff);
        }
        private void FormMain_Shown(object sender, EventArgs e)
        {
            // 폼이 생성될 때 설정 파일을 읽고 초기화 작업을 수행
            string path = Path.Combine(Application.StartupPath, "Setup.ini");
            FSetup = new IniFile(path);  // IniFile 인스턴스 생성 (IniFile은 커스텀 클래스)

            // 설정 파일에서 초기 채널 값을 읽어 설정
            int num = FSetup.ReadInteger("TestOut", "Channel", 1);
            Edit_Channel1.Text = num.ToString();
            FChIdx = num - 1;

            // 압력 상한값 설정
            FPresHighLevel = FSetup.ReadFloat("TestPress", "HighLevel", 450);

            // 측정 모드로 초기화
            Panel_SetupRunMode(Panel_Meas);

            // 수동 교정 설정값을 읽어 텍스트 상자에 설정
            Edit_PSetValue.Text = FSetup.ReadString("TestPress", "SetPres", "0.0");

            // COM 포트를 초기화하고 페이지 컨트롤 상태 변경
            ComPortInit();
            PageControl_SelectedIndexChanged(PageControl, EventArgs.Empty);
        }
        private void PageControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 탭 변경 후 이벤트 처리
            if (PageControl.SelectedTab == TS_Config)
            {
                // 설정 탭이 선택된 경우, 모든 COM 포트를 닫음
                ComPort1.Close();
                ComPort2.Close();
            }
            else if (PageControl.SelectedTab == TS_Measure)
            {
                // 측정 탭이 선택된 경우, COM 포트 상태에 따라 상태바 색상 변경
                if (ComPort1.IsOpen && ComPort2.IsOpen)
                {
                    StatusBar.BackColor = SystemColors.Control;
                }
                else
                {
                    StatusBar.BackColor = Color.Red;
                }
            }
        }
        // 변경 시작 전에 호출되는 메서드
        private void PageControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // 자동 루프가 활성화되어 있는 경우, 탭 변경을 허용하지 않음
            if (CheckBox_AutoLoop.Checked)
            {
                e.Cancel = true; // 탭 변경을 취소
                return;
            }

            // 설정 탭에서 벗어날 때 COM 포트를 다시 엶
            if (PageControl.SelectedTab == TS_Config)
            {
                if (!string.IsNullOrEmpty(ComPort1.PortName)) ComPort1.Open();
                if (!string.IsNullOrEmpty(ComPort2.PortName)) ComPort2.Open();
            }
        }
        private void CheckBox_AutoLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_AutoLoop.Checked)
            {
                switch (FRunMode)
                {
                    case RunMode.rmNon:
                        StatusBar.Panels[4].Text = "측 정";

                        FCom1_TxCmd = "";
                        FCom1_RxStr = "";
                        FCom1_TxNum = 0;
                        Timer_Com1Tx.Interval = 10;
                        Timer_Com1Tx.Enabled = true;
                        break;

                    case RunMode.rmTestOut:
                        StatusBar.Panels[4].Text = "출력 검증";

                        FCom1_TxCmd = "";
                        FCom1_RxStr = "";
                        FCom1_TxNum = 0;
                        Timer_Com1Tx.Interval = 10;
                        Timer_Com1Tx.Enabled = true;
                        break;

                    case RunMode.rmSetOut:
                        StatusBar.Panels[4].Text = "출력 교정";

                        FCom1_TxCmd = "";
                        FCom1_RxStr = "";
                        FCom1_TxNum = 0;
                        Timer_Com1Tx.Interval = 10;
                        Timer_Com1Tx.Enabled = true;
                        break;

                    case RunMode.rmTestPre:
                        StatusBar.Panels[4].Text = "차압 검증";

                        FCom2_TxCmd = "";
                        FCom2_RxStr = "";
                        Timer_Com2Tx.Interval = 10;
                        Timer_Com2Tx.Enabled = true;
                        break;

                    case RunMode.rmSetPre:
                        StatusBar.Panels[4].Text = "차압 교정";

                        FCom2_TxCmd = "";
                        FCom2_RxStr = "";
                        Timer_Com2Tx.Interval = 10;
                        Timer_Com2Tx.Enabled = true;
                        break;
                }

                // 모드 설정 그룹박스 비활성화
                GroupBox_SetMode.Enabled = false;

                // 프로세스 타이머 활성화
                Timer_RunPross.Enabled = true;
            }
            else
            {
                // "정지" 상태로 변경
                StatusBar.Panels[4].Text = "정 지";

                // 프로세스 타이머 비활성화 및 모드 설정 그룹박스 활성화
                Timer_RunPross.Enabled = false;
                GroupBox_SetMode.Enabled = true;
            }
        }
        private void DispMeasMode()
        {
            // 측정 모드에서의 값 표시
            double measVal = FBuff_mA[FChIdx];
            string strVal;

            // 측정 값이 3.0 이하이면 비어 있는 값을 표시
            if (measVal <= 3.0)
            {
                strVal = STR_NON_VALUE;
            }
            else
            {
                // 3.0 이상일 경우 값 포맷팅하여 문자열로 변환
                strVal = measVal.ToString("0.000");
            }
            Label_MeasValue.Text = strVal;
        }
        private void DispTestOutMode()
        {
            // 4 ~ 20mA 출력 검증 및 교정 모드
            double value = FBuff_mA[FChIdx];
            double errVal, refVal, refErr;
            string strVal, strErr;
            Color color;

            // 측정 값이 3.0 이하이면 비어 있는 값을 표시
            if (value <= 3.0)
            {
                color = SystemColors.Control;
                strVal = STR_NON_VALUE;
                strErr = STR_NON_VALUE;
            }
            else
            {
                // 기준값과 오차값 계산
                refVal = double.Parse(Label_RefValue.Text);
                errVal = refVal - value;

                strVal = value.ToString("0.000");
                strErr = errVal.ToString("0.000");
                refErr = double.Parse(Label_RefError.Text);

                // 오차 절대값과 기준 오차 비교하여 색상 결정
                errVal = Math.Abs(errVal);
                color = errVal > refErr ? Color.Red : Color.Lime;
            }

            // 화면에 측정값, 오차값, 그리고 색상 적용
            Label_MeasValue.Text = strVal;
            Label_MeasError.Text = strErr;

            Label_MeasValue.Parent.BackColor = color;
            Label_RefValue.Parent.BackColor = color;
            Label_MeasError.Parent.BackColor = color;
            Label_RefError.Parent.BackColor = color;
        }
        private void DispMultiRead_MODBUS(string rxStr)
        {
            // Modbus 다중 읽기 응답을 처리하는 메서드
            int addr = (FCom1_TxStr[3] << 8) | FCom1_TxStr[4];
            int count = (FCom1_TxStr[5] << 8) | FCom1_TxStr[6];

            // 주소가 REG_mA 일 경우, ADC 데이터를 처리
            if (addr == REG_mA)
            {
                double[] aryVal = new double[ADC_CH_MAX];
                string strData = rxStr.Substring(4, count * 2);

                for (int i = 0; i < count; i++)
                {
                    aryVal[i] = StrBuff2WordDel(ref strData) / 1000.0;
                    FBuff_mA[i] = aryVal[i];
                    FBuff_mH2O[i] = (FBuff_mA[i] - 4.0) * 31.25;
                }

                // 각 채널 데이터를 포맷하여 텍스트 박스에 표시
                Edit_CH1.Text = FBuff_mA[0].ToString("0.000");
                Edit_CH2.Text = FBuff_mA[1].ToString("0.000");
                Edit_CH3.Text = FBuff_mA[2].ToString("0.000");
                Edit_CH4.Text = FBuff_mA[3].ToString("0.000");

                // 현재 모드에 따라 다른 화면 갱신 메서드를 호출
                switch (FRunMode)
                {
                    case RunMode.rmNon:
                        DispMeasMode();
                        break;

                    case RunMode.rmTestOut:
                    case RunMode.rmSetOut:
                        DispTestOutMode();
                        break;

                    default:
                        break;
                }
            }
        }
        // Modbus 데이터에서 두 바이트를 단일 워드로 변환
        private int StrBuff2WordDel(ref string strData)
        {
            int data = (strData[0] << 8) | strData[1];
            strData = strData.Substring(2);  // 변환 후 원본 문자열에서 처리된 바이트 삭제
            return data;
        }
        private void DispSingleWrite_MODBUS(string rxStr)
        {
            // 단일 쓰기 응답을 처리하기 위한 메서드 (현재 구현되지 않음)
        }
        private void DispMultiWrite_MODBUS(string rxStr)
        {
            // 다중 쓰기 응답을 처리하기 위한 메서드 (현재 구현되지 않음)
        }
        private void Timer_Com1Rx_Tick(object sender, EventArgs e)
        {
            Timer_Com1Rx.Enabled = false; // 타이머 비활성화

            try
            {
                if (!string.IsNullOrEmpty(FCom1_RxStr))
                {
                    // LED 상태를 반전하여 데이터 수신을 시각적으로 표시
                    LED_Rx1.Value = !LED_Rx1.Value;

                    // 수신된 데이터 복사 후, 버퍼 초기화
                    string rxStr = FCom1_RxStr;
                    FCom1_RxStr = string.Empty;

                    // 수신 데이터를 헥사 값으로 디버그 출력 (CheckBox가 선택된 경우)
                    if (CheckBox_RxHexView.Checked)
                    {
                        fDebug.WriteMsgHex("Rx Hex: ", rxStr);
                    }

                    // CRC 검증 통과 시, 기능 코드에 따라 처리
                    if (CheckCRC_MODBUS(rxStr))
                    {
                        switch ((byte)rxStr[2])
                        {
                            case 0x03:
                                // 다중 읽기 명령 응답
                                DispMultiRead_MODBUS(rxStr);
                                break;

                            case 0x06:
                                // 단일 쓰기 명령 응답
                                DispSingleWrite_MODBUS(rxStr);
                                break;

                            case 0x10:
                                // 다중 쓰기 명령 응답
                                DispMultiWrite_MODBUS(rxStr);
                                break;
                        }
                    }
                    else
                    {
                        // CRC 오류 시 디버그 출력
                        fDebug.WriteMsgStr("Error --", "CRC [" + rxStr.Length + "] " + FDebCount);
                    }
                }
                else
                {
                    // 데이터가 비어 있는 경우
                }

                // 디버그 카운터 초기화
                FDebCount = 0;
            }
            catch
            {
                // 예외 처리는 생략됨
            }
        }
        private void Timer_Com1Tx_Tick(object sender, EventArgs e)
        {
            Timer_Com1Tx.Enabled = false; // 타이머 일시 정지

            if (Timer_Com1Rx.Enabled)
            {
                // 수신 타이머가 활성화 상태일 경우, 전송 타이머를 10ms 후 재활성화
                Timer_Com1Tx.Interval = 10;
                Timer_Com1Tx.Enabled = true;
            }
            else
            {
                string txStr;

                // 전송 명령이 설정된 경우 해당 명령을 사용하고, 없으면 새로운 명령 생성
                if (!string.IsNullOrEmpty(FCom1_TxCmd))
                {
                    txStr = FCom1_TxCmd;
                    FCom1_TxCmd = string.Empty;
                }
                else
                {
                    ushort readMem = REG_mA;
                    // Modbus 명령 생성 (주소 $01, 기능 코드 $03, 메모리 주소 및 데이터 길이 지정)
                    txStr = $"{(char)0x01}{(char)0x03}{(char)(readMem >> 8)}{(char)readMem}{(char)0x00}{(char)0x08}";

                    // CRC16 계산하여 전송 문자열에 추가
                    ushort crc16 = Calculate_CRC_MODBUS(txStr);
                    txStr += $"{(char)crc16}{(char)(crc16 >> 8)}";
                    //fDebug.WriteMsgHex("TX HEX: ", txStr); // 디버그 메시지 출력
                }

                // 생성된 명령 전송
                Com1_TxStr(txStr);

                // 수신 타이머 설정
                Timer_Com1Rx.Interval = FCom1_RxDelay;
                Timer_Com1Rx.Enabled = true;

                // 전송 타이머 설정: AutoLoop 체크박스 상태에 따라 활성화 여부 결정
                Timer_Com1Tx.Interval = FCom1_TxDelay;
                Timer_Com1Tx.Enabled = CheckBox_AutoLoop.Checked;
            }
        }
        private void DispTestPressMode(double value)
        {
            // 주어진 값을 소수점 1자리까지 문자열로 변환하여 Label_MeasValue에 표시
            string strVal = value.ToString("0.0");
            Label_MeasValue.Text = strVal;
        }
        private void Timer_Com2Rx_Tick(object sender, EventArgs e)
        {
            Timer_Com2Rx.Enabled = false;
            string rxStr;
            double value;

            // 수신 문자열이 비어 있지 않으면 데이터 처리 시작
            if (!string.IsNullOrEmpty(FCom2_RxStr))
            {
                LED_Rx2.Value = !LED_Rx2.Value;  // 수신 LED 표시를 반전시킴
                rxStr = FCom2_RxStr;
                FCom2_RxStr = string.Empty;

                // 수신 문자열의 첫 문자가 '=' 인지 확인
                if (rxStr[0] == '=')
                {
                    switch (rxStr[1])
                    {
                        case 'G':
                            // 수신 문자열을 다듬어 값을 추출하고, float로 변환
                            rxStr = rxStr.Substring(2).Trim();
                            value = double.Parse(rxStr);

                            // 현재 실행 모드에 따라 압력 측정값을 화면에 표시
                            if (FRunMode == RunMode.rmTestPre)
                            {
                                DispTestPressMode(value);
                            }
                            else if (FRunMode == RunMode.rmSetPre)
                            {
                                DispTestPressMode(value);
                            }

                            Timer_Com2Rx.Tag = 0;  // 수신 확인 플래그 초기화
                            break;

                        case 'P':
                            // 디버그 창에 수신된 'P' 메시지 출력
                            fDebug.WriteStr("COM2 RX: " + rxStr.Trim());
                            break;

                        case 'O':
                            // 디버그 창에 수신된 'O' 메시지 출력
                            fDebug.WriteStr("COM2 RX: " + rxStr.Trim());
                            break;

                        case 'T':
                            // 디버그 창에 수신된 'T' 메시지 출력
                            fDebug.WriteStr("COM2 RX: " + rxStr.Trim());
                            break;

                        case 'I':
                            // 디버그 창에 수신된 'I' 메시지 출력
                            fDebug.WriteStr("COM2 RX: " + rxStr.Trim());
                            break;
                    }
                }
            }
        }
        private void Timer_Com2Tx_Tick(object sender, EventArgs e)
        {
            Timer_Com2Tx.Enabled = false;
            string txStr;

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
                if (!string.IsNullOrEmpty(FCom2_TxCmd))
                {
                    txStr = FCom2_TxCmd;
                    FCom2_TxCmd = string.Empty;
                }
                else
                {
                    txStr = "#G";
                }

                Com2_TxStr(txStr);

                Timer_Com2Rx.Interval = FCom2_RxDelay;
                Timer_Com2Rx.Enabled = true;

                Timer_Com2Tx.Interval = FCom2_TxDelay;
                Timer_Com2Tx.Enabled = CheckBox_AutoLoop.Checked;
            }
        }
        private void SetRef_mA(int ref)
        {
            Panel_Ref.Tag = ref;
            if (ref == 0)
            {
                Label_RefError.Text = FSetup.ReadString("TestOut", "Error", "");
                Label_RefValue.Text = LOW_REF_mA.ToString("0.000");
            }
            else
            {
                Label_RefError.Text = FSetup.ReadString("TestOut", "Error", "");
                Label_RefValue.Text = HIGH_REF_mA.ToString("0.000");
            }
        }
        private void Panel8_Click(object sender, EventArgs e)
        {
            // Empty click event handler
        }
        private void Panel_SetupRunMode(object sender, EventArgs e)
        {
            Button_PSetValue.Enabled = false;
            Panel_Message.BackColor = SystemColors.Control;

            Panel_Meas.BackColor = SystemColors.Control;
            Panel_TestOut.BackColor = SystemColors.Control;
            Panel_SetOut.BackColor = SystemColors.Control;
            Panel_TestPres.BackColor = SystemColors.Control;
            Panel_SetPres.BackColor = SystemColors.Control;

            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.Lime;
                FRunMode = (RunMode)panel.Tag;

                switch (FRunMode)
                {
                    case RunMode.rmNon:
                        Label_Message.Text = "측정 모드";
                        Label_MeasValue.Text = STR_NON_VALUE;
                        Label_MeasError.Text = STR_NON_VALUE;
                        Label_RefValue.Text = STR_NON_VALUE;
                        Label_RefError.Text = STR_NON_VALUE;
                        break;

                    case RunMode.rmTestOut:
                        Panel_Message.BackColor = Color.Yellow;
                        Label_Message.Text = "출력 검증 모드";
                        SetRef_mA(0);
                        Label_MeasValue.Text = STR_NON_VALUE;
                        Label_MeasError.Text = STR_NON_VALUE;
                        break;

                    case RunMode.rmSetOut:
                        Panel_Message.BackColor = Color.Yellow;
                        Label_Message.Text = "출력 교정 모드";
                        SetRef_mA(0);
                        Label_MeasValue.Text = STR_NON_VALUE;
                        Label_MeasError.Text = STR_NON_VALUE;
                        break;

                    case RunMode.rmTestPre:
                        Panel_Message.BackColor = Color.Yellow;
                        Label_Message.Text = "압력 검증 모드";
                        Label_RefValue.Text = STR_NON_VALUE;
                        Label_RefError.Text = FSetup.ReadString("TestPress", "Error", "");
                        Label_MeasValue.Text = STR_NON_VALUE;
                        Label_MeasError.Text = STR_NON_VALUE;
                        break;

                    case RunMode.rmSetPre:
                        Panel_Message.BackColor = Color.Yellow;
                        Label_Message.Text = "압력 교정 모드";
                        Label_RefValue.Text = STR_NON_VALUE;
                        Label_RefError.Text = FSetup.ReadString("TestPress", "Error", "");
                        Label_MeasValue.Text = STR_NON_VALUE;
                        Label_MeasError.Text = STR_NON_VALUE;
                        break;
                }

                Label_MeasValue.Parent.BackColor = Color;
                Label_RefValue.Parent.BackColor = Color;
                Label_MeasError.Parent.BackColor = Color;
                Label_RefError.Parent.BackColor = Color;
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
        private void Timer_RunPross_Tick(object sender, EventArgs e)
        {
            FTimerSec += 1;

            switch (FRunMode)
            {
                case RunMode.rmNon:
                    // Do nothing
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

            StatusBar.Panels[5].Text = "Step: " + FRunStep.ToString();
            StatusBar.Panels[6].Text = "Time: " + FTimerSec.ToString();
        }
        //초기 생성 파일
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
