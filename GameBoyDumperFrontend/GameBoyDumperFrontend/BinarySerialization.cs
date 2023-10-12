using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyDumperFrontend
{
    internal class BinarySerialization
    {
        public static T FromByteArray<T>(byte[] bytes)
        {

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            var pointer = handle.AddrOfPinnedObject();
            Type structureType = typeof(T);

            T? theStructure = (T?)Marshal.PtrToStructure(pointer, structureType);
            handle.Free();

            if(!(theStructure is T))
            {
                throw new Exception($"Failed to Marshal to {typeof(T).FullName}");
            }
            return theStructure;
        }
    }
}
