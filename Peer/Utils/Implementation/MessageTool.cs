using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
    public class MessageTool
    {
        //public static unsafe byte[] Struct2Bytes(Object obj)
        //{
        //    int size = Marshal.SizeOf(obj);
        //    byte[] bytes = new byte[size];
        //    fixed (byte* pb = &bytes[0])
        //    {
        //        Marshal.StructureToPtr(obj, new IntPtr(pb), true);
        //    }
        //    return bytes;
        //}

        //public static unsafe Object Bytes2Struct(byte[] bytes,Type type)
        //{
        //    fixed (byte* pb = &bytes[0])
        //    {
        //        return Marshal.PtrToStructure(new IntPtr(pb), type);
        //    }
        //}

    }
}
