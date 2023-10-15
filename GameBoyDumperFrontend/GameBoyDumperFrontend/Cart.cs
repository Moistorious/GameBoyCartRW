using GameBoyDumperFrontend.CartClasses;
using GameBoyDumperFrontend.Interface;
using System;
using System.Runtime.InteropServices;

namespace GameBoyDumperFrontend
{
    public class Cart
    {

        const ushort BankSize = 0x4000;
        public enum CartType
        {
            GB,
            GBC_Backwards_Compatible,
            GBC
        }
        public ushort RomBanks;
        public byte RamBanks;
        public ushort RamBankSize;

        public CartHeader Header;
        public CartType GbcType;
        public int RamSize;
        public int FullRomSize;

        ICartridgeCommunication _cartCommunication;

        private byte[]? _headerBytes;

        public Cart(ICartridgeCommunication cartCommunication)
        {
            _cartCommunication = cartCommunication;
        }

        byte[] nintendoLogo = {0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
                               0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
                               0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E};

        public bool SanityCheck()
        {
            for (int i = 0; i < Header.NintendoLogo.Length; i++)
            {
                if (Header.NintendoLogo[i] != nintendoLogo[i])
                {
                    return false;
                }
            }
            return true;
        }

        public CartHeader GetHeader()
        {
            var headerSize = (UInt16)Marshal.SizeOf(typeof(CartHeader));
            _headerBytes = _cartCommunication.GetHeader();

            //ReadRange(HeaderAddress, headerSize, 0, ref _headerBytes);

            return BinarySerialization.FromByteArray<CartHeader>(_headerBytes);
        }

        public void EnableRAM()
        {
            byte temp = _cartCommunication.ReadByte(0x134); // Hack? needed?
            if (Header.CartridgeType <= 4)
            {
                _cartCommunication.WriteByte(0x6000, 1); // RAM mode
            }

            _cartCommunication.WriteByte(0x0000, 0x0A); // MBC?

            byte[] bytes = new byte[RamSize];

        }

        public void DisableRAM()
        {
            _cartCommunication.WriteByte(0x0000, 0x00);
        }

        public void ReadRange(ushort address, ushort length, int offset, ref byte[] buffer)
        {
            ushort bytesRead = 0;

            while (bytesRead < length)
            {
                byte bytesToRead = (byte)Math.Min(255, length - bytesRead);
                int bufferOffset = offset + (int)bytesRead;

                _cartCommunication.ReadBytes((ushort)(address + bytesRead), bytesToRead).CopyTo(buffer, bufferOffset);

                bytesRead += bytesToRead;
            }
        }

        public byte[] GetROM()
        {

            //byte[] RomSizeBytes = BitConverter.GetBytes(RomSize);
            byte[] bytes = new byte[FullRomSize];
            _cartCommunication.GetROM(bytes);
            /*ReadRange(0, BankSize, 0, ref bytes);

            for (byte bank = 1; bank < RomBanks; bank++)
            {
                _cartCommunication.SelectBank(Header.CartridgeType, bank);

                ReadRange((ushort)0x4000, BankSize, ((int)BankSize) * bank, ref bytes);
            }*/
            return bytes;
        }

        public void WriteBytes(ushort address, byte[] buffer, bool RAM = false)
        {
            ushort bytesWritten = 0;

            while (bytesWritten < buffer.Length)
            {
                byte bytesToWrite = (byte)Math.Min(255, buffer.Length - bytesWritten);
                byte[] segment = buffer.Skip(bytesWritten).Take(bytesToWrite).ToArray();

                _cartCommunication.WriteBytes((ushort)(address + bytesWritten), segment, RAM);

                bytesWritten += bytesToWrite;
            }
        }

        public void Reset()
        {
            _cartCommunication.Reset();
        }

        public void WriteRAM(byte[] buffer)
        {
            if (RamBankSize == 0)
            {
                return;
            }
            EnableRAM();

            for (byte currentBank = 0; currentBank < RamBanks; currentBank++)
            {
                _cartCommunication.WriteByte(0x4000, currentBank);
                byte[] bankData = buffer.Skip(currentBank * RamBankSize).Take(RamBankSize).ToArray();
                WriteBytes(0xA000, bankData, true);
            }
            DisableRAM();

        }

        public byte[] GetRAM()
        {
            if (RamBankSize == 0)
            {
                return new byte[0];
            }

            byte[] bytes = new byte[RamSize];
            _cartCommunication.GetRAM(bytes);
            
            return bytes;
        }

        public void init()
        {
            _cartCommunication.Init();
            RomBanks = 0; RamBanks = 0; RamBankSize = 0; RomBanks = 0;

            Header = GetHeader();
            if (_headerBytes == null)
            {
                throw new ApplicationException("Header Bytes is null!");
            }

            // header starts at 0x100, so to get 0x143, we'll just read it 0x43
            GbcType = GetCartType(_headerBytes[0x43]);
            
            RamBanks = GetRamBanks(Header.RamSize, Header.CartridgeType);
            RamBankSize = GetRamBankSize(Header.RamSize, Header.CartridgeType);
            RomBanks = GetRomBanks(Header.RomSize);

            FullRomSize = RomBanks * BankSize;
            RamSize = RamBankSize * RamBanks;
        }

        private CartType GetCartType(byte gbcType)
        {
            switch (gbcType)
            {
                case 0x80:
                    return CartType.GBC_Backwards_Compatible;
                case 0xC0:
                    return CartType.GBC;
                default:
                    return CartType.GB;
            }
        }

        private ushort GetRamBankSize(byte ramSize, byte cartridgeType)
        {
            ushort ramBankSize = 0;
            if (cartridgeType == 6)
            {
                ramBankSize = 0x200;
            } 
            if (ramSize == 1)
            {
                ramBankSize = 0x800;
            } 
            if (ramSize > 1)
            {
                ramBankSize = 0x2000;
            } 
            return ramBankSize;
        }

        private ushort GetRomBanks(byte romSize) {
            return romSize > 0 ? (ushort) (2 << romSize) : (ushort) 2;
        }

        private byte GetRamBanks(byte ramSize, byte cartridgeType)
        {
            byte ramBanks = 0;

            if (cartridgeType == 6)
            {
                ramBanks = 1;
            }
            switch (ramSize)
            {
                case 2:
                    ramBanks = 1;
                    break;
                case 3:
                    ramBanks = 4;
                    break;
                case 4:
                    ramBanks = 16;
                    break;
                case 5:
                    ramBanks = 8;
                    break;
            }
            return ramBanks;
        }
    }
}
