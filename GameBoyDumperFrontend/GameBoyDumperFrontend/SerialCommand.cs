using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyDumperFrontend
{
    public class SerialCommand
    {
        public enum CommandType
        {
            InitializeCart, // Set the cart type and stuff in memory (so the other commands work correctly)
            Validate, // Just make sure the Nintendo Logo is correct (Basically, is the cart seated?)
            Reset, // Reset all variables, and reset the cart
            SelectBank,

            ReadByte,
            ReadRange,

            WriteByte,
            WriteRange,

            WriteRamByte,
            WriteRamRange,

            GetHeader, // Send full header
            GetTitle, // Send just the title

            WriteFullRAM,
            WriteFullROM,

            GetFullROM, // Get the entire ROM
            GetFullRAM, // Get the entire RAM
        };

        public CommandType Command; // 2 bytes
        public UInt16 address; // 2 bytes
        public UInt16 length; // 2 bytes
        public byte[] data = new byte[0]; // variable
        
        public byte[] ToBytes()
        {
            
            byte[] output = new byte[data.Length + 6];
            output[0] = (byte)(((ushort)Command) & 0x00FF);
            output[1] = (byte)(((ushort)Command) >> 8);

            output[2] = (byte)(address & 0x00FF);
            output[3] = (byte)(address >> 8);

            output[4] = (byte)(length & 0x00FF);
            output[5] = (byte)(length >> 8);

            Array.Copy(data, 0, output, 6, data.Length);

            return output;
        }
    }
}
