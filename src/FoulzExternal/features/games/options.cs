using FoulzExternal.helpers.keybind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static Options.Aiming;

// daddy

namespace Options
{
    public class Humanoid
    {
        public bool WalkspeedEnabled = false;
        public bool JumpPowerEnabled = false;

        public float Walkspeed = 16f;
        public float JumpPower = 50f;

        public Humanoid()
        {
        }
    }

    public class Camera
    {
        public bool FOVEnabled = false;
        public float FOV = 70f;

        public Camera()
        {
        }
    }
    public class Visuals
    {
        public bool BoxESP = false;
        public bool FilledBox = false;
        public bool Box = false;
        public bool BoxFill = false;
        public bool Tracers = false;
        public bool Skeleton = false;
        public bool Name = false;
        public bool Distance = false;
        public bool Health = false;
        public bool ESP3D = false;
        public bool HeadCircle = false;
        public bool CornerESP = false;
        public bool RemoveBorders = false;
        public bool ChinaHat = false;
        public bool LocalPlayerESP = false;
        public int TracersStart = 0;
        public float NameSize = 12f;
        public float DistanceSize = 15f;
        public float TracerThickness = 1.5f;
        public float HeadCircleMaxScale = 2.5f;
        public Visuals()
        {
        }
    }

    public class Aiming
    {
        public KeyBind AimbotKey = new KeyBind("Aimbot");
        public int AimingType;
        public int ToggleType;
        public bool Aimbot = false;
        public bool StickyAim = false;
        public float Sensitivity = 1.0f;
        public bool Smoothness = false;
        public float SmoothnessX = 0.0f;
        public float SmoothnessY = 0.05f;
        public bool Prediction = false;
        public float PredictionY = 2f;
        public float PredictionX = 2f;
        public float FOV = 100f;
        public bool ShowFOV = false;
        public bool FillFOV = false;
        public bool AnimatedFOV = false;
        public float Range = 100f;
        public int TargetBone = 0;

        public Aiming() { }
    }
    public class Silent
    {
        public KeyBind SilentAimbotKey = new KeyBind("SilentAimbotKey");
        public bool SilentAimbot = false;
        public bool AlwaysOn = false;
        public bool SilentVisualizer = false;
        public bool ShowSilentFOV = false;
        public bool SPrediction = false;
        public float SilentFOV = 100f;
        public float PredictionY = 2f;
        public float PredictionX = 2f;
        public float SFOV = 150f;

        public Silent() { }
    }

    public class Checks
    {
        public bool TeamCheck = false;
        public bool DownedCheck = false;
        public bool TransparencyCheck = false;
        public bool WallCheck = false;
        public Checks()
        {
        }
    }
    public class Network
    {
        public KeyBind DeSyncBind = new KeyBind("DeSyncBind");
        public bool DeSync = false;
        public bool DeSyncVisualizer = false;

        public Network()
        {
        }
    }
    public class Flight
    {
        public KeyBind VFlightBind = new KeyBind("VFlightBind");
        public bool VFlight = false;
        public float VFlightSpeed = 1.5f;

        public Flight()
        {
        }
    }
    public static class Settings
    {
        public static Humanoid Humanoid = new Humanoid();
        public static Camera Camera = new Camera();
        public static Visuals Visuals = new Visuals();
        public static Aiming Aiming = new Aiming();
        public static Checks Checks = new Checks();
        public static Network Network = new Network();
        public static Flight Flight = new Flight();
        public static Silent Silent = new Silent();
    }
}
