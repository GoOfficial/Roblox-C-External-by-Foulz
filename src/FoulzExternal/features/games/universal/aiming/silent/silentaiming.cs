using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using FoulzExternal.SDK;
using FoulzExternal.SDK.caches;
using FoulzExternal.SDK.structures;
using FoulzExternal.SDK.worldtoscreen;
using FoulzExternal.storage;
using Offsets;
using Options;
using FoulzExternal.features.games.universal.checks.teamcheck;
using FoulzExternal.features.games.universal.checks.downedcheck;
using FoulzExternal.features.games.universal.checks.transparencycheck;
using SDKInstance = FoulzExternal.SDK.Instance;
using Point = System.Windows.Point;

// idk if ts work in 3rd person games, but I do know it works in rivals so its all that matters to me :)

namespace FoulzExternal.features.games.universal.aiming.silent
{
    internal static class silentaiming
    {
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")] private static extern bool get_pos(out WorldToScreenHelper.POINT p);

        private static bool vibing = false;
        private static Thread brain;
        private static readonly object safety = new();
        private static Scene view = new();

        public struct FOVCircle { public Point center; public float radius; public Color outline; public Color fillColor; public bool fill; }
        public class Scene { public List<FOVCircle> circles = new(); }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Viewport { public short x; public short y; }

        public static void Start()
        {
            if (vibing) return;
            vibing = true;
            brain = new Thread(run_it) { IsBackground = true };
            brain.Start();
        }

        public static void Stop() => vibing = false;

        public static Scene GetSceneSnapshot()
        {
            lock (safety) return new Scene { circles = new List<FOVCircle>(view.circles) };
        }

        private static void run_it()
        {
            var cache = new Dictionary<long, long>();

            while (vibing)
            {
                try
                {
                    if (SDKInstance.Mem == null) { Thread.Sleep(50); continue; }

                    var s = Options.Settings.Silent;
                    bool key = s.SilentAimbotKey.IsPressed();
                    bool active = s.SilentAimbot && (s.AlwaysOn || key);

                    get_pos(out var mouse);
                    var next = new Scene();

                    if (s.ShowSilentFOV)
                    {
                        float rad = s.SFOV + (float)Math.Sin((float)Environment.TickCount / 1000f * 2.5f) * 3.0f;
                        next.circles.Add(new FOVCircle
                        {
                            center = new Point(mouse.x, mouse.y),
                            radius = rad,
                            outline = Colors.White,
                            fillColor = Color.FromArgb(40, 255, 255, 255),
                            fill = true
                        });
                    }

                    if (!active)
                    {
                        reset_vp();
                        lock (safety) view = next;
                        Thread.Sleep(5);
                        continue;
                    }

                    RobloxPlayer best = default;
                    float closest = float.MaxValue;
                    Vector2 target_screen = new Vector2();

                    var lp = Storage.LocalPlayerInstance;
                    if (lp.IsValid)
                    {
                        var targets = playerobjects.CachedPlayerObjects;
                        if (targets != null)
                        {
                            foreach (var p in targets)
                            {
                                if (p.address == 0 || p.address == lp.Address || p.Health <= 0) continue;
                                if (Settings.Checks.TeamCheck && TeamCheck.isteammate(p)) continue;
                                if (Settings.Checks.DownedCheck && DownedCheck.is_downed(p)) continue;
                                if (Settings.Checks.TransparencyCheck && TransparencyCheck.is_clear(p)) continue;

                                var pred = get_pred(p, s, cache);
                                var screen = WorldToScreenHelper.WorldToScreen(pred);

                                if (screen.x == -1) continue;

                                float dist = (float)Math.Sqrt(Math.Pow(screen.x - mouse.x, 2) + Math.Pow(screen.y - mouse.y, 2));

                                if (dist < closest && dist <= s.SFOV)
                                {
                                    closest = dist;
                                    best = p;
                                    target_screen = new Vector2 { x = screen.x, y = screen.y };
                                }
                            }
                        }
                    }

                    if (best.address != 0)
                    {
                        set_vp(target_screen);

                        if (s.SilentVisualizer)
                        {
                            next.circles.Add(new FOVCircle { center = new Point(target_screen.x, target_screen.y), radius = 13.0f, outline = Colors.Black, fillColor = Color.FromArgb(60, 0, 0, 0), fill = true });

                            float pulse = 7.0f + (float)Math.Sin((float)Environment.TickCount / 200f) * 2.5f;
                            next.circles.Add(new FOVCircle { center = new Point(target_screen.x, target_screen.y), radius = pulse, outline = Colors.Black, fillColor = Color.FromArgb(90, 0, 0, 0), fill = true });
                        }
                    }
                    else
                    {
                        reset_vp();
                    }

                    lock (safety) view = next;
                }
                catch { }
                Thread.Sleep(1);
            }
            reset_vp();
        }

        private static void reset_vp()
        {
            try
            {
                var engine = Storage.VisualEngine;
                if (engine == 0) return;
                var res = SDKInstance.Mem.Read<Vector2>(engine + Offsets.VisualEngine.Dimensions);
                SDKInstance.Mem.Write<Viewport>(Storage.CameraInstance.Address + Offsets.Camera.Viewport, new Viewport { x = (short)res.x, y = (short)res.y });
            }
            catch { }
        }

        private static void set_vp(Vector2 target)
        {
            try
            {
                var engine = Storage.VisualEngine;
                if (engine == 0) return;
                var res = SDKInstance.Mem.Read<Vector2>(engine + Offsets.VisualEngine.Dimensions);

                Viewport vp = new Viewport
                {
                    x = (short)((res.x - target.x) * 2),
                    y = (short)((res.y - target.y) * 2)
                };
                SDKInstance.Mem.Write<Viewport>(Storage.CameraInstance.Address + Offsets.Camera.Viewport, vp);
            }
            catch { }
        }

        private static Vector3 get_bone(RobloxPlayer p, int id, Dictionary<long, long> cache)
        {
            bool r15 = p.RigType == 1;
            SDKInstance part = new SDKInstance(0);

            switch (id)
            {
                case 0: part = p.Head; break;
                case 1: part = p.HumanoidRootPart.IsValid ? p.HumanoidRootPart : (r15 ? p.Upper_Torso : p.Torso); break;
                case 2: part = r15 ? (p.Left_Hand.IsValid ? p.Left_Hand : p.Left_Lower_Arm) : p.Left_Arm; break;
                case 3: part = r15 ? (p.Right_Hand.IsValid ? p.Right_Hand : p.Right_Lower_Arm) : p.Right_Arm; break;
                case 4: part = r15 ? (p.Left_Foot.IsValid ? p.Left_Foot : p.Left_Lower_Leg) : p.Left_Leg; break;
                case 5: part = r15 ? (p.Right_Foot.IsValid ? p.Right_Foot : p.Right_Lower_Leg) : p.Right_Leg; break;
                default: part = p.Head; break;
            }

            if (!part.IsValid && p.Head.IsValid) part = p.Head;
            return get_xyz(part, cache);
        }

        private static Vector3 get_xyz(SDKInstance p, Dictionary<long, long> cache)
        {
            if (!p.IsValid) return new Vector3();
            if (!cache.TryGetValue(p.Address, out long ptr))
            {
                ptr = SDKInstance.Mem.ReadPtr(p.Address + Offsets.BasePart.Primitive);
                if (ptr != 0) cache[p.Address] = ptr;
            }
            return ptr != 0 ? SDKInstance.Mem.Read<Vector3>(ptr + Offsets.Primitive.Position) : new Vector3();
        }

        private static Vector3 get_pred(RobloxPlayer p, Options.Silent s, Dictionary<long, long> cache)
        {
            var pos = get_bone(p, Settings.Aiming.TargetBone, cache);

            if (s.SPrediction)
            {
                var root = p.HumanoidRootPart.IsValid ? p.HumanoidRootPart : p.Head;
                long prim = SDKInstance.Mem.ReadPtr(root.Address + Offsets.BasePart.Primitive);
                if (prim != 0)
                {
                    var vel = SDKInstance.Mem.Read<Vector3>(prim + Offsets.Primitive.AssemblyLinearVelocity);
                    float px = s.PredictionX != 0 ? (2.1f - s.PredictionX) : 0.0f;
                    float py = s.PredictionY != 0 ? (2.1f - s.PredictionY) : 0.0f;

                    pos.x += vel.x * px;
                    pos.y += vel.y * py;
                    pos.z += vel.z * px;
                }
            }
            return pos;
        }
    }
}