using System.ComponentModel;

namespace GameBoyDumperFrontend
{
    public partial class Form1 : Form
    {
        static Cart cart;
        BackgroundWorker worker;

        public Form1()
        {
            InitializeComponent();
            worker = new BackgroundWorker();

            saveFileDialog1.ValidateNames = true;
            saveFileDialog1.OverwritePrompt = true;

            cart = new Cart(new Serial("COM3"));

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            cart.Reset();
            if(e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            toggleButtons(true);
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void toggleButtons(bool enabled)
        {
            btn_ReadRam.Enabled = enabled;
            btn_ReadRom.Enabled = enabled;
            btn_WriteRam.Enabled = enabled;
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            uint totalBytesRead = 0;
            int targetBytes = 0;

            if (sender == null) { return; }

            BackgroundWorker worker = (BackgroundWorker)sender;

            if (!SanityCheck())
            {
                throw new Exception("It's broken");
            }
            var args = (workerArgs)e.Argument;

            void startProgress(int target)
            {
                cart.OnDataRead += Cart_OnDataRead;
                totalBytesRead = 0;
                targetBytes = target;
            }

            void stopProgress()
            {
                cart.OnDataRead -= Cart_OnDataRead;
            }

            void Cart_OnDataRead(object? sender, EventArgs e)
            {
                totalBytesRead += ((CartDataReadEventArgs)e).totalBytesRead;

                worker.ReportProgress((int)Math.Floor(((double)totalBytesRead / targetBytes) * 100));
            }

            cart.init();
            switch (args.targetOperation)
            {
                case workerArgs.operation.readRam:
                    startProgress(cart.RamSize);
                    File.WriteAllBytes(args.filename, cart.GetRAM());
                    stopProgress();
                    break;

                case workerArgs.operation.writeRam:
                    var ramBytes = File.ReadAllBytes(args.filename);
                    startProgress(ramBytes.Length);
                    cart.WriteRAM(ramBytes);
                    stopProgress();
                    break;

                case workerArgs.operation.readRom:
                    startProgress(cart.FullRomSize);
                    File.WriteAllBytes(args.filename, cart.GetROM());
                    stopProgress();
                    break;
            }

        }

        private bool SanityCheck()
        {
            return cart != null && cart.SanityCheck();
        }

        private void btn_WriteRam_Click(object sender, EventArgs e)
        {
            toggleButtons(false);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                worker.RunWorkerAsync(new workerArgs
                {
                    targetOperation = workerArgs.operation.writeRam,
                    filename = openFileDialog1.FileName
                });
            }
        }

        private void btn_ReadRom_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toggleButtons(false);
                worker.RunWorkerAsync(new workerArgs
                {
                    targetOperation = workerArgs.operation.readRom,
                    filename = saveFileDialog1.FileName
                });
            }
        }

        private void btn_ReadRam_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toggleButtons(false);
                worker.RunWorkerAsync(new workerArgs
                {
                    targetOperation = workerArgs.operation.readRam,
                    filename = saveFileDialog1.FileName
                });
            }
        }

        class workerArgs
        {
            public enum operation
            {
                readRom,
                readRam,
                writeRam
            }
            public operation targetOperation;
            public string filename = "";
        }
    }
}
