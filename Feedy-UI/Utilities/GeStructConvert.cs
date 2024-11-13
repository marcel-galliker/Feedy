using Infragistics.Controls.Charts;
using System;
using System.Runtime.InteropServices;

namespace GE_Utilities
{
    public class GeStructConvert
    {
        //--- ToStruct --------------------------------------
        public static int ToStruct<Type>(out Type dst, byte[] src, int offset=0)
        {
            int size = Marshal.SizeOf(typeof(Type));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(src, offset, ptr, size);
            dst = (Type)Marshal.PtrToStructure(ptr, typeof(Type));
            Marshal.FreeHGlobal(ptr);
            return size;
        }

        //--- ToBuffer --------------------------------------------
        public static void ToBuffer<Type>(out byte[] dst, Type src, int hdrsize=0)
        {
            int size    = Marshal.SizeOf(typeof(Type))+hdrsize;
            IntPtr ptr  = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(src, ptr, false);
            dst = new byte[size];
            Marshal.Copy(ptr, dst, hdrsize, size-hdrsize);
            Marshal.FreeHGlobal(ptr);            
        }

        //--- ToBuffer_Byte --------------------------------------------
        public static void ToBuffer_Byte(out byte[] dst, char[] src, int hdrsize, int size)
        {
            IntPtr ptr  = Marshal.AllocHGlobal(hdrsize+size);
            Marshal.StructureToPtr(src, ptr, false);
            dst = new byte[size];
            Marshal.Copy(ptr, dst, hdrsize, hdrsize+size);
            Marshal.FreeHGlobal(ptr);            
        }

        //--- OverwriteBuffer ---------------------------------------
        public static void OverwriteBuffer<Type>(byte[] dst, Type src)
        {
            int size = Marshal.SizeOf(typeof(Type));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(src, ptr, false);
            Marshal.Copy(ptr, dst, 0, size);
            Marshal.FreeHGlobal(ptr);            
        }

        //--- Compare -----------------------------
        public static int Compare<Type>(Type src, Type dst)
        {
            ToBuffer(out byte[] bytes_s, src );
            ToBuffer(out byte[] bytes_d, dst );
            for(int i=0; i<bytes_s.Length; i++)
            {
                if (bytes_s[i] < bytes_d[i]) return -1;
                if (bytes_s[i] > bytes_d[i]) return 1;
            }
            return 0;
        }

    }
}
