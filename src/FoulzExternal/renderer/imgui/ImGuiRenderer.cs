using ImGuiNET;
using ClickableTransparentOverlay;
using System.Numerics;
using FoulzExternal.games.universal.visuals;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;
using FoulzExternal.games.universal.aiming;
using System.Windows.Media;
using FoulzExternal.features.games.universal.desync;
using FoulzExternal.features.games.universal.aiming.silent;
using System.Windows;

// just imgui code that i had to made for visuals and shit

namespace IMGUI
{
    public class Program : Overlay
    {
        private static readonly object l = new();
        private static bool running = false;
        private static readonly System.Collections.Generic.Dictionary<long, long> localCache = new();

        private static System.Windows.Threading.Dispatcher watcher;

        private static ExitEventHandler? onAppExit;
        private static EventHandler? onDispatcherShutdown;
        private static EventHandler? onProcessExit;

        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private const uint WM_CLOSE = 0x0010;

        protected override void Render()
        {
            var v = visuals.GetSceneSnapshot();
            if (v != null)
            {
                var d = ImGui.GetBackgroundDrawList();

                foreach (var b in v.boxes)
                {
                    var min = new Vector2((float)b.r.Left, (float)b.r.Top);
                    var max = new Vector2((float)(b.r.Left + b.r.Width), (float)(b.r.Top + b.r.Height));

                    d.AddRectFilled(new Vector2(min.X + 1, min.Y + 1), new Vector2(max.X + 1, max.Y + 1), 0x30000000);
                    if (b.f) d.AddRectFilled(min, max, 0x40000000);
                    d.AddRect(min, max, 0xFFFFFFFF);
                    d.AddRect(new Vector2(min.X + 1, min.Y + 1), new Vector2(max.X - 1, max.Y - 1), 0x80FFFFFF);
                }

                foreach (var l in v.lines)
                    d.AddLine(new Vector2((float)l.a.X, (float)l.a.Y), new Vector2((float)l.b.X, (float)l.b.Y), u32(l.c), Math.Max(1.0f, (float)l.w));

                foreach (var o in v.dots)
                {
                    var p = new Vector2((float)o.p.X, (float)o.p.Y);
                    uint c = u32(o.c);
                    d.AddCircleFilled(p, Math.Max(0.5f, (float)o.r - 1.0f), (c & 0x00FFFFFF) | 0x40000000);
                    d.AddCircle(p, (float)o.r, c, 16, 1.0f);
                }

                foreach (var t in v.texts)
                {
                    var pos = new Vector2((float)t.p.X, (float)t.p.Y);
                    uint c = u32(t.c);
                    float w = Math.Max(30, t.t.Length * (float)t.s * 0.6f);
                    float x = t.ctr ? pos.X - w / 2 : pos.X;

                    d.AddRectFilled(new Vector2(x, pos.Y), new Vector2(x + w, pos.Y + (float)t.s + 6.0f), 0x60000000);
                    d.AddText(new Vector2(x + 4, pos.Y + 3), c, t.t);
                }
            }

            try
            {
                var a = aiming.GetSceneSnapshot();
                if (a != null && a.circles != null && a.circles.Count > 0)
                {
                    var d2 = ImGui.GetForegroundDrawList();
                    foreach (var c in a.circles)
                    {
                        var center = new Vector2((float)c.center.X, (float)c.center.Y);
                        float radius = Math.Max(1f, c.radius);

                        uint outline = u32(c.outline);
                        uint fill = u32(Color.FromArgb((byte)(c.fillColor.A), c.fillColor.R, c.fillColor.G, c.fillColor.B));

                        if (c.fill)
                            d2.AddCircleFilled(center, radius, fill, 100);

                        d2.AddCircle(center, radius, outline, 100, 1.5f);
                    }
                }

                var sa = silentaiming.GetSceneSnapshot();
                if (sa != null && sa.circles != null && sa.circles.Count > 0)
                {
                    var d2 = ImGui.GetForegroundDrawList();
                    foreach (var c in sa.circles)
                    {
                        var center = new Vector2((float)c.center.X, (float)c.center.Y);
                        float radius = Math.Max(1f, c.radius);

                        uint outline = u32(c.outline);
                        uint fill = u32(Color.FromArgb((byte)(c.fillColor.A), c.fillColor.R, c.fillColor.G, c.fillColor.B));

                        if (c.fill)
                            d2.AddCircleFilled(center, radius, fill, 100);

                        d2.AddCircle(center, radius, outline, 100, 1.5f);
                    }
                }
            }
            catch { }

            try
            {
                var s = desync.GetSceneSnapshot();
                if (s != null && s.Active)
                {
                    var screen = FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.WorldToScreen(s.Position);

                    string localWorld = "?";
                    try {
                        var lp = FoulzExternal.storage.Storage.LocalPlayerInstance;
                        if (lp.IsValid)
                        {
                            var guys = FoulzExternal.SDK.caches.playerobjects.CachedPlayerObjects;
                            if (guys != null)
                            {
                                var localObj = System.Linq.Enumerable.FirstOrDefault(guys, x => x.address == lp.Address);
                                if (localObj.address != 0)
                                {
                                    if (localObj.HumanoidRootPart.IsValid)
                                    {
                                        var pos = FoulzExternal.games.universal.visuals.visuals.GetPos(localObj.HumanoidRootPart, localCache);
                                        localWorld = $"HRP: {pos.x:0.00}, {pos.y:0.00}, {pos.z:0.00}";
                                    }
                                    else if (localObj.Humanoid.IsValid)
                                    {
                                        var pos = FoulzExternal.games.universal.visuals.visuals.GetPos(localObj.Humanoid, localCache);
                                        localWorld = $"Humanoid: {pos.x:0.00}, {pos.y:0.00}, {pos.z:0.00}";
                                    }
                                    else
                                    {
                                        localWorld = "localObj found, but no valid HRP or Humanoid";
                                    }
                                }
                                else
                                {
                                    localWorld = "localObj not found in playerobjects";
                                }
                            }
                            else
                            {
                                localWorld = "playerobjects.CachedPlayerObjects null";
                            }
                        }
                        else
                        {
                            localWorld = "LocalPlayerInstance not valid";
                        }
                    } catch (Exception ex) { localWorld = $"EX: {ex.Message}"; }
                    FoulzExternal.logging.LogsWindow.Log($"[Desync VIS] World: {s.Position.x:0.00}, {s.Position.y:0.00}, {s.Position.z:0.00} | Screen: {screen.x:0.00}, {screen.y:0.00} | Local: {localWorld}");
                    if (screen.x != -1 && screen.y != -1)
                    {
                        var d3 = ImGui.GetForegroundDrawList();
                        var center = new Vector2(screen.x, screen.y);

                        uint blackFill = 0x60000000;
                        uint whiteOutline = 0xFFFFFFFF;
                        uint whiteThin = 0x80FFFFFF;

                        float radius = 48.0f;

                        d3.AddCircleFilled(center, radius + 6.0f, 0x20000000, 64);
                        d3.AddCircleFilled(center, radius + 2.0f, 0x30000000, 64);
                        d3.AddCircleFilled(center, radius, blackFill, 64);

                        d3.AddCircle(center, radius + 0.5f, whiteOutline, 128, 2.0f);

                        d3.AddCircle(center, radius * 0.6f, whiteThin, 64, 1.0f);

                        float crossLen = 10.0f;
                        d3.AddLine(new Vector2(center.X - crossLen, center.Y), new Vector2(center.X + crossLen, center.Y), whiteOutline, 1.2f);
                        d3.AddLine(new Vector2(center.X, center.Y - crossLen), new Vector2(center.X, center.Y + crossLen), whiteOutline, 1.2f);
                        d3.AddCircleFilled(center, 3.0f, whiteOutline, 12);
                    }
                }
            }
            catch { }
        }

        private static bool is_rbx()
        {
            IntPtr h = GetForegroundWindow();
            if (h == IntPtr.Zero) return false;

            var sb = new StringBuilder(256);
            if (GetWindowTextW(h, sb, 256) > 0 && sb.ToString().Contains("Roblox")) return true;

            GetWindowThreadProcessId(h, out uint pid);
            try { return pid != 0 && Process.GetProcessById((int)pid).ProcessName.Contains("Roblox"); }
            catch { return false; }
        }

        public static void start()
        {
            lock (l)
            {
                if (running) return;
                running = true;
            }

            new Thread(() =>
            {
                try
                {
                    var app = new Program();
                    app.Start();

                    watcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                    try
                    {
                        if (Application.Current != null)
                        {
                            onAppExit = (s, e) => kill();
                            Application.Current.Exit += onAppExit;
                            onDispatcherShutdown = (s, e) => kill();
                            Application.Current.Dispatcher.ShutdownStarted += onDispatcherShutdown;
                        }

                        onProcessExit = (s, e) => kill();
                        AppDomain.CurrentDomain.ProcessExit += onProcessExit;
                    }
                    catch { }

                    new Thread(() =>
                    {
                        IntPtr win = IntPtr.Zero;
                        while (running)
                        {
                            if (win == IntPtr.Zero) win = FindWindow("Overlay", null);
                            if (win != IntPtr.Zero)
                                SetWindowPos(win, new IntPtr(-1), 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0040);

                            Thread.Sleep(200);
                        }
                    })
                    { IsBackground = true }.Start();

                    System.Windows.Threading.Dispatcher.Run();
                }
                catch { }
            })
            { IsBackground = true }.Start();
        }

        public static void kill()
        {
            lock (l)
            {
                if (!running) return;
                running = false;
            }

            try
            {
                try { if (Application.Current != null && onAppExit != null) Application.Current.Exit -= onAppExit; } catch { }
                try { if (Application.Current != null && onDispatcherShutdown != null) Application.Current.Dispatcher.ShutdownStarted -= onDispatcherShutdown; } catch { }
                try { if (onProcessExit != null) AppDomain.CurrentDomain.ProcessExit -= onProcessExit; } catch { }

                if (watcher != null && !watcher.HasShutdownStarted && !watcher.HasShutdownFinished)
                {
                    watcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
                }

                try
                {
                    IntPtr win = FindWindow("Overlay", null);
                    if (win != IntPtr.Zero)
                        PostMessage(win, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                catch { }
            }
            catch { }
        }

        private static uint u32(System.Windows.Media.Color c) => (uint)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);

        public static void Main(string[] args) => new Program().Start().Wait();
    }
}