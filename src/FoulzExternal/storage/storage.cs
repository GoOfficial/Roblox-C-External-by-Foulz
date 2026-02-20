using System;
using Offsets;
using FoulzExternal.logging;
using System.Diagnostics;
using System.IO;
using FoulzExternal.SDK;
using SInstance = FoulzExternal.SDK.Instance;

// i stored a lot of shit to make it easier for you, you're welcome

namespace FoulzExternal.storage
{
    internal static class Storage
    {
        public static long BaseAddress { get; private set; }
        public static long VisualEngine { get; private set; }
        public static long DataModel { get; private set; }
        public static long ModuleSize { get; private set; }
        public static int ProcessId { get; private set; }
        public static string RobloxVersion { get; private set; }
        public static SInstance VisualEngineInstance { get; private set; }
        public static SInstance DataModelInstance { get; private set; }
        public static SInstance WorkspaceInstance { get; private set; }
        public static SInstance PlayersInstance { get; private set; }
        public static SInstance CameraInstance { get; private set; }
        public static SInstance LocalPlayerInstance { get; private set; }
        public static string LocalPlayerName { get; private set; }
        public static long LocalPlayerUserId { get; private set; }
        public static long PlaceId { get; private set; }
        public static long GameId { get; private set; }
        public static bool IsInitialized => BaseAddress != 0 && VisualEngine != 0 && DataModel != 0;


        private static long _lastLoggedBaseAddress = 0;
        private static long _lastLoggedDataModel = 0;

        public static void Initialize(FoulzExternal.Memory m)
        {
            if (m == null) return;

            BaseAddress = m.Base;

            try
            {
                SInstance.Mem = m;
                VisualEngineInstance = SInstance.GetVisualEngine();
                VisualEngine = VisualEngineInstance.Address;

                if (VisualEngineInstance.IsValid)
                {
                    DataModelInstance = SInstance.GetDataModel();
                    DataModel = DataModelInstance.Address;
                }
                else
                {
                    DataModelInstance = new SInstance(0);
                    DataModel = 0;
                }

                bool hasdm = DataModelInstance.IsValid;
                WorkspaceInstance = hasdm ? DataModelInstance.FindFirstChildOfClass("Workspace") : new SInstance(0);
                PlayersInstance = hasdm ? DataModelInstance.FindFirstChildOfClass("Players") : new SInstance(0);
                CameraInstance = WorkspaceInstance.IsValid ? WorkspaceInstance.FindFirstChild("Camera") : new SInstance(0);
                LocalPlayerInstance = PlayersInstance.IsValid ? PlayersInstance.GetLocalPlayer() : new SInstance(0);

                try { LocalPlayerName = LocalPlayerInstance.IsValid ? LocalPlayerInstance.GetDisplayName() : ""; } catch { LocalPlayerName = ""; }
                try { LocalPlayerUserId = LocalPlayerInstance.IsValid ? m.Read<long>(LocalPlayerInstance.Address + Offsets.Player.UserId) : 0; } catch { LocalPlayerUserId = 0; }


                try
                {
                    if (DataModelInstance.IsValid)
                    {
                        try { PlaceId = DataModelInstance.GetPlaceID(); } catch { PlaceId = 0; }
                        try { GameId = DataModelInstance.GetGameID(); } catch { GameId = 0; }
                    }
                    else
                    {
                        PlaceId = 0;
                        GameId = 0;
                    }
                }
                catch { PlaceId = 0; GameId = 0; }

                ModuleSize = 0;
                ProcessId = 0;
                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        var main = p.MainModule;
                        if (main != null && main.BaseAddress.ToInt64() == BaseAddress)
                        {
                            ModuleSize = main.ModuleMemorySize;
                            ProcessId = p.Id;
                            break;
                        }
                    }
                    catch { }
                }

                try
                {
                    string ver = SInstance.FindRobloxVersion() ?? "";
                    RobloxVersion = ver.StartsWith("version-", StringComparison.OrdinalIgnoreCase) ? ver.Substring(8) : ver;
                }
                catch { RobloxVersion = ""; }

                try
                {
                    if (BaseAddress != _lastLoggedBaseAddress || DataModel != _lastLoggedDataModel)
                    {
                        LogsWindow.Log("[storage] PID: {0}", ProcessId);
                        LogsWindow.Log("[storage] Base Address: 0x{0:X16}", BaseAddress);
                        LogsWindow.Log("[storage] Visual Engine: 0x{0:X16}", VisualEngine);
                        LogsWindow.Log("[storage] DataModel: 0x{0:X16}", DataModel);
                        LogsWindow.Log("[storage] ModuleSize: 0x{0:X16}", ModuleSize);
                        LogsWindow.Log("[storage] RBX Version: {0}", string.IsNullOrEmpty(RobloxVersion) ? "???" : RobloxVersion);
                        LogsWindow.Log("[storage] Workspace: 0x{0:X16}", WorkspaceInstance.Address);
                        LogsWindow.Log("[storage] Players: 0x{0:X16}", PlayersInstance.Address);
                        LogsWindow.Log("[storage] Camera: 0x{0:X16}", CameraInstance.Address);
                        LogsWindow.Log("[storage] LocalPlayer: 0x{0:X16}", LocalPlayerInstance.Address);
                        LogsWindow.Log("[storage] Character: {0}", string.IsNullOrEmpty(LocalPlayerName) ? "???" : LocalPlayerName);
                        LogsWindow.Log("[storage] CharacterUID: {0}", LocalPlayerUserId);
                        LogsWindow.Log("[storage] PlaceId: {0}", PlaceId);
                        LogsWindow.Log("[storage] GameId: {0}", GameId);

                        _lastLoggedBaseAddress = BaseAddress;
                        _lastLoggedDataModel = DataModel;
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                VisualEngine = DataModel = 0;
                VisualEngineInstance = DataModelInstance = WorkspaceInstance = PlayersInstance = CameraInstance = LocalPlayerInstance = new SInstance(0);
                LocalPlayerName = "";
                LocalPlayerUserId = ProcessId = 0;
                PlaceId = GameId = 0;
                LogsWindow.Log("[storage] rip: {0}", ex.Message);
            }
        }

        public static void Refresh(FoulzExternal.Memory m) => Initialize(m);

        public static void Clear()
        {
            BaseAddress = 0;
            VisualEngine = 0;
            DataModel = 0;
            ModuleSize = 0;
            ProcessId = 0;
            RobloxVersion = "";

            VisualEngineInstance = DataModelInstance = WorkspaceInstance = PlayersInstance = CameraInstance = LocalPlayerInstance = new SInstance(0);
            LocalPlayerName = "";
            LocalPlayerUserId = 0;
            PlaceId = 0;
            GameId = 0;

            _lastLoggedBaseAddress = 0;
            _lastLoggedDataModel = 0;

            LogsWindow.Log("[storage] cleared.");
        }
    }
}