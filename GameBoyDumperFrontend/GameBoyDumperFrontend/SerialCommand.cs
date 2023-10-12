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
        public bool write; // 1 byte
        public bool writeRam = false;
        public bool reset = false;
        public bool selectBank = false;

        public UInt16 address; // 2 bytes
        public byte length;
        public byte[] data = new byte[0]; // variable
        
        public byte[] ToBytes()
        {
            if(write) { length = (byte)data.Length; }

            byte writeFlag = 0;
            if (write)
            {
                writeFlag |= 1;
            }
            if(writeRam)
            {
                writeFlag |= 2;
            }
            if(reset)
            {
                writeFlag |= 4;
            }
            if (selectBank)
            {
                writeFlag |= 8;
            }

            byte[] output = new byte[data.Length + 4];
            output[0] = writeFlag;
            output[1] = (byte)(address & 0x00FF);
            output[2] = (byte)(address >> 8);

            output[3] = (byte)(length & 0x00FF);
            //output[4] = (byte)(length >> 8);

            Array.Copy(data, 0, output, 4, data.Length);

            return output;
        }
    }
}
