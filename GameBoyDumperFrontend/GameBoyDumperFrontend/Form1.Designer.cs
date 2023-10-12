namespace GameBoyDumperFrontend
{
    partial class Form1
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
            progressBar1 = new System.Windows.Forms.ProgressBar();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            SuspendLayout();
            // 
            // btn_ReadRam
            // 
            btn_ReadRam.Location = new Point(52, 169);
            btn_ReadRam.Margin = new Padding(3, 2, 3, 2);
            btn_ReadRam.Name = "btn_ReadRam";
            btn_ReadRam.Size = new Size(82, 22);
            btn_ReadRam.TabIndex = 0;
            btn_ReadRam.Text = "Read RAM";
            btn_ReadRam.UseVisualStyleBackColor = true;
            btn_ReadRam.Click += btn_ReadRam_Click;
            // 
            // btn_ReadRom
            // 
            btn_ReadRom.Location = new Point(211, 169);
            btn_ReadRom.Margin = new Padding(3, 2, 3, 2);
            btn_ReadRom.Name = "btn_ReadRom";
            btn_ReadRom.Size = new Size(82, 22);
            btn_ReadRom.TabIndex = 1;
            btn_ReadRom.Text = "Read ROM";
            btn_ReadRom.UseVisualStyleBackColor = true;
            btn_ReadRom.Click += btn_ReadRom_Click;
            // 
            // btn_WriteRam
            // 
            btn_WriteRam.Location = new Point(52, 195);
            btn_WriteRam.Margin = new Padding(3, 2, 3, 2);
            btn_WriteRam.Name = "btn_WriteRam";
            btn_WriteRam.Size = new Size(82, 22);
            btn_WriteRam.TabIndex = 2;
            btn_WriteRam.Text = "Write RAM";
            btn_WriteRam.UseVisualStyleBackColor = true;
            btn_WriteRam.Click += btn_WriteRam_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(52, 70);
            progressBar1.Margin = new Padding(3, 2, 3, 2);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(301, 22);
            progressBar1.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(progressBar1);
            Controls.Add(btn_WriteRam);
            Controls.Add(btn_ReadRom);
            Controls.Add(btn_ReadRam);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_ReadRam;
        private Button btn_ReadRom;
        private Button btn_WriteRam;
        private System.Windows.Forms.ProgressBar progressBar1;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
    }
}