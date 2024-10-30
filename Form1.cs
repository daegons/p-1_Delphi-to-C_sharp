using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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






        private void RunSetupPres()
        {
            string str_val = Label_MeasValue.Text;
            if (str_val != STR_NON_VALUE && !string.IsNullOrEmpty(str_val))
            {
                switch (FRunStep)
                {
                    case 0:
                        if (FTimerSec >= 2)
                        {
                            Label_Message.Text = "차압 교정 시작..!";
                            Com2_SendCmd("#P0");  // 교정값 리셋
                            FRunStep++;
                            FTimerSec = 0;
                        }
                        break;

                    case 1:
                        if (FTimerSec >= 2)
                        {
                            Label_Message.Text = "영점 조정 중...!";
                            Com2_SendCmd("#P1");  // 교정값 리셋
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
                        // 수동 교정 진행 대기...
                        break;

                    case 4:
                        Label_Message.Text = "교정 완료...";
                        Panel_Message.BackColor = Color.Lime;
                        FRunStep++;
                        FTimerSec = 0;
                        break;

                    case 5:
                        // '교정 완료...' 상태 유지
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
            FTimerSec++;

            switch (FRunMode)
            {
                case RunMode.rmNon:
                    // 아무 작업도 수행하지 않음
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

            // 상태 표시줄 업데이트
            StatusBar.Panels[5].Text = "Step: " + FRunStep.ToString();
            StatusBar.Panels[6].Text = "Time: " + FTimerSec.ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
