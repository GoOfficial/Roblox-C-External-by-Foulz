using FoulzExternal.SDK;
using FoulzExternal.SDK.caches;
using FoulzExternal.SDK.structures;
using FoulzExternal.SDK.worldtoscreen;
using FoulzExternal.storage;
using Offsets;
using Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using SDKInstance = FoulzExternal.SDK.Instance;
using FoulzExternal.features.games.universal.checks.teamcheck;

namespace FoulzExternal.games.universal.visuals
{
    public static class visuals
    {
        [DllImport("user32.dll")] private static extern bool GetCursorPos(out WorldToScreenHelper.POINT p);
        [DllImport("user32.dll")] private static extern IntPtr FindWindow(string c, string n);
        [DllImport("user32.dll")] private static extern bool IsWindow(IntPtr h);
        [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr h, out WorldToScreenHelper.RECT r);
        [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr h, out WorldToScreenHelper.POINT p);

        private static bool isRunning = false;
        private static Thread worker;
        private static readonly object locker = new();
        private static Scene currentScene = new();

        public struct Box { public Rect r; public bool f; }
        public struct Line { public Point a, b; public Color c; public double w; }
        public struct Text { public string t; public Point p; public Color c; public double s; public bool ctr; }
        public struct Dot { public Point p; public double r; public Color c; }

        public class Scene
        {
            public List<Box> boxes = new();
            public List<Line> lines = new();
            public List<Text> texts = new();
            public List<Dot> dots = new();
        }

        public static void Start()
        {
            if (isRunning) return;
            isRunning = true;

            try { IMGUI.Program.start(); } catch { }

            worker = new Thread(MainLoop) { IsBackground = true };
            worker.Start();
        }

        public static void Stop() => isRunning = false;

        public static Scene GetSceneSnapshot()
        {
            lock (locker)
            {
                return new Scene
                {
                    boxes = new(currentScene.boxes),
                    lines = new(currentScene.lines),
                    texts = new(currentScene.texts),
                    dots = new(currentScene.dots)
                };
            }
        }

        private static void MainLoop()
        {
            var cache = new Dictionary<long, long>();
            IntPtr game = IntPtr.Zero;

            while (isRunning)
            {
                if (SDKInstance.Mem == null) { Thread.Sleep(50); continue; }
                if (game == IntPtr.Zero || !IsWindow(game)) game = FindWindow(null, "Roblox");
                if (game == IntPtr.Zero) { Thread.Sleep(200); continue; }
                if (!GetClientRect(game, out var rect) || !ClientToScreen(game, out var wp)) { Thread.Sleep(10); continue; }
                var res = new Vector2 { x = rect.right - rect.left, y = rect.bottom - rect.top };
                var view = SDKInstance.Mem.Read<Matrix4>(Storage.VisualEngine + Offsets.VisualEngine.ViewMatrix);
                var guys = playerobjects.CachedPlayerObjects;

                if (guys == null) { Thread.Sleep(5); continue; }

                var next = new Scene();
                var lp = Storage.LocalPlayerInstance;
                var localObj = guys.FirstOrDefault(x => x.address == lp.Address);
                Vector3 localPos = (localObj.address != 0 && localObj.Head.IsValid) ? GetPos(localObj.Head, cache) : new Vector3();

                foreach (var p in guys)
                {
                    if (p.address == 0 || (p.address == lp.Address && !Settings.Visuals.LocalPlayerESP)) continue;
                    if (Settings.Checks.TeamCheck && TeamCheck.isteammate(p)) continue;

                    var ut = p.Upper_Torso.IsValid ? p.Upper_Torso : p.Torso;
                    var lt = p.Lower_Torso.IsValid ? p.Lower_Torso : p.HumanoidRootPart;

                    var bounds = new List<SDKInstance> {
                        p.Head, p.HumanoidRootPart, ut, lt,
                        p.Left_Arm, p.Right_Arm, p.Left_Leg, p.Right_Leg,
                        p.Left_Foot, p.Right_Foot, p.Left_Hand, p.Right_Hand
                    };

                    float x1 = 1e6f, y1 = 1e6f, x2 = -1e6f, y2 = -1e6f;
                    Vector2 head2d = new Vector2 { x = -1, y = -1 };
                    bool valid = false;

                    foreach (var part in bounds)
                    {
                        if (!part.IsValid) continue;
                        var w3d = GetPos(part, cache);

                        if (part.Address == p.Head.Address) w3d.y += 0.5f;
                        if (part.Address == p.Left_Foot.Address || part.Address == p.Right_Foot.Address) w3d.y -= 0.5f;

                        var s2d = WorldToScreenHelper.WorldToScreen(w3d, view, res, wp);
                        if (s2d.x != -1)
                        {
                            x1 = Math.Min(x1, s2d.x); x2 = Math.Max(x2, s2d.x);
                            y1 = Math.Min(y1, s2d.y); y2 = Math.Max(y2, s2d.y);
                            valid = true;
                            if (part.Address == p.Head.Address) head2d = s2d;
                        }
                    }

                    if (!valid) continue;

                    var b = new Rect(x1, y1, x2 - x1, y2 - y1);
                    var hrpPos = GetPos(p.HumanoidRootPart, cache);
                    var dist = (float)Math.Sqrt(Math.Pow(localPos.x - hrpPos.x, 2) + Math.Pow(localPos.z - hrpPos.z, 2));

                    if (Settings.Visuals.BoxESP) next.boxes.Add(new Box { r = b, f = Settings.Visuals.FilledBox });

                    if (Settings.Visuals.CornerESP)
                    {
                        double th = b.Width / 4;
                        next.lines.Add(new Line { a = b.TopLeft, b = new Point(b.Left + th, b.Top), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.TopLeft, b = new Point(b.Left, b.Top + th), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.TopRight, b = new Point(b.Right - th, b.Top), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.TopRight, b = new Point(b.Right, b.Top + th), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.BottomLeft, b = new Point(b.Left + th, b.Bottom), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.BottomLeft, b = new Point(b.Left, b.Bottom - th), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.BottomRight, b = new Point(b.Right - th, b.Bottom), c = Colors.White, w = 1 });
                        next.lines.Add(new Line { a = b.BottomRight, b = new Point(b.Right, b.Bottom - th), c = Colors.White, w = 1 });
                    }

                    if (Settings.Visuals.Health)
                    {
                        float hp = p.Health / p.MaxHealth;
                        next.lines.Add(new Line { a = new Point(b.Left - 5, b.Bottom), b = new Point(b.Left - 5, b.Top), c = Colors.Red, w = 2 });
                        next.lines.Add(new Line { a = new Point(b.Left - 5, b.Bottom), b = new Point(b.Left - 5, b.Bottom - (b.Height * hp)), c = Colors.Lime, w = 2 });
                    }

                    if (Settings.Visuals.Name) next.texts.Add(new Text { t = p.Name, p = new Point(b.Left + b.Width / 2, b.Top - 15), c = Colors.White, s = Settings.Visuals.NameSize, ctr = true });
                    if (Settings.Visuals.Distance) next.texts.Add(new Text { t = $"{(int)dist}m", p = new Point(b.Left + b.Width / 2, b.Bottom + 2), c = Colors.White, s = Settings.Visuals.DistanceSize, ctr = true });

                    if (Settings.Visuals.HeadCircle && head2d.x != -1)
                        next.dots.Add(new Dot { p = new Point(head2d.x, head2d.y), r = (b.Height / 12) * Settings.Visuals.HeadCircleMaxScale, c = Colors.White });

                    if (Settings.Visuals.Tracers)
                    {
                        Point start = new Point(res.x / 2, res.y);
                        if (Settings.Visuals.TracersStart == 1) start = new Point(res.x / 2, 0);
                        else if (Settings.Visuals.TracersStart == 2 && GetCursorPos(out var m)) start = new Point(m.x, m.y);

                        var target = head2d.x != -1 ? new Point(head2d.x, head2d.y) : new Point(b.Left + b.Width / 2, b.Top);
                        next.lines.Add(new Line { a = start, b = target, c = Colors.White, w = Settings.Visuals.TracerThickness });
                    }

                    if (Settings.Visuals.Skeleton)
                    {
                        Action<Vector3, Vector3> Join = (v1, v2) => {
                            if (v1.x == 0 || v2.x == 0) return;
                            var s1 = WorldToScreenHelper.WorldToScreen(v1, view, res, wp);
                            var s2 = WorldToScreenHelper.WorldToScreen(v2, view, res, wp);
                            if (s1.x != -1 && s2.x != -1) next.lines.Add(new Line { a = new Point(s1.x, s1.y), b = new Point(s2.x, s2.y), c = Colors.White, w = 1 });
                        };

                        var h = GetPos(p.Head, cache);
                        if (p.RigType == 1)
                        {
                            var u = GetPos(p.Upper_Torso, cache); var l = GetPos(p.Lower_Torso, cache);
                            Join(h, u); Join(u, l);
                            Join(u, GetPos(p.Left_Upper_Arm, cache)); Join(GetPos(p.Left_Upper_Arm, cache), GetPos(p.Left_Hand, cache));
                            Join(u, GetPos(p.Right_Upper_Arm, cache)); Join(GetPos(p.Right_Upper_Arm, cache), GetPos(p.Right_Hand, cache));
                            Join(l, GetPos(p.Left_Upper_Leg, cache)); Join(GetPos(p.Left_Upper_Leg, cache), GetPos(p.Left_Foot, cache));
                            Join(l, GetPos(p.Right_Upper_Leg, cache)); Join(GetPos(p.Right_Upper_Leg, cache), GetPos(p.Right_Foot, cache));
                        }
                        else
                        {
                            var t = GetPos(p.Torso.IsValid ? p.Torso : p.HumanoidRootPart, cache);
                            Join(h, t); Join(t, GetPos(p.Left_Arm, cache)); Join(t, GetPos(p.Right_Arm, cache));
                            Join(t, GetPos(p.Left_Leg, cache)); Join(t, GetPos(p.Right_Leg, cache));
                        }
                    }

                    if (Settings.Visuals.ChinaHat && p.Head.IsValid)
                    {
                        var cp = GetPos(p.Head, cache);
                        var tip = WorldToScreenHelper.WorldToScreen(new Vector3 { x = cp.x, y = cp.y + 1.2f, z = cp.z }, view, res, wp);
                        if (tip.x != -1)
                        {
                            Point? f = null, l = null;
                            for (int a = 0; a <= 360; a += 30)
                            {
                                double rd = a * Math.PI / 180;
                                var r3 = new Vector3 { x = cp.x + (float)Math.Cos(rd) * 1.8f, y = cp.y + 0.2f, z = cp.z + (float)Math.Sin(rd) * 1.8f };
                                var r2 = WorldToScreenHelper.WorldToScreen(r3, view, res, wp);
                                if (r2.x != -1)
                                {
                                    var cur = new Point(r2.x, r2.y);
                                    next.lines.Add(new Line { a = new Point(tip.x, tip.y), b = cur, c = Colors.White, w = 1 });
                                    if (l != null) next.lines.Add(new Line { a = l.Value, b = cur, c = Colors.White, w = 1 });
                                    f ??= cur; l = cur;
                                }
                            }
                            if (f != null && l != null) next.lines.Add(new Line { a = l.Value, b = f.Value, c = Colors.White, w = 1 });
                        }
                    }
                }
                lock (locker) currentScene = next;
                Thread.Sleep(1);
            }
        }

        public static Vector3 GetPos(SDKInstance i, Dictionary<long, long> c)
        {
            if (!i.IsValid) return new Vector3();
            if (!c.TryGetValue(i.Address, out long ptr))
            {
                ptr = SDKInstance.Mem.ReadPtr(i.Address + Offsets.BasePart.Primitive);
                if (ptr != 0) c[i.Address] = ptr;
            }
            return ptr != 0 ? SDKInstance.Mem.Read<Vector3>(ptr + Offsets.Primitive.Position) : new Vector3();
        }
    }
}