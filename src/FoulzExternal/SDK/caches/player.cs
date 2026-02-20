using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FoulzExternal.SDK;
using FoulzExternal.storage;
using FoulzExternal.SDK.gamedetector;

namespace FoulzExternal.SDK.caches
{
    internal static class player
    {
        public static List<Instance> CachedPlayers { get; private set; } = new List<Instance>();
        private static Thread _tid;
        private static bool _vibin = false;
        private static readonly object _lock = new object();

        public static void Start()
        {
            if (_vibin) return;
            _vibin = true;
            _tid = new Thread(loop_cuh) { IsBackground = true };
            _tid.Start();
        }

        public static void Stop()
        {
            _vibin = false;
            try { _tid?.Join(200); } catch { }
            _tid = null;
            Clear();
        }

        public static void Clear()
        {
            lock (_lock)
            {
                CachedPlayers = new List<Instance>();
            }
        }

        private static void loop_cuh()
        {
            var list = new List<Instance>();

            while (_vibin)
            {
                try
                {
                    list.Clear();
                    var mode = finder.whatgame();

                    if (mode == GameType.pf)
                    {
                        long dm_ptr = Instance.Mem.ReadPtr(Storage.VisualEngine + 0x118);
                        var dm = new Instance(dm_ptr);
                        if (dm.IsValid)
                        {
                            var ws = dm.GetChildren().FirstOrDefault(x => x.GetClass() == "Workspace");
                            if (ws.IsValid)
                            {
                                var folder = ws.FindFirstChild("Players");
                                if (folder.IsValid)
                                {
                                    foreach (var team in folder.GetChildren())
                                    {
                                        if (team.GetClass() != "Folder") continue;
                                        foreach (var model in team.GetChildren())
                                        {
                                            if (model.GetClass() == "Model") list.Add(model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var inst = Storage.PlayersInstance;
                        if (inst.IsValid)
                        {
                            var kids = inst.GetChildren();
                            if (kids != null && kids.Count > 0)
                            {
                                foreach (var k in kids) list.Add(k);
                            }
                        }
                    }

                    if (list.Count >= 1)
                    {
                        lock (_lock)
                        {
                            CachedPlayers = new List<Instance>(list);
                        }
                    }
                }
                catch { }

                Thread.Sleep(5000);
            }
        }
    }
}