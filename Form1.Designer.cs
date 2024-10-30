namespace p_1_Delphi_to_C_sharp
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.Edit_AOffset = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Edit_AGain = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Edit_PGain = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Edit_POffset = new System.Windows.Forms.TextBox();
            this.Label_MeasValue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CheckBox_AutoLoop = new System.Windows.Forms.CheckBox();
            this.LED = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.LED_Rx2 = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.LED_Rx1 = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LED_Tx2 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.LED_Tx1 = new System.Windows.Forms.PictureBox();
            this.Button_Debug = new System.Windows.Forms.Button();
            this.Button_Stop = new System.Windows.Forms.Button();
            this.Button_Start = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Button_SetOut20mA = new System.Windows.Forms.Button();
            this.Button_PZero = new System.Windows.Forms.Button();
            this.Button_SetOut4mA = new System.Windows.Forms.Button();
            this.Button_AReset = new System.Windows.Forms.Button();
            this.Button_PReset = new System.Windows.Forms.Button();
            this.Button_SaveConfig = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.Edit_TxDelay2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Edit_RxDelay2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ComboBox_ComPort2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Edit_TxDelay1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Edit_RxDelay1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.LED)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Rx2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Rx1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Tx2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Tx1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Edit_AOffset
            // 
            this.Edit_AOffset.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_AOffset.Location = new System.Drawing.Point(16, 251);
            this.Edit_AOffset.Name = "Edit_AOffset";
            this.Edit_AOffset.Size = new System.Drawing.Size(77, 26);
            this.Edit_AOffset.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "전류 Offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "전류 Gain";
            // 
            // Edit_AGain
            // 
            this.Edit_AGain.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_AGain.Location = new System.Drawing.Point(99, 251);
            this.Edit_AGain.Name = "Edit_AGain";
            this.Edit_AGain.Size = new System.Drawing.Size(77, 26);
            this.Edit_AGain.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(97, 285);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "차압 Gain";
            // 
            // Edit_PGain
            // 
            this.Edit_PGain.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_PGain.Location = new System.Drawing.Point(99, 300);
            this.Edit_PGain.Name = "Edit_PGain";
            this.Edit_PGain.Size = new System.Drawing.Size(77, 26);
            this.Edit_PGain.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 285);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "차압 Offset";
            // 
            // Edit_POffset
            // 
            this.Edit_POffset.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_POffset.Location = new System.Drawing.Point(16, 300);
            this.Edit_POffset.Name = "Edit_POffset";
            this.Edit_POffset.Size = new System.Drawing.Size(77, 26);
            this.Edit_POffset.TabIndex = 6;
            // 
            // Label_MeasValue
            // 
            this.Label_MeasValue.AutoSize = true;
            this.Label_MeasValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label_MeasValue.Font = new System.Drawing.Font("굴림", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Label_MeasValue.Location = new System.Drawing.Point(17, 89);
            this.Label_MeasValue.Name = "Label_MeasValue";
            this.Label_MeasValue.Size = new System.Drawing.Size(117, 37);
            this.Label_MeasValue.TabIndex = 8;
            this.Label_MeasValue.Text = "00000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Font = new System.Drawing.Font("굴림", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(15, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 35);
            this.label6.TabIndex = 9;
            this.label6.Text = "측정값";
            // 
            // CheckBox_AutoLoop
            // 
            this.CheckBox_AutoLoop.AutoSize = true;
            this.CheckBox_AutoLoop.Location = new System.Drawing.Point(679, 20);
            this.CheckBox_AutoLoop.Name = "CheckBox_AutoLoop";
            this.CheckBox_AutoLoop.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_AutoLoop.TabIndex = 10;
            this.CheckBox_AutoLoop.Text = "연속작동";
            this.CheckBox_AutoLoop.UseVisualStyleBackColor = true;
            // 
            // LED
            // 
            this.LED.Location = new System.Drawing.Point(650, 48);
            this.LED.Name = "LED";
            this.LED.Size = new System.Drawing.Size(36, 32);
            this.LED.TabIndex = 11;
            this.LED.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 417);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.LED_Rx2);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.LED_Rx1);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.LED_Tx2);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.LED_Tx1);
            this.tabPage1.Controls.Add(this.Button_Debug);
            this.tabPage1.Controls.Add(this.Button_Stop);
            this.tabPage1.Controls.Add(this.Button_Start);
            this.tabPage1.Controls.Add(this.CheckBox_AutoLoop);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.Edit_POffset);
            this.tabPage1.Controls.Add(this.Label_MeasValue);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.LED);
            this.tabPage1.Controls.Add(this.Edit_PGain);
            this.tabPage1.Controls.Add(this.Edit_AGain);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.Edit_AOffset);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(768, 391);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(541, 86);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(26, 12);
            this.label13.TabIndex = 26;
            this.label13.Text = "Rx2";
            // 
            // LED_Rx2
            // 
            this.LED_Rx2.Location = new System.Drawing.Point(543, 101);
            this.LED_Rx2.Name = "LED_Rx2";
            this.LED_Rx2.Size = new System.Drawing.Size(24, 25);
            this.LED_Rx2.TabIndex = 25;
            this.LED_Rx2.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(541, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 12);
            this.label12.TabIndex = 24;
            this.label12.Text = "Rx1";
            // 
            // LED_Rx1
            // 
            this.LED_Rx1.Location = new System.Drawing.Point(543, 55);
            this.LED_Rx1.Name = "LED_Rx1";
            this.LED_Rx1.Size = new System.Drawing.Size(24, 25);
            this.LED_Rx1.TabIndex = 23;
            this.LED_Rx1.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(494, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(26, 12);
            this.label14.TabIndex = 22;
            this.label14.Text = "Tx2";
            // 
            // LED_Tx2
            // 
            this.LED_Tx2.Location = new System.Drawing.Point(496, 101);
            this.LED_Tx2.Name = "LED_Tx2";
            this.LED_Tx2.Size = new System.Drawing.Size(24, 25);
            this.LED_Tx2.TabIndex = 21;
            this.LED_Tx2.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(494, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 12);
            this.label11.TabIndex = 16;
            this.label11.Text = "Tx1";
            // 
            // LED_Tx1
            // 
            this.LED_Tx1.Location = new System.Drawing.Point(496, 55);
            this.LED_Tx1.Name = "LED_Tx1";
            this.LED_Tx1.Size = new System.Drawing.Size(24, 25);
            this.LED_Tx1.TabIndex = 15;
            this.LED_Tx1.TabStop = false;
            // 
            // Button_Debug
            // 
            this.Button_Debug.Location = new System.Drawing.Point(632, 230);
            this.Button_Debug.Name = "Button_Debug";
            this.Button_Debug.Size = new System.Drawing.Size(119, 27);
            this.Button_Debug.TabIndex = 14;
            this.Button_Debug.Text = "debug";
            this.Button_Debug.UseVisualStyleBackColor = true;
            this.Button_Debug.Click += new System.EventHandler(this.Button_Debug_Click);
            // 
            // Button_Stop
            // 
            this.Button_Stop.Location = new System.Drawing.Point(632, 168);
            this.Button_Stop.Name = "Button_Stop";
            this.Button_Stop.Size = new System.Drawing.Size(119, 47);
            this.Button_Stop.TabIndex = 13;
            this.Button_Stop.Text = "STOP";
            this.Button_Stop.UseVisualStyleBackColor = true;
            // 
            // Button_Start
            // 
            this.Button_Start.Location = new System.Drawing.Point(632, 115);
            this.Button_Start.Name = "Button_Start";
            this.Button_Start.Size = new System.Drawing.Size(119, 47);
            this.Button_Start.TabIndex = 12;
            this.Button_Start.Text = "START";
            this.Button_Start.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.Button_SetOut20mA);
            this.tabPage2.Controls.Add(this.Button_PZero);
            this.tabPage2.Controls.Add(this.Button_SetOut4mA);
            this.tabPage2.Controls.Add(this.Button_AReset);
            this.tabPage2.Controls.Add(this.Button_PReset);
            this.tabPage2.Controls.Add(this.Button_SaveConfig);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.Edit_TxDelay2);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.Edit_RxDelay2);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.ComboBox_ComPort2);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.Edit_TxDelay1);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.Edit_RxDelay1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(768, 391);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Button_SetOut20mA
            // 
            this.Button_SetOut20mA.Location = new System.Drawing.Point(446, 316);
            this.Button_SetOut20mA.Name = "Button_SetOut20mA";
            this.Button_SetOut20mA.Size = new System.Drawing.Size(55, 23);
            this.Button_SetOut20mA.TabIndex = 17;
            this.Button_SetOut20mA.Text = "20mA";
            this.Button_SetOut20mA.UseVisualStyleBackColor = true;
            this.Button_SetOut20mA.Click += new System.EventHandler(this.Button_SetOut20mA_Click);
            // 
            // Button_PZero
            // 
            this.Button_PZero.Location = new System.Drawing.Point(161, 345);
            this.Button_PZero.Name = "Button_PZero";
            this.Button_PZero.Size = new System.Drawing.Size(55, 23);
            this.Button_PZero.TabIndex = 16;
            this.Button_PZero.Text = "Zero";
            this.Button_PZero.UseVisualStyleBackColor = true;
            this.Button_PZero.Click += new System.EventHandler(this.Button_PZero_Click);
            // 
            // Button_SetOut4mA
            // 
            this.Button_SetOut4mA.Location = new System.Drawing.Point(385, 316);
            this.Button_SetOut4mA.Name = "Button_SetOut4mA";
            this.Button_SetOut4mA.Size = new System.Drawing.Size(55, 23);
            this.Button_SetOut4mA.TabIndex = 15;
            this.Button_SetOut4mA.Text = "4mA";
            this.Button_SetOut4mA.UseVisualStyleBackColor = true;
            this.Button_SetOut4mA.Click += new System.EventHandler(this.Button_SetOut4mA_Click);
            // 
            // Button_AReset
            // 
            this.Button_AReset.Location = new System.Drawing.Point(100, 316);
            this.Button_AReset.Name = "Button_AReset";
            this.Button_AReset.Size = new System.Drawing.Size(55, 23);
            this.Button_AReset.TabIndex = 14;
            this.Button_AReset.Text = "Reset";
            this.Button_AReset.UseVisualStyleBackColor = true;
            this.Button_AReset.Click += new System.EventHandler(this.Button_AReset_Click);
            // 
            // Button_PReset
            // 
            this.Button_PReset.Location = new System.Drawing.Point(100, 345);
            this.Button_PReset.Name = "Button_PReset";
            this.Button_PReset.Size = new System.Drawing.Size(55, 23);
            this.Button_PReset.TabIndex = 11;
            this.Button_PReset.Text = "Reset";
            this.Button_PReset.UseVisualStyleBackColor = true;
            this.Button_PReset.Click += new System.EventHandler(this.Button_PReset_Click);
            // 
            // Button_SaveConfig
            // 
            this.Button_SaveConfig.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_SaveConfig.Location = new System.Drawing.Point(247, 166);
            this.Button_SaveConfig.Name = "Button_SaveConfig";
            this.Button_SaveConfig.Size = new System.Drawing.Size(135, 46);
            this.Button_SaveConfig.TabIndex = 10;
            this.Button_SaveConfig.Text = "저 장";
            this.Button_SaveConfig.UseVisualStyleBackColor = true;
            this.Button_SaveConfig.Click += new System.EventHandler(this.Button_SaveConfig_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(20, 190);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 21);
            this.label9.TabIndex = 9;
            this.label9.Text = "TxDelay2";
            // 
            // Edit_TxDelay2
            // 
            this.Edit_TxDelay2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Edit_TxDelay2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_TxDelay2.Location = new System.Drawing.Point(120, 189);
            this.Edit_TxDelay2.Name = "Edit_TxDelay2";
            this.Edit_TxDelay2.Size = new System.Drawing.Size(74, 22);
            this.Edit_TxDelay2.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(20, 166);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 21);
            this.label10.TabIndex = 7;
            this.label10.Text = "RxDelay2";
            // 
            // Edit_RxDelay2
            // 
            this.Edit_RxDelay2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Edit_RxDelay2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_RxDelay2.Location = new System.Drawing.Point(120, 165);
            this.Edit_RxDelay2.Name = "Edit_RxDelay2";
            this.Edit_RxDelay2.Size = new System.Drawing.Size(74, 22);
            this.Edit_RxDelay2.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(21, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 21);
            this.label8.TabIndex = 5;
            this.label8.Text = "PORT";
            // 
            // ComboBox_ComPort2
            // 
            this.ComboBox_ComPort2.Font = new System.Drawing.Font("굴림", 9.75F);
            this.ComboBox_ComPort2.FormattingEnabled = true;
            this.ComboBox_ComPort2.Location = new System.Drawing.Point(120, 136);
            this.ComboBox_ComPort2.Name = "ComboBox_ComPort2";
            this.ComboBox_ComPort2.Size = new System.Drawing.Size(74, 21);
            this.ComboBox_ComPort2.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(21, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 21);
            this.label7.TabIndex = 3;
            this.label7.Text = "TxDelay1";
            // 
            // Edit_TxDelay1
            // 
            this.Edit_TxDelay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Edit_TxDelay1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_TxDelay1.Location = new System.Drawing.Point(121, 80);
            this.Edit_TxDelay1.Name = "Edit_TxDelay1";
            this.Edit_TxDelay1.Size = new System.Drawing.Size(74, 22);
            this.Edit_TxDelay1.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(21, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "RxDelay1";
            // 
            // Edit_RxDelay1
            // 
            this.Edit_RxDelay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Edit_RxDelay1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Edit_RxDelay1.Location = new System.Drawing.Point(121, 56);
            this.Edit_RxDelay1.Name = "Edit_RxDelay1";
            this.Edit_RxDelay1.Size = new System.Drawing.Size(74, 22);
            this.Edit_RxDelay1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LED)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Rx2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Rx1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Tx2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LED_Tx1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Edit_AOffset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Edit_AGain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Edit_PGain;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Edit_POffset;
        private System.Windows.Forms.Label Label_MeasValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox CheckBox_AutoLoop;
        private System.Windows.Forms.PictureBox LED;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Edit_RxDelay1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Edit_TxDelay1;
        private System.Windows.Forms.ComboBox ComboBox_ComPort2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Edit_TxDelay2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Edit_RxDelay2;
        private System.Windows.Forms.Button Button_Stop;
        private System.Windows.Forms.Button Button_Start;
        private System.Windows.Forms.Button Button_Debug;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox LED_Tx2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox LED_Tx1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox LED_Rx2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox LED_Rx1;
        private System.Windows.Forms.Button Button_SaveConfig;
        private System.Windows.Forms.Button Button_SetOut20mA;
        private System.Windows.Forms.Button Button_PZero;
        private System.Windows.Forms.Button Button_SetOut4mA;
        private System.Windows.Forms.Button Button_AReset;
        private System.Windows.Forms.Button Button_PReset;
    }
}

