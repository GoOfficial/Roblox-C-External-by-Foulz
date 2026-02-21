using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoulzExternal.helpers.keybind;
using Options;

namespace FoulzExternal.config
{
    public class ConfigData
    {
        public HumanoidConfig Humanoid { get; set; } = new();
        public CameraConfig Camera { get; set; } = new();
        public VisualsConfig Visuals { get; set; } = new();
        public AimingConfig Aiming { get; set; } = new();
        public SilentConfig Silent { get; set; } = new();
        public ChecksConfig Checks { get; set; } = new();
        public NetworkConfig Network { get; set; } = new();
        public FlightConfig Flight { get; set; } = new();
    }

    public class HumanoidConfig
    {
        public bool WalkspeedEnabled { get; set; }
        public bool JumpPowerEnabled { get; set; }
        public float Walkspeed { get; set; } = 16f;
        public float JumpPower { get; set; } = 50f;
    }

    public class CameraConfig
    {
        public bool FOVEnabled { get; set; }
        public float FOV { get; set; } = 70f;
    }

    public class VisualsConfig
    {
        public bool BoxESP { get; set; }
        public bool FilledBox { get; set; }
        public bool Box { get; set; }
        public bool BoxFill { get; set; }
        public bool Tracers { get; set; }
        public bool Skeleton { get; set; }
        public bool Name { get; set; }
        public bool Distance { get; set; }
        public bool Health { get; set; }
        public bool ESP3D { get; set; }
        public bool HeadCircle { get; set; }
        public bool CornerESP { get; set; }
        public bool RemoveBorders { get; set; }
        public bool ChinaHat { get; set; }
        public bool LocalPlayerESP { get; set; }
        public int TracersStart { get; set; }
        public float NameSize { get; set; } = 12f;
        public float DistanceSize { get; set; } = 15f;
        public float TracerThickness { get; set; } = 1.5f;
        public float HeadCircleMaxScale { get; set; } = 2.5f;
    }

    public class AimingConfig
    {
        public KeyBindConfig AimbotKey { get; set; } = new("Aimbot");
        public int AimingType { get; set; }
        public int ToggleType { get; set; }
        public bool Aimbot { get; set; }
        public bool StickyAim { get; set; }
        public float Sensitivity { get; set; } = 1.0f;
        public bool Smoothness { get; set; }
        public float SmoothnessX { get; set; }
        public float SmoothnessY { get; set; } = 0.05f;
        public bool Prediction { get; set; }
        public float PredictionY { get; set; } = 2f;
        public float PredictionX { get; set; } = 2f;
        public float FOV { get; set; } = 100f;
        public bool ShowFOV { get; set; }
        public bool FillFOV { get; set; }
        public bool AnimatedFOV { get; set; }
        public float Range { get; set; } = 100f;
        public int TargetBone { get; set; }
    }

    public class SilentConfig
    {
        public KeyBindConfig SilentAimbotKey { get; set; } = new("SilentAimbotKey");
        public bool SilentAimbot { get; set; }
        public bool AlwaysOn { get; set; }
        public bool SilentVisualizer { get; set; }
        public bool ShowSilentFOV { get; set; }
        public bool SPrediction { get; set; }
        public float SilentFOV { get; set; } = 100f;
        public float PredictionY { get; set; } = 2f;
        public float PredictionX { get; set; } = 2f;
        public float SFOV { get; set; } = 150f;
    }

    public class ChecksConfig
    {
        public bool TeamCheck { get; set; }
        public bool DownedCheck { get; set; }
        public bool TransparencyCheck { get; set; }
        public bool WallCheck { get; set; }
    }

    public class NetworkConfig
    {
        public KeyBindConfig DeSyncBind { get; set; } = new("DeSyncBind");
        public bool DeSync { get; set; }
        public bool DeSyncVisualizer { get; set; }
    }

    public class FlightConfig
    {
        public KeyBindConfig VFlightBind { get; set; } = new("VFlightBind");
        public bool VFlight { get; set; }
        public float VFlightSpeed { get; set; } = 1.5f;
    }

    public class KeyBindConfig
    {
        public int Key { get; set; }
        public int MouseButton { get; set; } = -1;
        public int ControllerButton { get; set; } = -1;
        public bool Waiting { get; set; }
        public string Label { get; set; } = string.Empty;

        public KeyBindConfig() { }
        public KeyBindConfig(string l) => Label = l ?? "";
    }

    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        private static string BasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FoulzExternal");
        private static string DefaultFilePath => Path.Combine(BasePath, "default_config.txt");

        public static string GetConfigDirectory()
        {
            string dir = Path.Combine(BasePath, "configs");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetDefaultConfigName()
        {
            try { return File.Exists(DefaultFilePath) ? File.ReadAllText(DefaultFilePath).Trim() : null; }
            catch { return null; }
        }

        public static bool SetDefaultConfigName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) { if (File.Exists(DefaultFilePath)) File.Delete(DefaultFilePath); return true; }
                File.WriteAllText(DefaultFilePath, name.Trim());
                return true;
            }
            catch { return false; }
        }

        public static bool SaveConfig(string name)
        {
            try
            {
                var path = Path.Combine(GetConfigDirectory(), name.EndsWith(".cfg") ? name : name + ".cfg");
                File.WriteAllText(path, JsonSerializer.Serialize(Export(), options));
                return true;
            }
            catch { return false; }
        }

        public static bool LoadConfig(string name)
        {
            try
            {
                var path = Path.Combine(GetConfigDirectory(), name.EndsWith(".cfg") ? name : name + ".cfg");
                if (!File.Exists(path)) return false;
                var data = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(path), options);
                if (data == null) return false;
                Import(data);
                return true;
            }
            catch { return false; }
        }

        public static bool LoadDefaultConfig()
        {
            string def = GetDefaultConfigName();
            return !string.IsNullOrEmpty(def) && LoadConfig(def);
        }

        public static void ResetToDefaults() => Import(new ConfigData());

        public static string[] GetAvailableConfigs()
        {
            try
            {
                var dir = GetConfigDirectory();
                var files = Directory.GetFiles(dir, "*.cfg");
                for (int i = 0; i < files.Length; i++) files[i] = Path.GetFileNameWithoutExtension(files[i]);
                return files;
            }
            catch { return Array.Empty<string>(); }
        }

        public static bool DeleteConfig(string name)
        {
            try
            {
                var path = Path.Combine(GetConfigDirectory(), name.EndsWith(".cfg") ? name : name + ".cfg");
                if (File.Exists(path)) File.Delete(path);
                return true;
            }
            catch { return false; }
        }

        private static ConfigData Export()
        {
            return new ConfigData
            {
                Humanoid = new() { WalkspeedEnabled = Settings.Humanoid.WalkspeedEnabled, JumpPowerEnabled = Settings.Humanoid.JumpPowerEnabled, Walkspeed = Settings.Humanoid.Walkspeed, JumpPower = Settings.Humanoid.JumpPower },
                Camera = new() { FOVEnabled = Settings.Camera.FOVEnabled, FOV = Settings.Camera.FOV },
                Visuals = new()
                {
                    BoxESP = Settings.Visuals.BoxESP,
                    FilledBox = Settings.Visuals.FilledBox,
                    Box = Settings.Visuals.Box,
                    BoxFill = Settings.Visuals.BoxFill,
                    Tracers = Settings.Visuals.Tracers,
                    Skeleton = Settings.Visuals.Skeleton,
                    Name = Settings.Visuals.Name,
                    Distance = Settings.Visuals.Distance,
                    Health = Settings.Visuals.Health,
                    ESP3D = Settings.Visuals.ESP3D,
                    HeadCircle = Settings.Visuals.HeadCircle,
                    CornerESP = Settings.Visuals.CornerESP,
                    RemoveBorders = Settings.Visuals.RemoveBorders,
                    ChinaHat = Settings.Visuals.ChinaHat,
                    LocalPlayerESP = Settings.Visuals.LocalPlayerESP,
                    TracersStart = Settings.Visuals.TracersStart,
                    NameSize = Settings.Visuals.NameSize,
                    DistanceSize = Settings.Visuals.DistanceSize,
                    TracerThickness = Settings.Visuals.TracerThickness,
                    HeadCircleMaxScale = Settings.Visuals.HeadCircleMaxScale
                },
                Aiming = new()
                {
                    AimbotKey = new() { Key = Settings.Aiming.AimbotKey.Key, MouseButton = Settings.Aiming.AimbotKey.MouseButton, ControllerButton = Settings.Aiming.AimbotKey.ControllerButton, Waiting = Settings.Aiming.AimbotKey.Waiting, Label = Settings.Aiming.AimbotKey.Label },
                    AimingType = Settings.Aiming.AimingType,
                    ToggleType = Settings.Aiming.ToggleType,
                    Aimbot = Settings.Aiming.Aimbot,
                    StickyAim = Settings.Aiming.StickyAim,
                    Sensitivity = Settings.Aiming.Sensitivity,
                    Smoothness = Settings.Aiming.Smoothness,
                    SmoothnessX = Settings.Aiming.SmoothnessX,
                    SmoothnessY = Settings.Aiming.SmoothnessY,
                    Prediction = Settings.Aiming.Prediction,
                    PredictionY = Settings.Aiming.PredictionY,
                    PredictionX = Settings.Aiming.PredictionX,
                    FOV = Settings.Aiming.FOV,
                    ShowFOV = Settings.Aiming.ShowFOV,
                    FillFOV = Settings.Aiming.FillFOV,
                    AnimatedFOV = Settings.Aiming.AnimatedFOV,
                    Range = Settings.Aiming.Range,
                    TargetBone = Settings.Aiming.TargetBone
                },
                Silent = new()
                {
                    SilentAimbotKey = new() { Key = Settings.Silent.SilentAimbotKey.Key, MouseButton = Settings.Silent.SilentAimbotKey.MouseButton, ControllerButton = Settings.Silent.SilentAimbotKey.ControllerButton, Waiting = Settings.Silent.SilentAimbotKey.Waiting, Label = Settings.Silent.SilentAimbotKey.Label },
                    SilentAimbot = Settings.Silent.SilentAimbot,
                    AlwaysOn = Settings.Silent.AlwaysOn,
                    SilentVisualizer = Settings.Silent.SilentVisualizer,
                    ShowSilentFOV = Settings.Silent.ShowSilentFOV,
                    SPrediction = Settings.Silent.SPrediction,
                    SilentFOV = Settings.Silent.SilentFOV,
                    PredictionY = Settings.Silent.PredictionY,
                    PredictionX = Settings.Silent.PredictionX,
                    SFOV = Settings.Silent.SFOV
                },
                Checks = new() { TeamCheck = Settings.Checks.TeamCheck, DownedCheck = Settings.Checks.DownedCheck, TransparencyCheck = Settings.Checks.TransparencyCheck, WallCheck = Settings.Checks.WallCheck },
                Network = new()
                {
                    DeSyncBind = new() { Key = Settings.Network.DeSyncBind.Key, MouseButton = Settings.Network.DeSyncBind.MouseButton, ControllerButton = Settings.Network.DeSyncBind.ControllerButton, Waiting = Settings.Network.DeSyncBind.Waiting, Label = Settings.Network.DeSyncBind.Label },
                    DeSync = Settings.Network.DeSync,
                    DeSyncVisualizer = Settings.Network.DeSyncVisualizer
                },
                Flight = new()
                {
                    VFlightBind = new() { Key = Settings.Flight.VFlightBind.Key, MouseButton = Settings.Flight.VFlightBind.MouseButton, ControllerButton = Settings.Flight.VFlightBind.ControllerButton, Waiting = Settings.Flight.VFlightBind.Waiting, Label = Settings.Flight.VFlightBind.Label },
                    VFlight = Settings.Flight.VFlight,
                    VFlightSpeed = Settings.Flight.VFlightSpeed
                }
            };
        }

        private static void Import(ConfigData c)
        {
            Settings.Humanoid.WalkspeedEnabled = c.Humanoid.WalkspeedEnabled;
            Settings.Humanoid.JumpPowerEnabled = c.Humanoid.JumpPowerEnabled;
            Settings.Humanoid.Walkspeed = c.Humanoid.Walkspeed;
            Settings.Humanoid.JumpPower = c.Humanoid.JumpPower;

            Settings.Camera.FOVEnabled = c.Camera.FOVEnabled;
            Settings.Camera.FOV = c.Camera.FOV;

            Settings.Visuals.BoxESP = c.Visuals.BoxESP;
            Settings.Visuals.FilledBox = c.Visuals.FilledBox;
            Settings.Visuals.Box = c.Visuals.Box;
            Settings.Visuals.BoxFill = c.Visuals.BoxFill;
            Settings.Visuals.Tracers = c.Visuals.Tracers;
            Settings.Visuals.Skeleton = c.Visuals.Skeleton;
            Settings.Visuals.Name = c.Visuals.Name;
            Settings.Visuals.Distance = c.Visuals.Distance;
            Settings.Visuals.Health = c.Visuals.Health;
            Settings.Visuals.ESP3D = c.Visuals.ESP3D;
            Settings.Visuals.HeadCircle = c.Visuals.HeadCircle;
            Settings.Visuals.CornerESP = c.Visuals.CornerESP;
            Settings.Visuals.RemoveBorders = c.Visuals.RemoveBorders;
            Settings.Visuals.ChinaHat = c.Visuals.ChinaHat;
            Settings.Visuals.LocalPlayerESP = c.Visuals.LocalPlayerESP;
            Settings.Visuals.TracersStart = c.Visuals.TracersStart;
            Settings.Visuals.NameSize = c.Visuals.NameSize;
            Settings.Visuals.DistanceSize = c.Visuals.DistanceSize;
            Settings.Visuals.TracerThickness = c.Visuals.TracerThickness;
            Settings.Visuals.HeadCircleMaxScale = c.Visuals.HeadCircleMaxScale;

            Settings.Aiming.AimbotKey.Key = c.Aiming.AimbotKey.Key;
            Settings.Aiming.AimbotKey.MouseButton = c.Aiming.AimbotKey.MouseButton;
            Settings.Aiming.AimbotKey.ControllerButton = c.Aiming.AimbotKey.ControllerButton;
            Settings.Aiming.AimbotKey.Label = c.Aiming.AimbotKey.Label;
            Settings.Aiming.AimingType = c.Aiming.AimingType;
            Settings.Aiming.ToggleType = c.Aiming.ToggleType;
            Settings.Aiming.Aimbot = c.Aiming.Aimbot;
            Settings.Aiming.StickyAim = c.Aiming.StickyAim;
            Settings.Aiming.Sensitivity = c.Aiming.Sensitivity;
            Settings.Aiming.Smoothness = c.Aiming.Smoothness;
            Settings.Aiming.SmoothnessX = c.Aiming.SmoothnessX;
            Settings.Aiming.SmoothnessY = c.Aiming.SmoothnessY;
            Settings.Aiming.Prediction = c.Aiming.Prediction;
            Settings.Aiming.PredictionY = c.Aiming.PredictionY;
            Settings.Aiming.PredictionX = c.Aiming.PredictionX;
            Settings.Aiming.FOV = c.Aiming.FOV;
            Settings.Aiming.ShowFOV = c.Aiming.ShowFOV;
            Settings.Aiming.FillFOV = c.Aiming.FillFOV;
            Settings.Aiming.AnimatedFOV = c.Aiming.AnimatedFOV;
            Settings.Aiming.Range = c.Aiming.Range;
            Settings.Aiming.TargetBone = c.Aiming.TargetBone;

            Settings.Silent.SilentAimbotKey.Key = c.Silent.SilentAimbotKey.Key;
            Settings.Silent.SilentAimbotKey.MouseButton = c.Silent.SilentAimbotKey.MouseButton;
            Settings.Silent.SilentAimbotKey.ControllerButton = c.Silent.SilentAimbotKey.ControllerButton;
            Settings.Silent.SilentAimbotKey.Label = c.Silent.SilentAimbotKey.Label;
            Settings.Silent.SilentAimbot = c.Silent.SilentAimbot;
            Settings.Silent.AlwaysOn = c.Silent.AlwaysOn;
            Settings.Silent.SilentVisualizer = c.Silent.SilentVisualizer;
            Settings.Silent.ShowSilentFOV = c.Silent.ShowSilentFOV;
            Settings.Silent.SPrediction = c.Silent.SPrediction;
            Settings.Silent.SilentFOV = c.Silent.SilentFOV;
            Settings.Silent.PredictionY = c.Silent.PredictionY;
            Settings.Silent.PredictionX = c.Silent.PredictionX;
            Settings.Silent.SFOV = c.Silent.SFOV;

            Settings.Checks.TeamCheck = c.Checks.TeamCheck;
            Settings.Checks.DownedCheck = c.Checks.DownedCheck;
            Settings.Checks.TransparencyCheck = c.Checks.TransparencyCheck;
            Settings.Checks.WallCheck = c.Checks.WallCheck;

            Settings.Network.DeSyncBind.Key = c.Network.DeSyncBind.Key;
            Settings.Network.DeSyncBind.MouseButton = c.Network.DeSyncBind.MouseButton;
            Settings.Network.DeSyncBind.ControllerButton = c.Network.DeSyncBind.ControllerButton;
            Settings.Network.DeSyncBind.Label = c.Network.DeSyncBind.Label;
            Settings.Network.DeSync = c.Network.DeSync;
            Settings.Network.DeSyncVisualizer = c.Network.DeSyncVisualizer;

            Settings.Flight.VFlightBind.Key = c.Flight.VFlightBind.Key;
            Settings.Flight.VFlightBind.MouseButton = c.Flight.VFlightBind.MouseButton;
            Settings.Flight.VFlightBind.ControllerButton = c.Flight.VFlightBind.ControllerButton;
            Settings.Flight.VFlightBind.Label = c.Flight.VFlightBind.Label;
            Settings.Flight.VFlight = c.Flight.VFlight;
            Settings.Flight.VFlightSpeed = c.Flight.VFlightSpeed;
        }
    }
}
