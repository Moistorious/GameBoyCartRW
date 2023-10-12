using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyDumperFrontend
{
    public class CartSerial : ICartridgeCommunication
    {
        public SerialPort _serialPort;

        public CartSerial()
        {
            _serialPort = new SerialPort();
        }

        private void ThrowIfOpen()
        {
            if (_serialPort.IsOpen)
            {
                throw new InvalidOperationException("Can't change port settings while open.");
            }
        }

        private void ThrowIfClosed()
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("Can't perform I/O on a closed port.");
            }
        }

        public void SetPortName(string portName)
        {
            ThrowIfOpen();
            _serialPort.PortName = portName;
        }

        public void OpenConnection() { ThrowIfOpen(); _serialPort.Open(); }
        public void CloseConnection() { _serialPort.Close(); }
        public void SetBaudRate(int baudRate) { ThrowIfOpen(); _serialPort.BaudRate = baudRate; }
        public void SetParity(Parity parity) { ThrowIfOpen(); _serialPort.Parity = parity; }
        public void SetDataBits(int dataBits) { ThrowIfOpen(); _serialPort.DataBits = dataBits; }
        public void SetStopBits(StopBits defaultPortStopBits) { ThrowIfOpen(); _serialPort.StopBits = defaultPortStopBits; }
        public void SetHandshake(Handshake defaultPortHandshake) { ThrowIfOpen(); _serialPort.Handshake = defaultPortHandshake; }
        public void SetReadTimeout(int timeout) { ThrowIfOpen(); _serialPort.ReadTimeout = timeout; }
        public void SetWriteTimeout(int timeout) { ThrowIfOpen(); _serialPort.WriteTimeout = timeout; }
        public byte ReadByte(UInt16 address) { return ReadBytes(address, 1)[0]; }
        public void WriteByte(UInt16 address, byte value) { WriteBytes(address, new byte[] { value }); }

        private void SendCommand(SerialCommand command)
        {
            ThrowIfClosed();
            byte[] serialCommand = command.ToBytes();
            _serialPort.Write(serialCommand, 0, serialCommand.Length);
        }

        public void WriteBytes(UInt16 address, byte[] data, bool RAM = false)
        {
            ThrowIfClosed();
            byte[] output = new byte[4];
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                write = true,
                writeRam = RAM,
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
            ThrowIfClosed();
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

        public void Reset()
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                write = false,
                reset = true,
                address = 0,
                length = 0
            });
        }

        public void SelectBank(byte cartType, byte bank)
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                write = false,
                selectBank = true,
                address = 0,
                length = 2,
                data = new byte[] { cartType, bank }
            });
        }
    }
}
