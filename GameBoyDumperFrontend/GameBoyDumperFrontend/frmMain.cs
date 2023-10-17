using System.ComponentModel;
using System.IO.Ports;

namespace GameBoyDumperFrontend
{
    public partial class frmMain : Form
    {
        Cart? cart;
        CartSerial serial;
        BackgroundWorker worker;

        public frmMain()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            serial = new CartSerial();

            saveFileDialog1.ValidateNames = true;
            saveFileDialog1.OverwritePrompt = true;

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

            //MessageBox.Show("reset", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //cart.Reset();
            if (e.Error != null)
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
            var args = e.Argument as workerArgs;
            BackgroundWorker? worker = sender as BackgroundWorker;

            int totalBytesRead = 0;
            int targetBytes = 0;

            if (args == null || worker == null) { throw new ArgumentException("We need an operation to perform!"); }

            if (!cart.SanityCheck()) { throw new Exception("It's broken"); }

            var onDataProcessed = new EventHandler((object? sender, EventArgs e) =>
            {
                totalBytesRead += ((CartSerial.CartDataEventArgs)e).ProcessedBytes;
                worker.ReportProgress((int)Math.Floor(((double)totalBytesRead / targetBytes) * 100));
            });

            serial.OnDataWritten += onDataProcessed;
            serial.OnDataRead += onDataProcessed;

            totalBytesRead = 0;
            switch (args.targetOperation)
            {
                case workerArgs.operation.readRam:
                    targetBytes = cart.RamSize;
                    File.WriteAllBytes(args.filename, cart.GetRAM());
                    break;

                case workerArgs.operation.writeRam:
                    var ramBytes = File.ReadAllBytes(args.filename);
                    targetBytes = ramBytes.Length;
                    cart.WriteRAM(ramBytes);
                    break;

                case workerArgs.operation.readRom:
                    targetBytes = cart.FullRomSize;
                    File.WriteAllBytes(args.filename, cart.GetROM());
                    break;
            }
            serial.OnDataWritten -= onDataProcessed;
            serial.OnDataRead -= onDataProcessed;
        }

        private void btn_WriteRam_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = $"{cart.Header.Title}.sav";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                toggleButtons(false);

                worker.RunWorkerAsync(new workerArgs
                {
                    targetOperation = workerArgs.operation.writeRam,
                    filename = openFileDialog1.FileName
                });
            }
        }

        private void btn_ReadRom_Click(object sender, EventArgs e)
        {
            var extension = String.Empty;

            switch (cart.GbcType)
            {
                case Cart.CartType.GBC:
                case Cart.CartType.GBC_Backwards_Compatible:
                    extension = "gbc";
                    break;
                default:
                    extension = "gb";
                    break;
            }

            saveFileDialog1.FileName = $"{cart.Header.Title}.{extension}";

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

            saveFileDialog1.FileName = $"{cart.Header.Title}.sav";

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


        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            // Don't display COM 1
            cboComPort.DataSource = (SerialPort.GetPortNames().Skip(1)).ToList();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btn_Refresh_Click(sender, e);
            cboBaud.SelectedIndex = 0;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            cart.Reset();
            txtCartName.Text = String.Empty;
            serial.CloseConnection();

            btn_Refresh.Enabled = true;
            cboBaud.Enabled = true;
            cboComPort.Enabled = true;
            btn_Close.Enabled = false;
            btn_Init.Enabled = true;

            toggleButtons(false);
        }
        private void btn_Init_Click(object sender, EventArgs e)
        {
            serial.SetPortName(cboComPort.Text);
            serial.SetBaudRate(int.Parse(cboBaud.Text));
            serial.OpenConnection();

            cart = new Cart(serial);

            cart.init();

            txtCartName.Text = cart.Header.Title;

            btn_Refresh.Enabled = false;
            cboBaud.Enabled = false;
            cboComPort.Enabled = false;
            btn_Close.Enabled = true;
            btn_Init.Enabled = false;

            toggleButtons(true);

        }
    }
}
