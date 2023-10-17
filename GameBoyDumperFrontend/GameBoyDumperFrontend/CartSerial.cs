using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GameBoyDumperFrontend.CartClasses;
using GameBoyDumperFrontend.Interface;

namespace GameBoyDumperFrontend
{
    public class CartSerial : ICartridgeCommunication
    {
        public SerialPort _serialPort;
        public event EventHandler? OnDataRead;
        public event EventHandler? OnDataWritten;

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
            int bytesWritten = 0;

            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.WriteRange,
                address = address,
                length = (byte)data.Length,
            });

            for (int bytesWriten = 0; bytesWriten < data.Length;)
            {

                byte bytesToWrite = (byte)Math.Min(255, data.Length);

                _serialPort.Write(data, bytesWritten, bytesToWrite);
                OnDataWritten?.Invoke(this, new CartDataEventArgs() { ProcessedBytes = bytesToWrite });
                bytesWriten += bytesToWrite;
            }

            // When the write is complete, we will receive "Done" over serial.
            while (totalBytesRead < 4)
            {
                totalBytesRead += ReadBytes(output, totalBytesRead, _serialPort.BytesToRead);
            }
        }


        public byte[] ReadBytes(UInt16 address, byte length)
        {
            ThrowIfClosed();
            byte[] output = new byte[length];
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.ReadRange,
                address = address,
                length = length
            });

            while (totalBytesRead < length)
            {
                totalBytesRead += ReadBytes(output, totalBytesRead, _serialPort.BytesToRead);
            }
            return output;
        }

        public void GetROM(byte[] buffer)
        {
            ThrowIfClosed();
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.GetFullROM
            });

            while (totalBytesRead < buffer.Length)
            {
                totalBytesRead += ReadBytes(buffer, totalBytesRead, _serialPort.BytesToRead);
            }
        }

        public void GetRAM(byte[] buffer)
        {
            ThrowIfClosed();
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.GetFullRAM
            });

            while (totalBytesRead < buffer.Length)
            {
                totalBytesRead += ReadBytes(buffer, totalBytesRead, _serialPort.BytesToRead);
            }
        }

        public void WriteRAM(byte[] buffer)
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.WriteFullRAM
            });

            int totalBytesRead = 0;
            
            for (int bytesWritten = 0; bytesWritten < buffer.Length;)
            {
                // We'll allow the reader to request bytes, because otherwise we will overflow its receive buffer.
                byte bytesToWrite = (byte)_serialPort.ReadByte();
                _serialPort.Write(buffer, bytesWritten, bytesToWrite);
                OnDataWritten?.Invoke(this, new CartDataEventArgs() { ProcessedBytes = bytesToWrite });
                bytesWritten += bytesToWrite;
            }
            totalBytesRead = 0;

            // Reader sends "DONE" when finished.
            while (totalBytesRead < 4)
            {
                totalBytesRead += ReadBytes(buffer, totalBytesRead, _serialPort.BytesToRead);
            }
        }

        private int ReadBytes(byte[] buffer, int offset, int count)
        {
            int bytesRead = _serialPort.Read(buffer, offset, _serialPort.BytesToRead);
            OnDataRead?.Invoke(this, new CartDataEventArgs() { ProcessedBytes = bytesRead });
            return bytesRead;
        }

        public byte[] GetHeader()
        {
            ThrowIfClosed();
            byte[] output = new byte[Marshal.SizeOf<CartHeader>()];
            int totalBytesRead = 0;
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.GetHeader,
                address = 0x00,
                length = 0x00
            });

            while (totalBytesRead < output.Length)
            {
                totalBytesRead += ReadBytes(output, totalBytesRead, _serialPort.BytesToRead);
            }
            return output;
        }

        public void Init()
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.InitializeCart
            });
        }

        public void Reset()
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.Reset,
                address = 0,
                length = 0
            });
        }

        public void SelectBank(byte cartType, byte bank)
        {
            ThrowIfClosed();
            SendCommand(new SerialCommand
            {
                Command = SerialCommand.CommandType.SelectBank,
                address = 0,
                length = 2,
                data = new byte[] { cartType, bank }
            });
        }

        public class CartDataEventArgs : EventArgs
        {
            public int ProcessedBytes = 0;
        }
    }
}
