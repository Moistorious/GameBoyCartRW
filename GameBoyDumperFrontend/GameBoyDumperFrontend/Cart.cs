using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace GameBoyDumperFrontend
{
    public class Cart
    {

        public string Name;
        public int RamSize;
        public int FullRomSize;
        public event EventHandler OnDataRead;

        const ushort HeaderAddress = 0x100;
        const ushort BankSize = 0x4000;
        const byte HeaderSize = 0x4f;
        ISerial _serial;
        byte _cartridgeType;
        ushort _romSize;
        ushort _romBanks;
        ushort _ramSize;
        byte _ramBanks;
        ushort _ramBankSize;
        int _ramLength;
        ushort _address;
        ushort _bytesRead;

        public Cart(ISerial serial)
        {
            _serial = serial;
        }

        byte[] nintendoLogo = {0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
                               0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
                               0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E};
        // Nintendo logo located at 0x104-0x133
        public bool SanityCheck()
        {
            SerialCommand command = new SerialCommand
            {
                write = false,
                address = 0x104,
                length = 0x133 - 0x104
            };
            var logo = _serial.ReadBytes(0x104, (byte)nintendoLogo.Count());
            for (int i = 0; i < logo.Length; i++)
            {
                if (logo[i] != nintendoLogo[i])
                {
                    return false;
                }
            }
            return true;
        }

        void SelectBankOld(byte cartType, byte bank)
        {
            if (cartType >= 5)
            {                          // MBC2 and above
                _serial.WriteByte(0x2100, bank); // Set ROM bank
            }
            else
            {
                _serial.WriteByte(0x6000, 0);           // Set ROM Mode
                _serial.WriteByte(0x4000, (byte)(bank >> 5));   // Set bits 5 & 6 (01100000) of ROM bank
                _serial.WriteByte(0x2000, (byte)(bank & 0x1F)); // Set bits 0 & 4 (00011111) of ROM bank
            }
        }
        void SelectBank(byte cartType, byte bank)
        {
            _serial.SelectBank(cartType, bank);
        }

        public byte[] GetHeader()
        {
            return _serial.ReadBytes(HeaderAddress, HeaderSize);
        }

        public void EnableRAM()
        {
            byte temp = _serial.ReadByte(0x134); // Hack? needed?
            if (_cartridgeType <= 4)
            {
                _serial.WriteByte(0x6000, 1); // RAM mode
            }

            _serial.WriteByte(0x0000, 0x0A); // MBC?

            byte[] bytes = new byte[RamSize];

        }

        public void DisableRAM()
        {
            _serial.WriteByte(0x0000, 0x00);
        }

        public void ReadRange(ushort address, ushort length, int offset, ref byte[] buffer)
        {
            ushort bytesRead = 0; 

            while (bytesRead < length)
            {
                byte bytesToRead = (byte)Math.Min(255, length - bytesRead);
                int bufferOffset = offset + (int)bytesRead;

                _serial.ReadBytes((ushort)(address + bytesRead), bytesToRead).CopyTo(buffer, bufferOffset);

                OnDataRead?.Invoke(this, new CartDataReadEventArgs() { totalBytesRead = bytesToRead });

                bytesRead += bytesToRead;
            }
        }

        public byte[] GetROM()
        {

            //byte[] RomSizeBytes = BitConverter.GetBytes(RomSize);
            byte[] bytes = new byte[FullRomSize];
            ReadRange(0, BankSize, 0, ref bytes);

            for (byte bank = 1; bank < _romBanks; bank++)
            {
                SelectBank(_cartridgeType, bank);

                ReadRange((ushort)0x4000, BankSize, ((int)BankSize) * bank, ref bytes);
            }
            return bytes;
        }

        public void WriteBytes(ushort address, byte[] buffer, bool RAM = false)
        {
            ushort bytesWritten = 0;

            while (bytesWritten < buffer.Length)
            {
                byte bytesToWrite = (byte)Math.Min(255, buffer.Length- bytesWritten);
                byte[] segment = buffer.Skip(bytesWritten).Take(bytesToWrite).ToArray();

                _serial.WriteBytes((ushort)(address + bytesWritten),segment, RAM);

                OnDataRead?.Invoke(this, new CartDataReadEventArgs() { totalBytesRead = bytesToWrite });

                bytesWritten += bytesToWrite;
            }
        }

        public void Reset()
        {
            _serial.Reset();
        }

        public void WriteRAM(byte[] buffer)
        {
            if (_ramBankSize == 0)
            {
                return;
            }
            EnableRAM();

            for (byte currentBank = 0; currentBank < _ramBanks; currentBank++)
            {
                _serial.WriteByte(0x4000, currentBank);
                byte[] bankData = buffer.Skip(currentBank * _ramBankSize).Take(_ramBankSize).ToArray();
                WriteBytes(0xA000, bankData, true);
            }
            DisableRAM();
            
        }

        public byte[] GetRAM()
        {
            if (_ramBankSize == 0) { 
                return new byte[0];
            }

            byte[] bytes = new byte[RamSize];
            EnableRAM();
            for(byte currentBank = 0; currentBank < _ramBanks; currentBank++) {
                _serial.WriteByte(0x4000, currentBank);
                ReadRange(0xA000, _ramBankSize, _ramBankSize * currentBank, ref bytes);
            }
            DisableRAM();
            return bytes;
        }

        public void init()
        {
            _cartridgeType = _serial.ReadByte(0x0147);
            _romSize = _serial.ReadByte(0x0148);
            _romBanks = 0;
            _ramSize = _serial.ReadByte(0x0149);
            _ramBanks = 0;
            _ramBankSize = 0;
            _romBanks = 2;
            byte[] nameBytes = new byte[15];
            _serial.ReadBytes((ushort)(0x0134), 15).CopyTo(nameBytes, 0);
            Name = Encoding.UTF8.GetString(nameBytes).Trim('\0');

            if (_romSize > 0)
            {
                _romBanks = (ushort)(2 << _romSize);
            }

            // RAM banks
            _ramBanks = 0; // Default 0K RAM
            if (_cartridgeType == 6)
            {
                _ramBanks = 1;
            }
            switch (_ramSize)
            {
                case 2:
                    _ramBanks = 1;
                    break;
                case 3:
                    _ramBanks = 4;
                    break;
                case 4:
                    _ramBanks = 16;
                    break;
                case 5:
                    _ramBanks = 8;
                    break;
            }

            // RAM end address
            if (_cartridgeType == 6)
            {
                _ramBankSize = 0x200;
            } // MBC2 512bytes (nibbles)
            if (_ramSize == 1)
            {
                _ramBankSize = 0x800;
            } // 2K RAM
            if (_ramSize > 1)
            {
                _ramBankSize = 0x2000;
            } // 8K RAM

            FullRomSize = _romBanks * BankSize;
            RamSize = _ramBankSize * _ramBanks;
        }

    }
    class CartDataReadEventArgs : EventArgs
    {
        public uint totalBytesRead = 0;
    }
}
