using System;
using System.Threading;
using FoulzExternal.SDK;
using Offsets;
using Options;
using FoulzExternal.SDK.structures;
using FoulzExternal.SDK.caches;
using FoulzExternal.games.universal.visuals;
using FoulzExternal.storage;

namespace FoulzExternal.features.games.universal.desync
{
    internal static class desync
    {
        private static bool vibing = false;
        private static Thread brain;
        private static readonly object safety = new();

        private static bool is_active = false;
        private static bool held = false;
        private static Vector3 spawn = new();

        private static int old_hash = 0;
        private static DateTime chill_out = DateTime.MinValue;

        public class Scene { public bool Active; public Vector3 Position; }

        public static void Start()
        {
            if (vibing) return;
            vibing = true;
            brain = new Thread(do_work) { IsBackground = true };
            brain.Start();
        }

        public static void Stop() => vibing = false;

        public static Scene GetSceneSnapshot()
        {
            lock (safety) return new Scene { Active = is_active, Position = spawn };
        }
        private static Vector3 get_coords()
        {
            try
            {
                var lp = Storage.LocalPlayerInstance;
                if (!lp.IsValid) return new Vector3();
                var guys = playerobjects.CachedPlayerObjects;
                if (guys == null) return new Vector3();
                var localObj = System.Linq.Enumerable.FirstOrDefault(guys, x => x.address == lp.Address);
                if (localObj.address == 0 || !localObj.HumanoidRootPart.IsValid) return new Vector3();
                return visuals.GetPos(localObj.HumanoidRootPart, new System.Collections.Generic.Dictionary<long, long>());
            }
            catch { return new Vector3(); }
        }

        private static int get_hash()
        {
            var b = Options.Settings.Network.DeSyncBind;
            if (b == null) return 0;
            return (b.Key & 0xFFFF) | ((b.MouseButton & 0xFF) << 16) ^ ((b.ControllerButton & 0xFF) << 24);
        }

        private static void do_work()
        {
            while (vibing)
            {
                try
                {
                    if (!Options.Settings.Network.DeSync)
                    {
                        if (is_active)
                        {
                            is_active = false;
                            lock (safety) spawn = new();
                        }
                        Thread.Sleep(200);
                        continue;
                    }

                    int cur = get_hash();
                    if (cur != old_hash)
                    {
                        old_hash = cur;
                        chill_out = DateTime.UtcNow.AddMilliseconds(300);
                    }

                    bool key = Options.Settings.Network.DeSyncBind.IsPressed();
                    if (DateTime.UtcNow < chill_out) key = false;

                    if (key && !held)
                    {
                        held = true;
                        is_active = true;
                        lock (safety) spawn = get_coords();
                        try { SDK.Instance.Mem.Write<bool>(SDK.Instance.Mem.Base + FFlagOffsets.FFlags.NextGenReplicatorEnabledWrite4, true); } catch { }
                    }
                    else if (!key && held)
                    {
                        held = false;
                        is_active = false;
                        lock (safety) spawn = new();
                        try { SDK.Instance.Mem.Write<bool>(SDK.Instance.Mem.Base + FFlagOffsets.FFlags.NextGenReplicatorEnabledWrite4, false); } catch { }
                    }

                    Thread.Sleep(30);
                }
                catch { Thread.Sleep(100); }
            }

            try { SDK.Instance.Mem.Write<bool>(SDK.Instance.Mem.Base + FFlagOffsets.FFlags.NextGenReplicatorEnabledWrite4, false); } catch { }
            is_active = false;
            lock (safety) spawn = new();
        }
    }
}