namespace GameBoyDumperFrontend
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_ReadRam = new Button();
            btn_ReadRom = new Button();
            btn_WriteRam = new Button();
            progressBar1 = new ProgressBar();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            btn_Init = new Button();
            txtCartName = new TextBox();
            cboComPort = new ComboBox();
            btn_Refresh = new Button();
            btn_Close = new Button();
            cboBaud = new ComboBox();
            SuspendLayout();
            // 
            // btn_ReadRam
            // 
            btn_ReadRam.Enabled = false;
            btn_ReadRam.Location = new Point(59, 176);
            btn_ReadRam.Name = "btn_ReadRam";
            btn_ReadRam.Size = new Size(94, 29);
            btn_ReadRam.TabIndex = 0;
            btn_ReadRam.Text = "Read RAM";
            btn_ReadRam.UseVisualStyleBackColor = true;
            btn_ReadRam.Click += btn_ReadRam_Click;
            // 
            // btn_ReadRom
            // 
            btn_ReadRom.Enabled = false;
            btn_ReadRom.Location = new Point(310, 176);
            btn_ReadRom.Name = "btn_ReadRom";
            btn_ReadRom.Size = new Size(94, 29);
            btn_ReadRom.TabIndex = 1;
            btn_ReadRom.Text = "Read ROM";
            btn_ReadRom.UseVisualStyleBackColor = true;
            btn_ReadRom.Click += btn_ReadRom_Click;
            // 
            // btn_WriteRam
            // 
            btn_WriteRam.Enabled = false;
            btn_WriteRam.Location = new Point(184, 176);
            btn_WriteRam.Name = "btn_WriteRam";
            btn_WriteRam.Size = new Size(94, 29);
            btn_WriteRam.TabIndex = 2;
            btn_WriteRam.Text = "Write RAM";
            btn_WriteRam.UseVisualStyleBackColor = true;
            btn_WriteRam.Click += btn_WriteRam_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(59, 93);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(344, 29);
            progressBar1.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "sav";
            openFileDialog1.Filter = "SaveFiles|*.sav|All files|*.*";
            // 
            // btn_Init
            // 
            btn_Init.Location = new Point(355, 129);
            btn_Init.Margin = new Padding(3, 4, 3, 4);
            btn_Init.Name = "btn_Init";
            btn_Init.Size = new Size(86, 31);
            btn_Init.TabIndex = 4;
            btn_Init.Text = "Init Cart";
            btn_Init.UseVisualStyleBackColor = true;
            btn_Init.Click += btn_Init_Click;
            // 
            // txtCartName
            // 
            txtCartName.Location = new Point(59, 43);
            txtCartName.Margin = new Padding(3, 4, 3, 4);
            txtCartName.Name = "txtCartName";
            txtCartName.ReadOnly = true;
            txtCartName.Size = new Size(343, 27);
            txtCartName.TabIndex = 5;
            txtCartName.TextAlign = HorizontalAlignment.Center;
            // 
            // cboComPort
            // 
            cboComPort.FormattingEnabled = true;
            cboComPort.Location = new Point(59, 129);
            cboComPort.Margin = new Padding(3, 4, 3, 4);
            cboComPort.Name = "cboComPort";
            cboComPort.Size = new Size(138, 28);
            cboComPort.TabIndex = 6;
            // 
            // btn_Refresh
            // 
            btn_Refresh.Location = new Point(205, 129);
            btn_Refresh.Margin = new Padding(3, 4, 3, 4);
            btn_Refresh.Name = "btn_Refresh";
            btn_Refresh.Size = new Size(24, 31);
            btn_Refresh.TabIndex = 7;
            btn_Refresh.Text = "⟳";
            btn_Refresh.UseVisualStyleBackColor = true;
            btn_Refresh.Click += btn_Refresh_Click;
            // 
            // btn_Close
            // 
            btn_Close.Enabled = false;
            btn_Close.Location = new Point(448, 129);
            btn_Close.Margin = new Padding(3, 4, 3, 4);
            btn_Close.Name = "btn_Close";
            btn_Close.Size = new Size(86, 31);
            btn_Close.TabIndex = 8;
            btn_Close.Text = "Close";
            btn_Close.UseVisualStyleBackColor = true;
            btn_Close.Click += btn_Close_Click;
            // 
            // cboBaud
            // 
            cboBaud.FormattingEnabled = true;
            cboBaud.Items.AddRange(new object[] { "9600", "19200", "115200" });
            cboBaud.Location = new Point(235, 131);
            cboBaud.Margin = new Padding(3, 4, 3, 4);
            cboBaud.Name = "cboBaud";
            cboBaud.Size = new Size(90, 28);
            cboBaud.TabIndex = 9;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(791, 345);
            Controls.Add(cboBaud);
            Controls.Add(btn_Close);
            Controls.Add(btn_Refresh);
            Controls.Add(cboComPort);
            Controls.Add(txtCartName);
            Controls.Add(btn_Init);
            Controls.Add(progressBar1);
            Controls.Add(btn_WriteRam);
            Controls.Add(btn_ReadRom);
            Controls.Add(btn_ReadRam);
            Name = "frmMain";
            Text = "Moist Cart Read";
            Load += frmMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_ReadRam;
        private Button btn_ReadRom;
        private Button btn_WriteRam;
        private ProgressBar progressBar1;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private Button btn_Init;
        private TextBox txtCartName;
        private ComboBox cboComPort;
        private Button btn_Refresh;
        private Button btn_Close;
        private ComboBox cboBaud;
    }
}