using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GameBoyDumperFrontend.CartClasses
{
    [StructLayout(LayoutKind.Sequential, Pack = 1,CharSet = CharSet.Ansi, Size = 0x150)]
    unsafe public struct CartHeader
    {
        const int HeaderOffset = 0x100;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (0x103 - HeaderOffset + 1))]
        byte[] EntryPoint;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x133 - 0x104 + 1)]
        public byte[] NintendoLogo;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x143 - 0x134 + 1)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x143 - 0x134 + 1)]
        public string Title;

        //[FieldOffset(0x13F - HeaderOffset)]
        //public fixed byte ManufacturerCode[0x142 - 0x13F + 1];

        //[FieldOffset(0x143)]
        //public byte CGBFlag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x145 - 0x144 + 1)]
        public byte[] NewLicenseeCode;

        public byte SGBflag;

        public byte CartridgeType;

        public byte RomSize;

        public byte RamSize;

        public byte DestinationCode;

        public byte OldLicenseeCode;

        public byte MaskRomVersionNumber;

        public byte HeaderChecksum;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14F - 0x14E + 1)]
        public byte[] GlobalChecksum;

    }
}
