using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoulzExternal.helpers.keybind;
using Options;

namespace FoulzExternal.config
{
    /// <summary>
    /// Serializable configuration data structure
    /// </summary>
    public class ConfigData
    {
        public HumanoidConfig Humanoid { get; set; } = new HumanoidConfig();
        public CameraConfig Camera { get; set; } = new CameraConfig();
        public VisualsConfig Visuals { get; set; } = new VisualsConfig();
        public AimingConfig Aiming { get; set; } = new AimingConfig();
        public SilentConfig Silent { get; set; } = new SilentConfig();
        public ChecksConfig Checks { get; set; } = new ChecksConfig();
        public NetworkConfig Network { get; set; } = new NetworkConfig();
        public FlightConfig Flight { get; set; } = new FlightConfig();
    }

    public class HumanoidConfig
    {
        public bool WalkspeedEnabled { get; set; } = false;
        public bool JumpPowerEnabled { get; set; } = false;
        public float Walkspeed { get; set; } = 16f;
        public float JumpPower { get; set; } = 50f;
    }

    public class CameraConfig
    {
        public bool FOVEnabled { get; set; } = false;
        public float FOV { get; set; } = 70f;
    }

    public class VisualsConfig
    {
        public bool BoxESP { get; set; } = false;
        public bool FilledBox { get; set; } = false;
        public bool Box { get; set; } = false;
        public bool BoxFill { get; set; } = false;
        public bool Tracers { get; set; } = false;
        public bool Skeleton { get; set; } = false;
        public bool Name { get; set; } = false;
        public bool Distance { get; set; } = false;
        public bool Health { get; set; } = false;
        public bool ESP3D { get; set; } = false;
        public bool HeadCircle { get; set; } = false;
        public bool CornerESP { get; set; } = false;
        public bool RemoveBorders { get; set; } = false;
        public bool ChinaHat { get; set; } = false;
        public bool LocalPlayerESP { get; set; } = false;
        public int TracersStart { get; set; } = 0;
        public float NameSize { get; set; } = 12f;
        public float DistanceSize { get; set; } = 15f;
        public float TracerThickness { get; set; } = 1.5f;
        public float HeadCircleMaxScale { get; set; } = 2.5f;
    }

    public class AimingConfig
    {
        public KeyBindConfig AimbotKey { get; set; } = new KeyBindConfig("Aimbot");
        public int AimingType { get; set; } = 0;
        public int ToggleType { get; set; } = 0;
        public bool Aimbot { get; set; } = false;
        public bool StickyAim { get; set; } = false;
        public float Sensitivity { get; set; } = 1.0f;
        public bool Smoothness { get; set; } = false;
        public float SmoothnessX { get; set; } = 0.0f;
        public float SmoothnessY { get; set; } = 0.05f;
        public bool Prediction { get; set; } = false;
        public float PredictionY { get; set; } = 2f;
        public float PredictionX { get; set; } = 2f;
        public float FOV { get; set; } = 100f;
        public bool ShowFOV { get; set; } = false;
        public bool FillFOV { get; set; } = false;
        public bool AnimatedFOV { get; set; } = false;
        public float Range { get; set; } = 100f;
        public int TargetBone { get; set; } = 0;
    }

    public class SilentConfig
    {
        public KeyBindConfig SilentAimbotKey { get; set; } = new KeyBindConfig("SilentAimbotKey");
        public bool SilentAimbot { get; set; } = false;
        public bool AlwaysOn { get; set; } = false;
        public bool SilentVisualizer { get; set; } = false;
        public bool ShowSilentFOV { get; set; } = false;
        public bool SPrediction { get; set; } = false;
        public float SilentFOV { get; set; } = 100f;
        public float PredictionY { get; set; } = 2f;
        public float PredictionX { get; set; } = 2f;
        public float SFOV { get; set; } = 150f;
    }

    public class ChecksConfig
    {
        public bool TeamCheck { get; set; } = false;
        public bool DownedCheck { get; set; } = false;
        public bool TransparencyCheck { get; set; } = false;
        public bool WallCheck { get; set; } = false;
    }

    public class NetworkConfig
    {
        public KeyBindConfig DeSyncBind { get; set; } = new KeyBindConfig("DeSyncBind");
        public bool DeSync { get; set; } = false;
        public bool DeSyncVisualizer { get; set; } = false;
    }

    public class FlightConfig
    {
        public KeyBindConfig VFlightBind { get; set; } = new KeyBindConfig("VFlightBind");
        public bool VFlight { get; set; } = false;
        public float VFlightSpeed { get; set; } = 1.5f;
    }

    public class KeyBindConfig
    {
        public int Key { get; set; } = 0;
        public int MouseButton { get; set; } = -1;
        public int ControllerButton { get; set; } = -1;
        public bool Waiting { get; set; } = false;
        public string Label { get; set; } = string.Empty;

        public KeyBindConfig() { }

        public KeyBindConfig(string label)
        {
            Label = label ?? string.Empty;
        }
    }

    /// <summary>
    /// Configuration manager for saving and loading .cfg files
    /// </summary>
    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        /// <summary>
        /// Gets the default configuration directory path
        /// </summary>
        public static string GetConfigDirectory()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configDir = Path.Combine(appData, "FoulzExternal", "configs");
            
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            
            return configDir;
        }

        /// <summary>
        /// Gets the path to the default config settings file
        /// </summary>
        private static string GetDefaultConfigSettingsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configDir = Path.Combine(appData, "FoulzExternal");
            
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            
            return Path.Combine(configDir, "default_config.txt");
        }

        /// <summary>
        /// Gets the name of the default config, or null if none is set
        /// </summary>
        public static string GetDefaultConfigName()
        {
            try
            {
                string path = GetDefaultConfigSettingsPath();
                if (File.Exists(path))
                {
                    string name = File.ReadAllText(path).Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        return name;
                    }
                }
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to read default config: {0}", ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Sets the default config name
        /// </summary>
        public static bool SetDefaultConfigName(string configName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configName))
                {
                    // Clear default config
                    string path = GetDefaultConfigSettingsPath();
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Default config cleared");
                    return true;
                }

                // Verify config exists
                string configPath = GetConfigPath(configName);
                if (!File.Exists(configPath))
                {
                    FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Cannot set default: config '{0}' does not exist", configName);
                    return false;
                }

                string path2 = GetDefaultConfigSettingsPath();
                File.WriteAllText(path2, configName.Trim());
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Default config set to: {0}", configName);
                return true;
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to set default config: {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Loads the default config if one is set
        /// </summary>
        public static bool LoadDefaultConfig()
        {
            string defaultConfig = GetDefaultConfigName();
            if (!string.IsNullOrEmpty(defaultConfig))
            {
                return LoadConfig(defaultConfig);
            }
            return false;
        }

        /// <summary>
        /// Gets the full path for a configuration file
        /// </summary>
        public static string GetConfigPath(string configName)
        {
            if (!configName.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase))
            {
                configName += ".cfg";
            }
            
            return Path.Combine(GetConfigDirectory(), configName);
        }

        /// <summary>
        /// Saves current settings to a .cfg file
        /// </summary>
        public static bool SaveConfig(string configName)
        {
            try
            {
                ConfigData config = ExportCurrentSettings();
                string filePath = GetConfigPath(configName);
                string json = JsonSerializer.Serialize(config, _jsonOptions);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to save config '{0}': {1}", configName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Loads settings from a .cfg file
        /// </summary>
        public static bool LoadConfig(string configName)
        {
            try
            {
                string filePath = GetConfigPath(configName);
                
                if (!File.Exists(filePath))
                {
                    FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Config file '{0}' not found", configName);
                    return false;
                }

                string json = File.ReadAllText(filePath);
                ConfigData config = JsonSerializer.Deserialize<ConfigData>(json, _jsonOptions);
                
                if (config == null)
                {
                    FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to deserialize config '{0}'", configName);
                    return false;
                }

                ImportSettings(config);
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Successfully loaded config '{0}'", configName);
                return true;
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to load config '{0}': {1}", configName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Resets all settings to default values
        /// </summary>
        public static void ResetToDefaults()
        {
            try
            {
                ConfigData defaultConfig = new ConfigData();
                ImportSettings(defaultConfig);
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Settings reset to defaults");
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to reset to defaults: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of all available config files
        /// </summary>
        public static string[] GetAvailableConfigs()
        {
            try
            {
                string configDir = GetConfigDirectory();
                if (!Directory.Exists(configDir))
                {
                    return Array.Empty<string>();
                }

                string[] files = Directory.GetFiles(configDir, "*.cfg");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileNameWithoutExtension(files[i]);
                }
                
                return files;
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Deletes a configuration file
        /// </summary>
        public static bool DeleteConfig(string configName)
        {
            try
            {
                string filePath = GetConfigPath(configName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Deleted config '{0}'", configName);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                FoulzExternal.logging.LogsWindow.Log("[ConfigManager] Failed to delete config '{0}': {1}", configName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Exports current settings to ConfigData
        /// </summary>
        private static ConfigData ExportCurrentSettings()
        {
            return new ConfigData
            {
                Humanoid = new HumanoidConfig
                {
                    WalkspeedEnabled = Settings.Humanoid.WalkspeedEnabled,
                    JumpPowerEnabled = Settings.Humanoid.JumpPowerEnabled,
                    Walkspeed = Settings.Humanoid.Walkspeed,
                    JumpPower = Settings.Humanoid.JumpPower
                },
                Camera = new CameraConfig
                {
                    FOVEnabled = Settings.Camera.FOVEnabled,
                    FOV = Settings.Camera.FOV
                },
                Visuals = new VisualsConfig
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
                Aiming = new AimingConfig
                {
                    AimbotKey = new KeyBindConfig
                    {
                        Key = Settings.Aiming.AimbotKey.Key,
                        MouseButton = Settings.Aiming.AimbotKey.MouseButton,
                        ControllerButton = Settings.Aiming.AimbotKey.ControllerButton,
                        Waiting = Settings.Aiming.AimbotKey.Waiting,
                        Label = Settings.Aiming.AimbotKey.Label
                    },
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
                Silent = new SilentConfig
                {
                    SilentAimbotKey = new KeyBindConfig
                    {
                        Key = Settings.Silent.SilentAimbotKey.Key,
                        MouseButton = Settings.Silent.SilentAimbotKey.MouseButton,
                        ControllerButton = Settings.Silent.SilentAimbotKey.ControllerButton,
                        Waiting = Settings.Silent.SilentAimbotKey.Waiting,
                        Label = Settings.Silent.SilentAimbotKey.Label
                    },
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
                Checks = new ChecksConfig
                {
                    TeamCheck = Settings.Checks.TeamCheck,
                    DownedCheck = Settings.Checks.DownedCheck,
                    TransparencyCheck = Settings.Checks.TransparencyCheck,
                    WallCheck = Settings.Checks.WallCheck
                },
                Network = new NetworkConfig
                {
                    DeSyncBind = new KeyBindConfig
                    {
                        Key = Settings.Network.DeSyncBind.Key,
                        MouseButton = Settings.Network.DeSyncBind.MouseButton,
                        ControllerButton = Settings.Network.DeSyncBind.ControllerButton,
                        Waiting = Settings.Network.DeSyncBind.Waiting,
                        Label = Settings.Network.DeSyncBind.Label
                    },
                    DeSync = Settings.Network.DeSync,
                    DeSyncVisualizer = Settings.Network.DeSyncVisualizer
                },
                Flight = new FlightConfig
                {
                    VFlightBind = new KeyBindConfig
                    {
                        Key = Settings.Flight.VFlightBind.Key,
                        MouseButton = Settings.Flight.VFlightBind.MouseButton,
                        ControllerButton = Settings.Flight.VFlightBind.ControllerButton,
                        Waiting = Settings.Flight.VFlightBind.Waiting,
                        Label = Settings.Flight.VFlightBind.Label
                    },
                    VFlight = Settings.Flight.VFlight,
                    VFlightSpeed = Settings.Flight.VFlightSpeed
                }
            };
        }

        /// <summary>
        /// Imports settings from ConfigData to current Settings
        /// </summary>
        private static void ImportSettings(ConfigData config)
        {
            // Humanoid
            Settings.Humanoid.WalkspeedEnabled = config.Humanoid.WalkspeedEnabled;
            Settings.Humanoid.JumpPowerEnabled = config.Humanoid.JumpPowerEnabled;
            Settings.Humanoid.Walkspeed = config.Humanoid.Walkspeed;
            Settings.Humanoid.JumpPower = config.Humanoid.JumpPower;

            // Camera
            Settings.Camera.FOVEnabled = config.Camera.FOVEnabled;
            Settings.Camera.FOV = config.Camera.FOV;

            // Visuals
            Settings.Visuals.BoxESP = config.Visuals.BoxESP;
            Settings.Visuals.FilledBox = config.Visuals.FilledBox;
            Settings.Visuals.Box = config.Visuals.Box;
            Settings.Visuals.BoxFill = config.Visuals.BoxFill;
            Settings.Visuals.Tracers = config.Visuals.Tracers;
            Settings.Visuals.Skeleton = config.Visuals.Skeleton;
            Settings.Visuals.Name = config.Visuals.Name;
            Settings.Visuals.Distance = config.Visuals.Distance;
            Settings.Visuals.Health = config.Visuals.Health;
            Settings.Visuals.ESP3D = config.Visuals.ESP3D;
            Settings.Visuals.HeadCircle = config.Visuals.HeadCircle;
            Settings.Visuals.CornerESP = config.Visuals.CornerESP;
            Settings.Visuals.RemoveBorders = config.Visuals.RemoveBorders;
            Settings.Visuals.ChinaHat = config.Visuals.ChinaHat;
            Settings.Visuals.LocalPlayerESP = config.Visuals.LocalPlayerESP;
            Settings.Visuals.TracersStart = config.Visuals.TracersStart;
            Settings.Visuals.NameSize = config.Visuals.NameSize;
            Settings.Visuals.DistanceSize = config.Visuals.DistanceSize;
            Settings.Visuals.TracerThickness = config.Visuals.TracerThickness;
            Settings.Visuals.HeadCircleMaxScale = config.Visuals.HeadCircleMaxScale;

            // Aiming
            Settings.Aiming.AimbotKey.Key = config.Aiming.AimbotKey.Key;
            Settings.Aiming.AimbotKey.MouseButton = config.Aiming.AimbotKey.MouseButton;
            Settings.Aiming.AimbotKey.ControllerButton = config.Aiming.AimbotKey.ControllerButton;
            Settings.Aiming.AimbotKey.Waiting = config.Aiming.AimbotKey.Waiting;
            Settings.Aiming.AimbotKey.Label = config.Aiming.AimbotKey.Label;
            Settings.Aiming.AimingType = config.Aiming.AimingType;
            Settings.Aiming.ToggleType = config.Aiming.ToggleType;
            Settings.Aiming.Aimbot = config.Aiming.Aimbot;
            Settings.Aiming.StickyAim = config.Aiming.StickyAim;
            Settings.Aiming.Sensitivity = config.Aiming.Sensitivity;
            Settings.Aiming.Smoothness = config.Aiming.Smoothness;
            Settings.Aiming.SmoothnessX = config.Aiming.SmoothnessX;
            Settings.Aiming.SmoothnessY = config.Aiming.SmoothnessY;
            Settings.Aiming.Prediction = config.Aiming.Prediction;
            Settings.Aiming.PredictionY = config.Aiming.PredictionY;
            Settings.Aiming.PredictionX = config.Aiming.PredictionX;
            Settings.Aiming.FOV = config.Aiming.FOV;
            Settings.Aiming.ShowFOV = config.Aiming.ShowFOV;
            Settings.Aiming.FillFOV = config.Aiming.FillFOV;
            Settings.Aiming.AnimatedFOV = config.Aiming.AnimatedFOV;
            Settings.Aiming.Range = config.Aiming.Range;
            Settings.Aiming.TargetBone = config.Aiming.TargetBone;

            // Silent
            Settings.Silent.SilentAimbotKey.Key = config.Silent.SilentAimbotKey.Key;
            Settings.Silent.SilentAimbotKey.MouseButton = config.Silent.SilentAimbotKey.MouseButton;
            Settings.Silent.SilentAimbotKey.ControllerButton = config.Silent.SilentAimbotKey.ControllerButton;
            Settings.Silent.SilentAimbotKey.Waiting = config.Silent.SilentAimbotKey.Waiting;
            Settings.Silent.SilentAimbotKey.Label = config.Silent.SilentAimbotKey.Label;
            Settings.Silent.SilentAimbot = config.Silent.SilentAimbot;
            Settings.Silent.AlwaysOn = config.Silent.AlwaysOn;
            Settings.Silent.SilentVisualizer = config.Silent.SilentVisualizer;
            Settings.Silent.ShowSilentFOV = config.Silent.ShowSilentFOV;
            Settings.Silent.SPrediction = config.Silent.SPrediction;
            Settings.Silent.SilentFOV = config.Silent.SilentFOV;
            Settings.Silent.PredictionY = config.Silent.PredictionY;
            Settings.Silent.PredictionX = config.Silent.PredictionX;
            Settings.Silent.SFOV = config.Silent.SFOV;

            // Checks
            Settings.Checks.TeamCheck = config.Checks.TeamCheck;
            Settings.Checks.DownedCheck = config.Checks.DownedCheck;
            Settings.Checks.TransparencyCheck = config.Checks.TransparencyCheck;
            Settings.Checks.WallCheck = config.Checks.WallCheck;

            // Network
            Settings.Network.DeSyncBind.Key = config.Network.DeSyncBind.Key;
            Settings.Network.DeSyncBind.MouseButton = config.Network.DeSyncBind.MouseButton;
            Settings.Network.DeSyncBind.ControllerButton = config.Network.DeSyncBind.ControllerButton;
            Settings.Network.DeSyncBind.Waiting = config.Network.DeSyncBind.Waiting;
            Settings.Network.DeSyncBind.Label = config.Network.DeSyncBind.Label;
            Settings.Network.DeSync = config.Network.DeSync;
            Settings.Network.DeSyncVisualizer = config.Network.DeSyncVisualizer;

            // Flight
            Settings.Flight.VFlightBind.Key = config.Flight.VFlightBind.Key;
            Settings.Flight.VFlightBind.MouseButton = config.Flight.VFlightBind.MouseButton;
            Settings.Flight.VFlightBind.ControllerButton = config.Flight.VFlightBind.ControllerButton;
            Settings.Flight.VFlightBind.Waiting = config.Flight.VFlightBind.Waiting;
            Settings.Flight.VFlightBind.Label = config.Flight.VFlightBind.Label;
            Settings.Flight.VFlight = config.Flight.VFlight;
            Settings.Flight.VFlightSpeed = config.Flight.VFlightSpeed;
        }
    }
}
