using System.IO.Ports;

namespace GameBoyDumperFrontend
{
    public interface ISerial
    {
        byte ReadByte(ushort address);
        byte[] ReadBytes(ushort address, byte length);
        void WriteByte(ushort address, byte value);
        void WriteBytes(ushort address, byte[] data);
        public void SelectBank(byte cartType, byte bank);
    }
}