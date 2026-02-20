using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Offsets;
using FoulzExternal.SDK.structures;
using System.Diagnostics;
using System.IO;

// some things i implemented in my C++ code that I don't actually use in this C# project, like readstringexplorer, it was just an expiermental readstring in VMM.cs, you can delete it if u want

namespace FoulzExternal.SDK
{
    public struct Instance
    {
        public long Address;
        public static Memory Mem;

        public Instance(long address)
        {
            Address = address;
        }

        public bool IsValid => Address > 0x1000;

        private static string ReadStringExplorer(long address)
        {
            if (address == 0) return "";
            try
            {
                int length = Mem.Read<int>(address + 0x18);
                long strPtr = address;
                if (length >= 16)
                {
                    strPtr = Mem.ReadPtr(address);
                }

                var sb = new StringBuilder();
                for (int i = 0; i < length; ++i)
                {
                    byte b = Mem.Read<byte>(strPtr + i);
                    if (b == 0) break;
                    sb.Append((char)b);
                }
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        private static string FetchString(long address)
        {
            if (address == 0) return "";
            try
            {
                int length = Mem.Read<int>(address + 0x18);
                if (length >= 16)
                {
                    long padding = Mem.ReadPtr(address);
                    return Mem.ReadString(padding);
                }
                return Mem.ReadString(address);
            }
            catch
            {
                return ReadStringExplorer(address);
            }
        }

        public static Instance GetVisualEngine()
        {
            return new Instance(Mem.ReadPtr(Mem.Base + Offsets.VisualEngine.Pointer));
        }

        public static Instance GetDataModel()
        {
            long v1 = Mem.ReadPtr(GetVisualEngine().Address + Offsets.VisualEngine.FakeDataModel);
            return new Instance(Mem.ReadPtr(v1 + Offsets.FakeDataModel.RealDataModel));
        }

        public string GetName()
        {
            long nameAddy = Mem.ReadPtr(Address + Offsets.Instance.Name);
            return FetchString(nameAddy);
        }

        public string GetDisplayName()
        {
            long namePtr = Mem.ReadPtr(Address + Offsets.Player.DisplayName);
            if (namePtr != 0)
            {
                string ptrRes = FetchString(namePtr);
                if (!string.IsNullOrEmpty(ptrRes)) return ptrRes;

                string fallback = FetchString(Address + Offsets.Player.DisplayName);
                if (!string.IsNullOrEmpty(fallback)) return fallback;
            }
            return "No DisplayName";
        }

        public string GetClass()
        {
            long classAddress = Mem.ReadPtr(Address + Offsets.Instance.ClassDescriptor);
            if (classAddress == 0) return "";
            long sizeAddress = classAddress + Offsets.Instance.ClassName;
            long classNamePtr = Mem.ReadPtr(sizeAddress);
            return FetchString(classNamePtr);
        }

        public List<Instance> GetChildren()
        {
            List<Instance> children = new List<Instance>();
            long start = Mem.ReadPtr(Address + Offsets.Instance.ChildrenStart);
            if (start == 0) return children;

            long end = Mem.ReadPtr(start + Offsets.Instance.ChildrenEnd);
            for (long entry = Mem.ReadPtr(start); entry < end; entry += 0x10)
            {
                if (entry == 0) break;
                long inst = Mem.ReadPtr(entry);
                if (inst != 0) children.Add(new Instance(inst));
            }
            return children;
        }

        public Instance FindFirstChild(string name)
        {
            return GetChildren().FirstOrDefault(c => c.GetName() == name);
        }

        public Instance FindFirstChildOfClass(string className)
        {
            return GetChildren().FirstOrDefault(c => c.GetClass() == className);
        }

        public Instance GetLocalPlayer()
        {
            return new Instance(Mem.ReadPtr(Address + Offsets.Player.LocalPlayer));
        }

        public long GetUserID() => Mem.Read<long>(Address + Offsets.Player.UserId);
        public long GetPlaceID() => Mem.Read<long>(Address + Offsets.DataModel.PlaceId);
        public long GetGameID() => Mem.Read<long>(Address + Offsets.DataModel.GameId);

        public string GetAttribute(string attrName)
        {
            long attr_container = Mem.ReadPtr(Address + Offsets.Instance.AttributeContainer);
            if (attr_container == 0) return "";
            long attr_list = Mem.ReadPtr(attr_container + Offsets.Instance.AttributeList);
            if (attr_list == 0) return "";

            for (int i = 0x0; i < 0x1000; i += (int)Offsets.Instance.AttributeToNext)
            {
                long namePtr = Mem.ReadPtr(attr_list + i);
                string name = FetchString(namePtr);
                if (string.IsNullOrEmpty(name)) break;

                if (name == attrName)
                {
                    return FetchString(attr_list + i + Offsets.Instance.AttributeToValue);
                }
            }
            return "";
        }

        public T GetAttributeValue<T>(string attrName) where T : struct
        {
            long attr_container = Mem.ReadPtr(Address + Offsets.Instance.AttributeContainer);
            if (attr_container == 0) return default;
            long attr_list = Mem.ReadPtr(attr_container + Offsets.Instance.AttributeList);
            if (attr_list == 0) return default;

            for (int i = 0x0; i < 0x1000; i += (int)Offsets.Instance.AttributeToNext)
            {
                long namePtr = Mem.ReadPtr(attr_list + i);
                string name = FetchString(namePtr);
                if (string.IsNullOrEmpty(name)) break;

                if (name == attrName) return Mem.Read<T>(attr_list + i + Offsets.Instance.AttributeToValue);
            }
            return default;
        }

        public void SetAttributeValue<T>(string attrName, T value) where T : struct
        {
            long attr_container = Mem.ReadPtr(Address + Offsets.Instance.AttributeContainer);
            if (attr_container == 0) return;
            long attr_list = Mem.ReadPtr(attr_container + Offsets.Instance.AttributeList);
            if (attr_list == 0) return;

            for (int i = 0x0; i < 0x1000; i += (int)Offsets.Instance.AttributeToNext)
            {
                long namePtr = Mem.ReadPtr(attr_list + i);
                string name = FetchString(namePtr);
                if (string.IsNullOrEmpty(name)) break;

                if (name == attrName)
                {
                    Mem.Write<T>(attr_list + i + Offsets.Instance.AttributeToValue, value);
                    break;
                }
            }
        }

        public Vector3 GetPosition()
        {
            long prim = Mem.ReadPtr(Address + Offsets.BasePart.Primitive);
            return Mem.Read<Vector3>(prim + Offsets.Primitive.Position);
        }

        public Vector3 GetSize()
        {
            long prim = Mem.ReadPtr(Address + Offsets.BasePart.Primitive);
            return Mem.Read<Vector3>(prim + Offsets.Primitive.Size);
        }

        public sCFrame GetCFrame()
        {
            long prim = Mem.ReadPtr(Address + Offsets.BasePart.Primitive);
            return Mem.Read<sCFrame>(prim + 0x11C);
        }

        public string GetSpecialMeshID()
        {
            string str = ReadStringExplorer(Address + Offsets.SpecialMesh.MeshId);
            Match m = Regex.Match(str, @"(\d{5,})");
            return m.Success ? m.Groups[1].Value : "0";
        }

        public Instance GetCharacter()
        {
            return new Instance(Mem.ReadPtr(Address + Offsets.Player.ModelInstance));
        }

        public Instance GetHumanoid()
        {
            return GetCharacter().FindFirstChildOfClass("Humanoid");
        }

        public float GetHealth() => Mem.Read<float>(GetHumanoid().Address + Offsets.Humanoid.Health);
        public float GetMaxHealth() => Mem.Read<float>(GetHumanoid().Address + Offsets.Humanoid.MaxHealth);

        public void SetWalkspeed(float v)
        {
            long h = GetHumanoid().Address;
            float val = (float)Math.Round(v);
            Mem.Write(h + Offsets.Humanoid.WalkspeedCheck, val);
            Mem.Write(h + Offsets.Humanoid.Walkspeed, val);
        }

        public void SetJumpPower(float v)
        {
            Mem.Write(GetHumanoid().Address + Offsets.Humanoid.JumpPower, (float)Math.Round(v));
        }

        public float GetWalkspeed() => (float)Math.Round(Mem.Read<float>(GetHumanoid().Address + Offsets.Humanoid.Walkspeed));
        public float GetJumpPower() => (float)Math.Round(Mem.Read<float>(GetHumanoid().Address + Offsets.Humanoid.JumpPower));

        public float GetFOV()
        {
            float rad = Mem.Read<float>(Address + Offsets.Camera.FieldOfView);
            return (float)Math.Round(rad * 180.0f / 3.1415926535f);
        }

        public void SetFOV(float v)
        {
            float rad = (float)Math.Round(v) * 3.1415926535f / 180.0f;
            Mem.Write(Address + Offsets.Camera.FieldOfView, rad);
        }

        public static string FindRobloxVersion()
        {
            try
            {
                Process[] all = Process.GetProcesses();
                foreach (var p in all)
                {
                    try
                    {
                        var module = p.MainModule;
                        if (module == null) continue;
                        if (Mem != null && Mem.Base != 0 && module.BaseAddress.ToInt64() == Mem.Base)
                        {
                            string dir = Path.GetDirectoryName(module.FileName);
                            if (string.IsNullOrEmpty(dir)) return "";
                            return Path.GetFileName(dir);
                        }
                    }
                    catch { }
                }

                string[] names = new[] { "RobloxPlayerBeta", "RobloxPlayer", "Roblox" };
                foreach (var name in names)
                {
                    var ps = Process.GetProcessesByName(name);
                    if (ps.Length == 0) continue;
                    try
                    {
                        var mod = ps[0].MainModule;
                        string dir = Path.GetDirectoryName(mod.FileName);
                        if (string.IsNullOrEmpty(dir)) return "";
                        return Path.GetFileName(dir);
                    }
                    catch { }
                }
            }
            catch { }
            return "";
        }

        public static double GetRenderPing()
        {
            try
            {
                var dm = GetDataModel();
                if (!dm.IsValid) return -1.0;

                var stats = dm.FindFirstChild("Stats");
                if (!stats.IsValid) return -1.0;

                var perf = stats.FindFirstChild("PerformanceStats");
                if (!perf.IsValid) return -1.0;

                var pingInst = perf.FindFirstChild("Ping");
                if (!pingInst.IsValid) return -1.0;

                return Mem.Read<double>(pingInst.Address + Offsets.StatsItem.Value);
            }
            catch
            {
                return -1.0;
            }
        }
    }
}