
using GameBoyDumperFrontend;

public class Program
{
    [STAThread]
    public static void Main()
    {

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmMain());
    }


}