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

        private void DebugForm_Load(object sender, EventArgs e)
        {

        }
    }
}
