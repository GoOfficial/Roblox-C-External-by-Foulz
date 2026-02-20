using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using FoulzExternal;
using FoulzExternal.SDK;
using FoulzExternal.storage;
using FoulzExternal.logging.notifications;
using Offsets;
using FoulzExternal.SDK.caches;

// my same TPHandler I use for C++ code, but I think I may have implemented it correctly, im not too sure

namespace FoulzExternal.SDK.tphandler
{
    internal static class TPHandler
    {
        private static Thread t;
        private static bool active;
        private static readonly object locker = new();

        public static void Start()
        {
            if (t != null && t.IsAlive) return;
            active = true;
            t = new Thread(watch_vibe) { IsBackground = true };
            t.Start();
        }

        public static void Stop()
        {
            active = false;
            try { t?.Join(500); } catch { }
            t = null;
        }

        private static bool game_up(Instance dm)
        {
            try
            {
                return dm.IsValid && Instance.Mem.Read<int>(dm.Address + Offsets.DataModel.GameLoaded) != 0;
            }
            catch { return false; }
        }

        private static void watch_vibe()
        {
            bool was_open = false;
            int last_p = 0;
            long last_g = 0;

            while (active)
            {
                try
                {
                    var procs = Process.GetProcessesByName("RobloxPlayerBeta")
                        .Concat(Process.GetProcessesByName("RobloxPlayer"))
                        .Concat(Process.GetProcessesByName("Roblox")).ToArray();

                    if (procs.Length == 0)
                    {
                        if (was_open)
                        {
                            notify.Notify("Roblox", "Roblox client termination detected. All worker threads placed in hold state.", 4000);
                            was_open = false;
                        }
                        Instance.Mem = null;
                        Storage.Clear();
                        player.Clear();
                        playerobjects.Clear();
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (!was_open)
                    {
                        notify.Notify("Roblox", "Roblox client launch detected. Initializing systems and worker threads.", 4000);
                        was_open = true;
                    }

                    var m = new Memory();
                    if (!m.Attach("RobloxPlayerBeta") && !m.Attach("RobloxPlayer") && !m.Attach("Roblox"))
                    {
                        Thread.Sleep(120);
                        continue;
                    }

                    Storage.Initialize(m);
                    var dm = Storage.DataModelInstance;
                    if (!dm.IsValid)
                    {
                        Thread.Sleep(120);
                        continue;
                    }

                    int p_id = 0;
                    long g_id = 0;
                    try { p_id = (int)dm.GetPlaceID(); } catch { }
                    try { g_id = dm.GetGameID(); } catch { }

                    string dmn = "";
                    try { dmn = dm.GetName() ?? ""; } catch { }

                    bool ok = game_up(dm);

                    if (ok)
                    {
                        player.Start();
                        playerobjects.Start();
                    }

                    if (last_p == 0 && last_g == 0 && ok)
                    {
                        last_p = p_id;
                        last_g = g_id;
                    }

                    if (dmn == "LuaApp" || (ok && (p_id != last_p || g_id != last_g)))
                    {
                        lock (locker)
                        {
                            if (ok && g_id != last_g && last_g != 0)
                                notify.Notify("Roblox", "Game context change detected. Reinitializing all worker threads and subsystems.", 4000);

                            Instance.Mem = null;
                            Storage.Clear();
                            player.Clear();
                            playerobjects.Clear();
                            last_p = 0;
                            last_g = 0;
                        }

                        bool found = false;
                        for (int i = 0; active && !found && i < 600; i++)
                        {
                            try
                            {
                                var check = Process.GetProcessesByName("RobloxPlayerBeta")
                                    .Concat(Process.GetProcessesByName("RobloxPlayer"))
                                    .Concat(Process.GetProcessesByName("Roblox")).ToArray();

                                if (check.Length == 0) break;

                                var m2 = new Memory();
                                if (!m2.Attach("RobloxPlayerBeta") && !m2.Attach("RobloxPlayer") && !m2.Attach("Roblox"))
                                {
                                    Thread.Sleep(120);
                                    continue;
                                }

                                Storage.Initialize(m2);
                                var dmi = Storage.DataModelInstance;
                                if (!dmi.IsValid) { Thread.Sleep(120); continue; }

                                bool ok2 = game_up(dmi);
                                long gid2 = 0;
                                try { gid2 = dmi.GetGameID(); } catch { }
                                string n2 = "";
                                try { n2 = dmi.GetName() ?? ""; } catch { }

                                if (ok2 && !string.IsNullOrEmpty(n2) && n2 != "LuaApp" && gid2 != 0)
                                {
                                    try { last_p = (int)dmi.GetPlaceID(); } catch { }
                                    last_g = gid2;
                                    player.Start();
                                    playerobjects.Start();
                                    found = true;
                                    break;
                                }
                            }
                            catch { }
                            Thread.Sleep(120);
                        }
                    }
                }
                catch { }
                Thread.Sleep(100);
            }
        }
    }
}