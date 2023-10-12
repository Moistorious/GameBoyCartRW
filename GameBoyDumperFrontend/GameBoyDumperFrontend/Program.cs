
using GameBoyDumperFrontend;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

public class Program
{
    //Thread readThread = new Thread(Read);
    //readThread.Start();
    [STAThread]
    public static void Main()
    {

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());

        //WriteRam("C:\\Users\\Tiama\\OneDrive\\Desktop\\Gameboy\\dev\\POKEMON YELLOW.sav");
        //ReadRam($"C:\\Users\\Tiama\\OneDrive\\Desktop\\poeprograms\\{cart.Name}.sav");
        //ReadRom($"C:\\Users\\Tiama\\OneDrive\\Desktop\\poeprograms\\{cart.Name}.gb");
        //File.WriteAllBytes($"C:\\Users\\Tiama\\OneDrive\\Desktop\\poeprograms\\{cart.Name}.sav", ramData);
    }


}