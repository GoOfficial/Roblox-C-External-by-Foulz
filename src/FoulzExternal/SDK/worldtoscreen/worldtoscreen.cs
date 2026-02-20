using FoulzExternal.SDK;
using FoulzExternal.SDK.structures;
using FoulzExternal.storage;
using Offsets;
using System;
using System.Runtime.InteropServices;

// i think i did this correct in C#

namespace FoulzExternal.SDK.worldtoscreen
{
    internal static class WorldToScreenHelper
    {
        [StructLayout(LayoutKind.Sequential)] public struct POINT { public int x; public int y; }
        [StructLayout(LayoutKind.Sequential)] public struct RECT { public int left, top, right, bottom; }

        [DllImport("user32.dll")] private static extern IntPtr FindWindow(string c, string n);
        [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr h, out RECT r);
        [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr h, out POINT p);

        public static bool GetWindowInfo(out Vector2 size, out POINT pos)
        {
            size = new Vector2 { x = 0, y = 0 };
            pos = new POINT { x = 0, y = 0 };

            IntPtr hw = FindWindow(null, "Roblox");
            if (hw == IntPtr.Zero) return false;

            if (GetClientRect(hw, out var r))
                size = new Vector2 { x = (float)(r.right - r.left), y = (float)(r.bottom - r.top) };

            return ClientToScreen(hw, out pos);
        }

        public static Vector2 WorldToScreen(Vector3 w, Matrix4 m, Vector2 res, POINT p)
        {
            float x = (w.x * m.data[0]) + (w.y * m.data[1]) + (w.z * m.data[2]) + m.data[3];
            float y = (w.x * m.data[4]) + (w.y * m.data[5]) + (w.z * m.data[6]) + m.data[7];
            float z = (w.x * m.data[8]) + (w.y * m.data[9]) + (w.z * m.data[10]) + m.data[11];
            float w_v = (w.x * m.data[12]) + (w.y * m.data[13]) + (w.z * m.data[14]) + m.data[15];

            if (w_v < 0.1f) return new Vector2 { x = -1, y = -1 };

            float invw = 1.0f / w_v;
            float ndcX = x * invw;
            float ndcY = y * invw;

            return new Vector2
            {
                x = ((res.x / 2) * ndcX + (res.x / 2)) + p.x,
                y = (-(res.y / 2) * ndcY + (res.y / 2)) + p.y
            };
        }

        public static Vector2 WorldToScreen(Vector3 world)
        {
            try
            {
                var m = Instance.Mem.Read<Matrix4>(Storage.VisualEngine + Offsets.VisualEngine.ViewMatrix);
                if (GetWindowInfo(out var res, out var p))
                    return WorldToScreen(world, m, res, p);
            }
            catch { }
            return new Vector2 { x = -1, y = -1 };
        }
    }
}