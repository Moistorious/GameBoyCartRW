
using GameBoyDumperFrontend;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

public class Program
{
    static ProgressBar progress = null;
    static uint totalBytesRead = 0;
    //Thread readThread = new Thread(Read);
    //readThread.Start();

    public static void Main()
    {
        Cart cart = new Cart(new Serial("COM3"));
        //Cart cart = new Cart(new FakeSerial("C:\\Users\\Tiama\\Downloads\\Battle Unit Zeoth (Japan)\\Battle Unit Zeoth (Japan).gb"));
        if (!cart.SanityCheck())
        {
            Console.WriteLine("Cart data invalid, try reseating the cart?");
            return;
        }
        else
        {
            Console.WriteLine("Yay!");
        }
        cart.init();

        progress = new ProgressBar(cart.FullRomSize);
        cart.OnDataRead += Cart_OnDataRead;
        //cart.Test();
        var romData = cart.GetROM();



        //File.WriteAllBytes("C:\\Users\\Tiama\\OneDrive\\Desktop\\poeprograms\\Header", cart.GetHeader());
        File.WriteAllBytes("C:\\Users\\Tiama\\OneDrive\\Desktop\\poeprograms\\ROM4", romData);

    }

    private static void Cart_OnDataRead(object? sender, EventArgs e)
    {
        totalBytesRead += ((CartDataReadEventArgs)e).totalBytesRead;
        progress.Report(totalBytesRead);
    }

    /*
        public static void Read()
        {
            int numberRead = 0;
            UInt32 expectedLength = 0;
            while (_continue)
            {
                try
                {
                    if (State == States.WaitingForAck && !_command.write && _serialPort.BytesToRead >= _command.length)
                    {
                        int bytesRead = _serialPort.Read(_buffer, 0, _serialPort.BytesToRead);
                        //Console.Write(Encoding.ASCII.GetString(_buffer, 0, bytesRead));
                        string bitString = BitConverter.ToString(_buffer,0, bytesRead);
                        Console.Write(bitString);
                        State = States.Init;
                    }
                    /*
                    if (State == States.WaitingForAck )
                    {//https://www.youtube.com/watch?v=2vpTYaDGmCs
                        if (_serialPort.BytesToRead < 4) { continue; }
                        int bytesread = _serialPort.Read(_buffer, 0, 4);
                        expectedLength = 0;
                        expectedLength = (UInt32)_buffer[3] << 24 | (UInt32)_buffer[2] << 16 | (UInt32)_buffer[1] << 8 | (UInt32)_buffer[0];
                        State = States.ReadingData;
                        Console.WriteLine($"Expecting {expectedLength} Bytes");

                        if (expectedLength != 79 && act == Operations.Header)
                        {
                            throw new Exception("Bad length");
                        }
                        progress = new ProgressBar((double)expectedLength);
                    } else if (State == States.ReadingData && expectedLength > 0)
                    {
                        if (_data.Count < expectedLength) { 
                            numberRead = _serialPort.Read(_buffer, 0, 1024);
                            for (int i = 0; i < numberRead; i++)
                            {
                                _data.Add(_buffer[i]);
                            }
                            progress.Report(_data.Count);
                        }
                        else
                        {
                            State = States.DataRead;
                            progress.Dispose();
                        }
                    }*/
    //}
    //  catch (TimeoutException) { }
    //}
}