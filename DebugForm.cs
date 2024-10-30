using System;
using System.Text;
using System.Windows.Forms;

namespace p_1_Delphi_to_C_sharp
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        // 디버그 메시지를 출력하는 메서드
        public void WriteLine(string message)
        {
            textBoxDebug.AppendText(message + Environment.NewLine); // 메시지 출력
        }

        // 헥사 형식으로 메시지를 출력하는 메서드
        public void WriteMsgHex(string prefix, string message)
        {
            if (textBoxDebug != null)
            {
                textBoxDebug.AppendText($"{prefix}{BitConverter.ToString(Encoding.ASCII.GetBytes(message))}{Environment.NewLine}");
            }
        }
        // 추가: 메시지와 정보를 표시하는 메서드
        public void WriteMsgStr(string title, string message)
        {
            textBoxDebug.AppendText($"{title}: {message}{Environment.NewLine}");
        }

        // DebugForm 클래스에 추가
        public void WriteStr(string message)
        {
            textBoxDebug.AppendText(message + Environment.NewLine); // 메시지 출력
        }
        private void DebugForm_Load(object sender, EventArgs e)
        {

        }
    }
}
