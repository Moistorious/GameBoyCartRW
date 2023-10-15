using System.IO.Ports;

namespace GameBoyDumperFrontend.Interface
{
    public interface ICartridgeCommunication
    {
        byte ReadByte(ushort address);
        byte[] ReadBytes(ushort address, byte length);
        void WriteByte(ushort address, byte value);
        void WriteBytes(ushort address, byte[] data, bool RAM = false);
        public void SelectBank(byte cartType, byte bank);
        void Reset();
        byte[] GetHeader();
        void GetROM(byte[] buffer);
        void Init();
        void GetRAM(byte[] buffer);
    }
}