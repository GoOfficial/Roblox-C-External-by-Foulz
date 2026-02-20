using System;
using System.Threading;
using Offsets;
using SDKInstance = FoulzExternal.SDK.Instance;
using FoulzExternal.SDK;
using FoulzExternal.storage;
using Options;

namespace FoulzExternal.features.games.universal.camera
{
    internal static class CameraModule
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

                    var cache = Storage.CameraInstance;
                    if (!cache.IsValid) { Thread.Sleep(200); continue; }

                    var c = new SDKInstance(cache.Address);
                    if (!c.IsValid) { Thread.Sleep(50); continue; }

                    var s = Settings.Camera;
                    if (s.FOVEnabled) c.SetFOV(s.FOV);
                }
                catch { }

                Thread.Sleep(10);
            }
        }
    }
}