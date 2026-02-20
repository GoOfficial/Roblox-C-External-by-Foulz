using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FoulzExternal.SDK;
using FoulzExternal.storage;
using FoulzExternal.SDK.gamedetector;
using Offsets;

namespace FoulzExternal.SDK.caches
{
    public struct RobloxPlayer
    {
        public long address;
        public string Name;
        public Instance Team;
        public Instance Character;
        public Instance Humanoid;
        public float Health;
        public float MaxHealth;
        public int RigType;
        public Instance Head;
        public Instance HumanoidRootPart;
        public Instance Left_Arm;
        public Instance Left_Leg;
        public Instance Right_Arm;
        public Instance Right_Leg;
        public Instance Torso;
        public Instance Upper_Torso;
        public Instance Lower_Torso;
        public Instance Right_Upper_Arm;
        public Instance Right_Lower_Arm;
        public Instance Right_Hand;
        public Instance Left_Upper_Arm;
        public Instance Left_Lower_Arm;
        public Instance Left_Hand;
        public Instance Right_Upper_Leg;
        public Instance Right_Lower_Leg;
        public Instance Right_Foot;
        public Instance Left_Upper_Leg;
        public Instance Left_Lower_Leg;
        public Instance Left_Foot;
        public Instance TeammateLabel;
    }

    internal static class playerobjects
    {
        public static List<RobloxPlayer> CachedPlayerObjects { get; private set; } = new List<RobloxPlayer>();
        private static Thread _tid;
        private static bool _vibin = false;
        private static readonly object _sync = new object();

        public static void Start()
        {
            if (_vibin) return;
            _vibin = true;
            _tid = new Thread(loop_bro) { IsBackground = true };
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
            lock (_sync)
            {
                CachedPlayerObjects.Clear();
            }
        }

        private static void loop_bro()
        {
            var list_cuh = new List<RobloxPlayer>();

            while (_vibin)
            {
                try
                {
                    list_cuh.Clear();
                    var plrs = player.CachedPlayers.ToList();
                    var currentGame = finder.whatgame();

                    foreach (var inst in plrs)
                    {
                        if (!inst.IsValid) continue;

                        var info = new RobloxPlayer();
                        info.address = inst.Address;
                        info.Name = inst.GetName();

                        try { info.Team = new Instance(Instance.Mem.ReadPtr(inst.Address + Offsets.Player.Team)); } catch { }

                        info.Character = inst.GetCharacter();
                        if (!info.Character.IsValid) continue;

                        info.Humanoid = info.Character.FindFirstChildOfClass("Humanoid");
                        if (!info.Humanoid.IsValid) continue;

                        try
                        {
                            info.Health = Instance.Mem.Read<float>(info.Humanoid.Address + Offsets.Humanoid.Health);
                            info.MaxHealth = Instance.Mem.Read<float>(info.Humanoid.Address + Offsets.Humanoid.MaxHealth);
                            info.RigType = Instance.Mem.Read<int>(info.Humanoid.Address + Offsets.Humanoid.RigType);
                        }
                        catch { info.Health = 100f; info.MaxHealth = 100f; info.RigType = 0; }

                        var c = info.Character;
                        info.Head = c.FindFirstChild("Head");
                        info.HumanoidRootPart = c.FindFirstChild("HumanoidRootPart");

                        if (currentGame == GameType.rivals && info.HumanoidRootPart.IsValid)
                        {
                            info.TeammateLabel = info.HumanoidRootPart.FindFirstChild("TeammateLabel");
                        }

                        if (!info.Head.IsValid) continue;

                        if (info.RigType == 0)
                        {
                            info.Left_Arm = c.FindFirstChild("Left Arm");
                            info.Left_Leg = c.FindFirstChild("Left Leg");
                            info.Right_Arm = c.FindFirstChild("Right Arm");
                            info.Right_Leg = c.FindFirstChild("Right Leg");
                            info.Torso = c.FindFirstChild("Torso");
                        }
                        else
                        {
                            info.Upper_Torso = c.FindFirstChild("UpperTorso");
                            info.Lower_Torso = c.FindFirstChild("LowerTorso");
                            info.Right_Upper_Arm = c.FindFirstChild("RightUpperArm");
                            info.Right_Lower_Arm = c.FindFirstChild("RightLowerArm");
                            info.Right_Hand = c.FindFirstChild("RightHand");
                            info.Left_Upper_Arm = c.FindFirstChild("LeftUpperArm");
                            info.Left_Lower_Arm = c.FindFirstChild("LeftLowerArm");
                            info.Left_Hand = c.FindFirstChild("LeftHand");
                            info.Right_Upper_Leg = c.FindFirstChild("RightUpperLeg");
                            info.Right_Lower_Leg = c.FindFirstChild("RightLowerLeg");
                            info.Right_Foot = c.FindFirstChild("RightFoot");
                            info.Left_Upper_Leg = c.FindFirstChild("LeftUpperLeg");
                            info.Left_Lower_Leg = c.FindFirstChild("LeftLowerLeg");
                            info.Left_Foot = c.FindFirstChild("LeftFoot");
                        }

                        list_cuh.Add(info);
                    }

                    lock (_sync)
                    {
                        CachedPlayerObjects = new List<RobloxPlayer>(list_cuh);
                    }
                }
                catch { }
                Thread.Sleep(500);
            }
        }
    }
}