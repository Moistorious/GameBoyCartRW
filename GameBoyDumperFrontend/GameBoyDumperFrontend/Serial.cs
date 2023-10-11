using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyDumperFrontend
{
    public class Serial : ISerial
    {
        public SerialPort _serialPort;
        public Serial(string port, int baudRate = 9600)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = port;
            _serialPort.BaudRate = baudRate;
            //_serialPort.Parity = SetPortParity(_serialPort.Parity);
            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            //_serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();
        }


        public byte ReadByte(UInt16 address)
        {
            return ReadBytes(address, 1)[0];
        }

        public void WriteByte(UInt16 address, byte value)
        {
            WriteBytes(address, new byte[] { value });
        }

        public void WriteBytes(UInt16 address, byte[] data)
        {
            byte[] output = new byte[4];
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                write = true,
                address = address,
                length = (byte)data.Length,
                data = data
            });
            // When the write is complete, we will receive "Done" over serial.
            while (totalBytesRead < 4)
            {
                totalBytesRead += _serialPort.Read(output, totalBytesRead, _serialPort.BytesToRead);
            }
        }

        public byte[] ReadBytes(UInt16 address, byte length)
        {
            byte[] output = new byte[length];
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                write = false,
                address = address,
                length = length
            });


            while (totalBytesRead < length)
            {
                totalBytesRead += _serialPort.Read(output, totalBytesRead, _serialPort.BytesToRead);
            }
            return output;
        }

        private void SendCommand(SerialCommand command)
        {
            byte[] serialCommand = command.ToBytes();
            _serialPort.Write(serialCommand, 0, serialCommand.Length);
        }

        // Display Port values and prompt user to enter a port.
        public string SetPortName(string defaultPortName)
        {
            string portName = "";

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }

        // Display BaudRate values and prompt user to enter a value.
        public int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate = "";

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            //baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        // Display PortParity values and prompt user to enter a value.
        public Parity SetPortParity(Parity defaultPortParity)
        {
            string parity = "";

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            //parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
        // Display DataBits values and prompt user to enter a value.
        public int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits = "";

            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
            //dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        // Display StopBits values and prompt user to enter a value.
        public StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits = "";

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            //stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }
        public Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake = "";

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
            //handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }

        public void SelectBank(byte cartType, byte bank)
        {
            SendCommand(new SerialCommand
            {
                write = true,
                address = 0xffff,
                length = 2,
                data = new byte[] { cartType, bank }
            });
        }
    }
}
