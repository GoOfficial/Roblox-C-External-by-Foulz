using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// same code I use for all my externals 

namespace FoulzExternal
{
    namespace buffer
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Buffer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public byte[] data;
        }
    }
    public class Memory
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint a, bool b, int p);

        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAlloc(IntPtr a, UIntPtr s, uint t, uint p);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string n);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr m, string n);

        private delegate int SFn(IntPtr h, IntPtr a, byte[] b, uint l, out uint r);

        private SFn _r;
        private SFn _w;

        public IntPtr Handle;
        public long Base;


        public bool Attach(string n)
        {
            Process[] ps = Process.GetProcessesByName(n);
            if (ps.Length == 0) return false;

            Process p = ps[0];
            Handle = OpenProcess(0x1F0FFF, false, p.Id);
            Base = p.MainModule.BaseAddress.ToInt64();

            _r = (SFn)Marshal.GetDelegateForFunctionPointer(v("NtReadVirtualMemory"), typeof(SFn));
            _w = (SFn)Marshal.GetDelegateForFunctionPointer(v("NtWriteVirtualMemory"), typeof(SFn));

            return Handle != IntPtr.Zero;
        }

        private IntPtr v(string n)
        {
            IntPtr m = GetModuleHandle("ntdll.dll");
            IntPtr a = GetProcAddress(m, n);
            uint id = 0;

            for (int i = 0; i < 32; i++)
            {
                if (Marshal.ReadByte(a, i) == 0xB8)
                {
                    id = (uint)Marshal.ReadInt32(a, i + 1);
                    break;
                }
            }

            byte[] c = {
                0x4C, 0x8B, 0xD1,
                0xB8, 0x00, 0x00, 0x00, 0x00,
                0x0F, 0x05,
                0xC3
            };

            byte[] b = BitConverter.GetBytes(id);
            Buffer.BlockCopy(b, 0, c, 4, 4);

            IntPtr s = VirtualAlloc(IntPtr.Zero, (UIntPtr)c.Length, 0x3000, 0x40);
            Marshal.Copy(c, 0, s, c.Length);
            return s;
        }

        public T Read<T>(long a) where T : struct
        {
            int s = Marshal.SizeOf(typeof(T));
            byte[] b = new byte[s];
            _r(Handle, (IntPtr)a, b, (uint)s, out _);
            GCHandle h = GCHandle.Alloc(b, GCHandleType.Pinned);
            T d = (T)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(T));
            h.Free();
            return d;
        }

        public void Write<T>(long a, T v) where T : struct
        {
            int s = Marshal.SizeOf(typeof(T));
            byte[] b = new byte[s];
            IntPtr p = Marshal.AllocHGlobal(s);
            Marshal.StructureToPtr(v, p, false);
            Marshal.Copy(p, b, 0, s);
            Marshal.FreeHGlobal(p);
            _w(Handle, (IntPtr)a, b, (uint)s, out _);
        }

        public long ReadPtr(long a) => Read<long>(a);

        public string ReadString(long address)
        {
            var buffer = Read<buffer.Buffer>(address);
            return Encoding.UTF8.GetString(buffer.data).Split('\0')[0];
        }
    }
}