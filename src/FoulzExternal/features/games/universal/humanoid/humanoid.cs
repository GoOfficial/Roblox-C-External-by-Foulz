using System;
using System.Threading;
using Offsets;
using SDKInstance = FoulzExternal.SDK.Instance;
using FoulzExternal.SDK;
using FoulzExternal.storage;
using Options;

namespace FoulzExternal.games.universal.humanoid
{
    internal static class HumanoidModule
    {
        private static Thread t;
        private static bool active;

        public static void Start()
        {
            if (active) return;
            active = true;
            t = new Thread(tick) { IsBackground = true };
            t.Start();
        }

        public static void Stop() => active = false;

        private static void tick()
        {
            while (active)
            {
                try
                {
                    if (!Storage.IsInitialized || SDKInstance.Mem == null) { Thread.Sleep(200); continue; }

                    var me = Storage.LocalPlayerInstance;
                    if (!me.IsValid) { Thread.Sleep(200); continue; }

                    var chara = me.GetCharacter();
                    if (!chara.IsValid) { Thread.Sleep(50); continue; }

                    var hum = chara.FindFirstChildOfClass("Humanoid");
                    if (!hum.IsValid) { Thread.Sleep(50); continue; }

                    var s = Settings.Humanoid;
                    if (s.WalkspeedEnabled) me.SetWalkspeed(s.Walkspeed);
                    if (s.JumpPowerEnabled) me.SetJumpPower(s.JumpPower);
                }
                catch { }

                Thread.Sleep(10);
            }
        }
    }
}