using System;
using System.Threading;
using FoulzExternal.SDK;
using FoulzExternal.SDK.structures;
using FoulzExternal.SDK.caches;
using FoulzExternal.storage;
using Offsets;
using Options;
using System.Windows.Input;


// ts don't work, i'll fix and update soon when I got the time

namespace FoulzExternal.features.games.universal.flight
{
    internal static class flight
    {
        private static bool running = false;
        private static Thread thread;
        private static readonly object locker = new();

        public static void Start()
        {
            lock (locker)
            {
                if (running) return;
                running = true;
                thread = new Thread(start_flight) { IsBackground = true };
                thread.Start();
            }
        }

        public static void Stop()
        {
            lock (locker)
            {
                running = false;
                thread?.Join();
            }
        }

        private static void start_flight()
        {
            while (running)
            {
                try
                {
                    if (!Options.Settings.Flight.VFlight)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    if (!Options.Settings.Flight.VFlightBind.IsPressed())
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    var lp = Storage.LocalPlayerInstance;
                    if (!lp.IsValid)
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                    var chr = lp.GetCharacter();
                    if (!chr.IsValid)
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                    var hrp = chr.FindFirstChild("HumanoidRootPart");
                    if (!hrp.IsValid)
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                    var camera = Storage.WorkspaceInstance.FindFirstChild("Camera");
                    if (!camera.IsValid)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    long physics_addr = hrp.Address;
                    if (physics_addr == 0)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    try
                    {
                        var rotation_matrix = SDK.Instance.Mem.Read<Matrix3x3>(camera.Address + Offsets.Camera.Rotation);
                        var look_vector = new Vector3 { x = rotation_matrix.r02, y = rotation_matrix.r12, z = rotation_matrix.r22 };
                        var right_vector = new Vector3 { x = rotation_matrix.r00, y = rotation_matrix.r10, z = rotation_matrix.r20 };

                        Vector3 direction = new Vector3 { x = 0f, y = 0f, z = 0f };
                        bool trying_to_move = false;

                        if (IsKeyDown(Key.W)) { direction = direction - look_vector; trying_to_move = true; }
                        if (IsKeyDown(Key.S)) { direction = direction + look_vector; trying_to_move = true; }
                        if (IsKeyDown(Key.A)) { direction = direction - right_vector; trying_to_move = true; }
                        if (IsKeyDown(Key.D)) { direction = direction + right_vector; trying_to_move = true; }

                        if (direction.Magnitude() > 0.01f)
                            direction = direction.Normalize();

                        float speed = Options.Settings.Flight.VFlightSpeed;
                        if (trying_to_move)
                            SDK.Instance.Mem.Write<Vector3>(physics_addr + Offsets.Primitive.AssemblyLinearVelocity, direction * speed);
                        else
                            SDK.Instance.Mem.Write<Vector3>(physics_addr + Offsets.Primitive.AssemblyLinearVelocity, new Vector3 { x = 0f, y = 0f, z = 0f });
                    }
                    catch { }
                }
                catch { }
                Thread.Sleep(10);
            }
        }

        private static bool IsKeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
    }
}
