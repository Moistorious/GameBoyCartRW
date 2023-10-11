using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyDumperFrontend
{
    public class FakeSerial : ISerial
    {
        byte[] data;
        byte _bank = 0;
        public FakeSerial(string filename) {

            data = File.ReadAllBytes(filename);
        }

        public byte ReadByte(ushort address)
        {

            return (byte)data[adjustAddress(address)];
        }

        private int adjustAddress(ushort address)
        {
            int outAddress = address;
            if (_bank > 1)
            {
                outAddress += ((_bank - 1) * 0x4000);
            }
            return outAddress;
        }

        public void SelectBank(byte cartType, byte bank) {
            _bank = bank;
        }

        public byte[] ReadBytes(ushort address, byte length)
        {
            return (data.Skip(adjustAddress(address)).Take(length)).ToArray();
        }


        public void WriteByte(ushort address, byte value)
        {
        }

        public void WriteBytes(ushort address, byte[] data)
        {
        }
    }
}
