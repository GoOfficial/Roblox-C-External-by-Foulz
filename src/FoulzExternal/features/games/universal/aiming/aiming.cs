using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using FoulzExternal.SDK;
using FoulzExternal.SDK.caches;
using FoulzExternal.storage;
using Offsets;
using Options;
using SDKInstance = FoulzExternal.SDK.Instance;
using Point = System.Windows.Point;
using FoulzExternal.SDK.structures;
using FoulzExternal.features.games.universal.checks.teamcheck;
using FoulzExternal.features.games.universal.checks.downedcheck;
using FoulzExternal.features.games.universal.checks.transparencycheck;

namespace FoulzExternal.games.universal.aiming
{
    public static class aiming
    {
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")] private static extern bool get_pos(out FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT p);
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")] private static extern bool set_pos(int x, int y);
        [DllImport("user32.dll", EntryPoint = "mouse_event")] private static extern void mouse_go(uint f, int dx, int dy, uint d, int e);

        private static bool vibing = false;
        private static Thread brain;
        private static readonly object safety = new();
        private static Scene view = new();
        private static RobloxPlayer locked;
        private static RobloxPlayer current;
        private static bool is_on = false;
        private static bool old_key = false;
        private static bool was_active = false;

        private static float acc_x = 0.0f;
        private static float acc_y = 0.0f;

        public struct FOVCircle { public Point center; public float radius; public Color outline; public Color fillColor; public int type; public bool fill; }
        public class Scene { public List<FOVCircle> circles = new(); }

        public static void Start()
        {
            if (vibing) return;
            vibing = true;
            brain = new Thread(go_crazy) { IsBackground = true };
            brain.Start();
        }

        public static void Stop() => vibing = false;

        public static Scene GetSceneSnapshot()
        {
            lock (safety) return new Scene { circles = new List<FOVCircle>(view.circles) };
        }

        private static Vector3 get_bone_pos(RobloxPlayer p, int bone, Dictionary<long, long> cache)
        {
            bool r15 = p.RigType == 1;
            SDKInstance part = new SDKInstance(0);

            switch (bone)
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

        private static RobloxPlayer find_victim(Aiming s, FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT m, Dictionary<long, long> cache)
        {
            RobloxPlayer best = default;
            float closest = float.MaxValue;

            var lp = Storage.LocalPlayerInstance;
            if (!lp.IsValid) return best;

            var targets = playerobjects.CachedPlayerObjects;
            if (targets == null) return best;

            foreach (var p in targets)
            {
                if (p.address == 0 || p.address == lp.Address || p.Health <= 0) continue;
                if (Settings.Checks.TeamCheck && TeamCheck.isteammate(p)) continue;
                if (Settings.Checks.DownedCheck && DownedCheck.is_downed(p)) continue;
                if (Settings.Checks.TransparencyCheck && TransparencyCheck.is_clear(p)) continue;

                var worldPos = get_bone_pos(p, s.TargetBone, cache);
                var screenPos = FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.WorldToScreen(worldPos);
                if (screenPos.x == -1) continue;

                bool inRange = false;
                try
                {
                    var lChar = Storage.LocalPlayerInstance.GetCharacter();
                    var lRoot = lChar.IsValid ? lChar.FindFirstChild("HumanoidRootPart") : new SDKInstance(0);
                    Vector3 lPos = new Vector3();
                    if (lRoot.IsValid)
                    {
                        long lptr = SDKInstance.Mem.ReadPtr(lRoot.Address + Offsets.BasePart.Primitive);
                        if (lptr != 0) lPos = SDKInstance.Mem.Read<Vector3>(lptr + Offsets.Primitive.Position);
                    }

                    float dx = worldPos.x - lPos.x;
                    float dy = worldPos.y - lPos.y;
                    float dz = worldPos.z - lPos.z;
                    if ((float)Math.Sqrt(dx * dx + dy * dy + dz * dz) <= s.Range) inRange = true;
                }
                catch { }

                if (!inRange) continue;

                float dist = (float)Math.Sqrt(Math.Pow(screenPos.x - m.x, 2) + Math.Pow(screenPos.y - m.y, 2));
                if (dist < closest && dist <= s.FOV)
                {
                    closest = dist;
                    best = p;
                }
            }
            return best;
        }

        private static Vector3 get_pred(RobloxPlayer p, Aiming s, Dictionary<long, long> cache)
        {
            var pos = get_bone_pos(p, s.TargetBone, cache);
            if (s.Prediction)
            {
                var part = p.HumanoidRootPart.IsValid ? p.HumanoidRootPart : p.Head;
                long prim = SDKInstance.Mem.ReadPtr(part.Address + Offsets.BasePart.Primitive);
                if (prim != 0)
                {
                    var vel = SDKInstance.Mem.Read<Vector3>(prim + Offsets.Primitive.AssemblyLinearVelocity);
                    float px = s.PredictionX != 0 ? (2.1f - s.PredictionX) : 0.0f;
                    float py = s.PredictionY != 0 ? (2.1f - s.PredictionY) : 0.0f;

                    pos.x += s.PredictionX != 0 ? vel.x * px : 0.0f;
                    pos.y += s.PredictionY != 0 ? vel.y * py : 0.0f;
                    pos.z += s.PredictionX != 0 ? vel.z * px : 0.0f;
                }
            }
            return pos;
        }

        private static void move_rat(FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT target, FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT current, Aiming s)
        {
            if (current.x == target.x && current.y == target.y) return;

            float dx = (float)(target.x - current.x);
            float dy = (float)(target.y - current.y);

            float smooth = s.Smoothness ? s.SmoothnessY : 0.05f;
            float t = Math.Clamp(1.0f - smooth, 0.01f, 1.0f);

            float sens = 1.0f;
            try { sens = SDKInstance.Mem.Read<float>(SDKInstance.Mem.Base + Offsets.MouseService.SensitivityPointer); } catch { }
            if (sens <= 0.0f) sens = 1.0f;

            float scale = s.Sensitivity / (sens + 0.2f);
            float speed = 0.2f;

            acc_x += dx * t * scale * speed;
            acc_y += dy * t * scale * speed;

            int mx = (int)acc_x;
            int my = (int)acc_y;

            if (Math.Abs(dx) < 1.0f && Math.Abs(dy) < 1.0f) { acc_x = 0; acc_y = 0; return; }

            acc_x -= mx; acc_y -= my;

            if (mx != 0 || my != 0) mouse_go(0x0001, mx, my, 0, 0);
        }

        private static void lock_on(RobloxPlayer t, Aiming s, FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT m, Dictionary<long, long> cache)
        {
            var predPos = get_pred(t, s, cache);
            var screen = FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.WorldToScreen(predPos);
            if (screen.x == -1) return;

            if (s.AimingType == 0) spin_cam(predPos, s);
            else move_rat(new FoulzExternal.SDK.worldtoscreen.WorldToScreenHelper.POINT { x = (int)screen.x, y = (int)screen.y }, m, s);
        }

        private static void go_crazy()
        {
            IntPtr window = IntPtr.Zero;
            var cache = new Dictionary<long, long>();

            while (vibing)
            {
                try
                {
                    if (SDKInstance.Mem == null) { Thread.Sleep(50); continue; }
                    window = (window == IntPtr.Zero || !junk.IsWindow(window)) ? junk.FindWindow(null, "Roblox") : window;
                    if (window == IntPtr.Zero) { Thread.Sleep(200); continue; }

                    var s = Settings.Aiming;
                    bool key = s.AimbotKey.IsPressed();
                    if (s.ToggleType == 1 && key && !old_key) is_on = !is_on;
                    old_key = key;

                    bool active = s.Aimbot && (s.ToggleType == 1 ? is_on : key);

                    get_pos(out var mouse);
                    var next = new Scene();

                    Color outl = Colors.White;
                    Color fill = Color.FromArgb(128, 255, 255, 255);
                    if (s.AnimatedFOV)
                    {
                        float t = (float)Environment.TickCount / 1000f;
                        outl = get_rainbow(t);
                        fill = Color.FromArgb(50, outl.R, outl.G, outl.B);
                    }

                    if (s.ShowFOV)
                        next.circles.Add(new FOVCircle { center = new Point(mouse.x, mouse.y), radius = s.FOV, outline = outl, fillColor = fill, fill = s.FillFOV });

                    if (!active)
                    {
                        locked = default; current = default; was_active = false;
                        lock (safety) view = next;
                        Thread.Sleep(10);
                        continue;
                    }

                    bool fresh = false;
                    if (!was_active || locked.address == 0 || locked.Health == 0 || (Settings.Checks.DownedCheck && DownedCheck.is_downed(locked)) || (Settings.Checks.TransparencyCheck && TransparencyCheck.is_clear(locked)))
                        fresh = true;

                    if (fresh) locked = find_victim(s, mouse, cache);

                    if (s.StickyAim)
                    {
                        if (current.address == 0 || current.Health == 0 || (Settings.Checks.DownedCheck && DownedCheck.is_downed(current)) || (Settings.Checks.TransparencyCheck && TransparencyCheck.is_clear(current)))
                            current = find_victim(s, mouse, cache);
                        locked = current.address != 0 ? current : locked;
                    }

                    if (locked.address != 0) lock_on(locked, s, mouse, cache);

                    was_active = true;
                    lock (safety) view = next;
                }
                catch { }
                Thread.Sleep(5);
            }
        }

        private static void spin_cam(Vector3 target, Aiming s)
        {
            long cam = Storage.CameraInstance.Address;
            var curRot = SDKInstance.Mem.Read<Matrix3x3>(cam + Offsets.Camera.Rotation);
            var camPos = SDKInstance.Mem.Read<Vector3>(cam + Offsets.Camera.Position);
            var lookAt = sCFrame.LookAt(camPos, target, new Vector3 { x = 0, y = 1, z = 0 });

            Vector3 r = lookAt.RightVector; Vector3 u = lookAt.UpVector; Vector3 l = lookAt.LookVector;
            Matrix3x3 targetMat = new Matrix3x3 { r00 = r.x, r01 = u.x, r02 = l.x, r10 = r.y, r11 = u.y, r12 = l.y, r20 = r.z, r21 = u.z, r22 = l.z };

            Vector4 curQ = Vector4.FromMatrix(curRot);
            Vector4 tarQ = Vector4.FromMatrix(targetMat);

            float t = 1.0f;
            if (s.Smoothness)
            {
                float slow = Math.Clamp((s.SmoothnessX + s.SmoothnessY) * 0.5f, 0.0f, 0.99f);
                t = (float)Math.Pow(1.0f - slow, 4);
                if (slow > 0 && t < 0.01f) t = 0.01f;
            }

            SDKInstance.Mem.Write(cam + Offsets.Camera.Rotation, Vector4.Slerp(curQ, tarQ, t).ToMatrix());
        }

        private static Color get_rainbow(float t)
        {
            byte r = (byte)((Math.Sin(t) * 0.5 + 0.5) * 255);
            byte g = (byte)((Math.Sin(t + 2.094) * 0.5 + 0.5) * 255);
            byte b = (byte)((Math.Sin(t + 4.188) * 0.5 + 0.5) * 255);
            return Color.FromRgb(r, g, b);
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

        private static float get_clear(SDKInstance i) => i.IsValid ? SDKInstance.Mem.Read<float>(i.Address + 0x6C) : 0f;
    }

    internal static class junk
    {
        [DllImport("user32.dll")] public static extern IntPtr FindWindow(string c, string n);
        [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
    }
}