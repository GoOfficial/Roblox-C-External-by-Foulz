using System;
using System.Runtime.InteropServices;

// this is my keybind code that I use for my C++ external, works like a charm

namespace FoulzExternal.helpers.keybind
{
    public class KeyBind
    {
        public int Key { get; set; } = 0;
        public int MouseButton { get; set; } = -1;
        public int ControllerButton { get; set; } = -1;
        public bool Waiting { get; set; } = false;
        public string Label { get; set; }

        public KeyBind(string label)
        {
            Label = label ?? string.Empty;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        private static extern uint XInputGetState14(uint dwUserIndex, out XINPUT_STATE pState);
        [DllImport("xinput1_3.dll", EntryPoint = "XInputGetState")]
        private static extern uint XInputGetState13(uint dwUserIndex, out XINPUT_STATE pState);

        private static bool TryGetXInputState(out XINPUT_STATE state)
        {
            state = new XINPUT_STATE();
            try
            {
                uint r = XInputGetState14(0, out state);
                if (r == 0) return true;
            }
            catch { }
            try
            {
                uint r = XInputGetState13(0, out state);
                if (r == 0) return true;
            }
            catch { }
            return false;
        }

        private static ushort MapControllerButton(int idx)
        {
            return idx switch
            {
                0 => 0x1000,
                1 => 0x2000,
                2 => 0x4000,
                3 => 0x8000,
                4 => 0x0100,
                5 => 0x0200,
                6 => 0x0020,
                7 => 0x0010,
                8 => 0x0040,
                9 => 0x0080,
                _ => (ushort)0,
            };
        }

        public bool IsPressed()
        {
            try
            {
                if (Key > 0 && Key < 256)
                {
                    if ((GetAsyncKeyState(Key) & 0x8000) != 0) return true;
                }

                if (MouseButton >= 0)
                {
                    int vk = MouseButton switch
                    {
                        0 => 0x01,
                        1 => 0x02,
                        2 => 0x04,
                        _ => 0
                    };
                    if (vk != 0 && (GetAsyncKeyState(vk) & 0x8000) != 0) return true;
                }

                if (ControllerButton >= 0)
                {
                    if (TryGetXInputState(out var st))
                    {
                        var mask = MapControllerButton(ControllerButton);
                        if (mask != 0 && (st.Gamepad.wButtons & mask) != 0) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool IsPressed(KeyBind bind) => bind != null && bind.IsPressed();

        public override string ToString() => Label ?? string.Empty;
    }
}
