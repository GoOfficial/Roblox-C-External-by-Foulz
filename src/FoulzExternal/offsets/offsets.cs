//UPDATE OFFSETS FROM: https://imtheo.lol/Offsets/Offsets.cs

// =============================================================
//                       RbxDumperV2                            
// -------------------------------------------------------------
//  Dumped By       : theo (https://imtheo.lol)                 
//  Roblox Version  : version-df7528517c6849f7
//  Dumper Version  : 2.1.2
//  Dumped At       : 20:42 20/02/2026 (GMT)
//  Total Offsets   : 300
// -------------------------------------------------------------
//  Join the discord!                                           
//  https://discord.gg/rbxoffsets                               
// =============================================================

namespace Offsets
{
    public static class Info
    {
        public static string ClientVersion = "version-df7528517c6849f7";
    }

    public static class AirProperties
    {
        public const long AirDensity = 0x18;
        public const long GlobalWind = 0x3c;
    }

    public static class AnimationTrack
    {
        public const long Animation = 0xd0;
        public const long Animator = 0x118;
        public const long IsPlaying = 0x5a8;
        public const long Looped = 0xf5;
        public const long Speed = 0xe4;
    }

    public static class Animator
    {
        public const long ActiveAnimations = 0x648;
    }

    public static class Atmosphere
    {
        public const long Color = 0xd0;
        public const long Decay = 0xdc;
        public const long Density = 0xe8;
        public const long Glare = 0xec;
        public const long Haze = 0xf0;
        public const long Offset = 0xf4;
    }

    public static class Attachment
    {
        public const long Position = 0xdc;
    }

    public static class BasePart
    {
        public const long Color3 = 0x194;
        public const long Primitive = 0x148;
        public const long Shape = 0x1b1;
        public const long Transparency = 0xf0;
    }

    public static class BloomEffect
    {
        public const long Enabled = 0xc8;
        public const long Intensity = 0xd0;
        public const long Size = 0xd4;
        public const long Threshold = 0xd8;
    }

    public static class BlurEffect
    {
        public const long Enabled = 0xc8;
        public const long Size = 0xd0;
    }

    public static class ByteCode
    {
        public const long Pointer = 0x10;
        public const long Size = 0x20;
    }

    public static class Camera
    {
        public const long CameraSubject = 0xe8;
        public const long CameraType = 0x158;
        public const long FieldOfView = 0x160;
        public const long Position = 0x11c;
        public const long Rotation = 0xf8;
        public const long Viewport = 0x2ac;
        public const long ViewportSize = 0x2e8;
    }

    public static class CharacterMesh
    {
        public const long BaseTextureId = 0xe0;
        public const long BodyPart = 0x160;
        public const long MeshId = 0x110;
        public const long OverlayTextureId = 0x140;
    }

    public static class ClickDetector
    {
        public const long MaxActivationDistance = 0x100;
        public const long MouseIcon = 0xe0;
    }

    public static class Clothing
    {
        public const long Color3 = 0x128;
        public const long Template = 0x108;
    }

    public static class ColorCorrectionEffect
    {
        public const long Brightness = 0xdc;
        public const long Contrast = 0xe0;
        public const long Enabled = 0xc8;
        public const long TintColor = 0xd0;
    }

    public static class ColorGradingEffect
    {
        public const long Enabled = 0xc8;
        public const long TonemapperPreset = 0xd0;
    }

    public static class DataModel
    {
        public const long CreatorId = 0x188;
        public const long GameId = 0x190;
        public const long GameLoaded = 0x5f8;
        public const long JobId = 0x138;
        public const long PlaceId = 0x198;
        public const long PlaceVersion = 0x1b4;
        public const long PrimitiveCount = 0x438;
        public const long ScriptContext = 0x3f0;
        public const long ServerIP = 0x5e0;
        public const long Workspace = 0x178;
    }

    public static class DepthOfFieldEffect
    {
        public const long Enabled = 0xc8;
        public const long FarIntensity = 0xd0;
        public const long FocusDistance = 0xd4;
        public const long InFocusRadius = 0xd8;
        public const long NearIntensity = 0xdc;
    }

    public static class FakeDataModel
    {
        public const long Pointer = 0x7e35858;
        public const long RealDataModel = 0x1c0;
    }

    public static class GuiBase2D
    {
        public const long AbsolutePosition = 0x110;
        public const long AbsoluteRotation = 0x188;
        public const long AbsoluteSize = 0x118;
    }

    public static class GuiObject
    {
        public const long BackgroundColor3 = 0x548;
        public const long BackgroundTransparency = 0x554;
        public const long BorderColor3 = 0x554;
        public const long Image = 0xa00;
        public const long LayoutOrder = 0x584;
        public const long Position = 0x518;
        public const long RichText = 0xaa8;
        public const long Rotation = 0x188;
        public const long ScreenGui_Enabled = 0x4cc;
        public const long Size = 0x538;
        public const long Text = 0xe08;
        public const long TextColor3 = 0xeb8;
        public const long Visible = 0x5b1;
        public const long ZIndex = 0x5a8;
    }

    public static class Humanoid
    {
        public const long AutoJumpEnabled = 0x1d8;
        public const long AutoRotate = 0x1d9;
        public const long BreakJointsOnDeath = 0x1db;
        public const long CameraOffset = 0x140;
        public const long DisplayDistanceType = 0x18c;
        public const long DisplayName = 0xd0;
        public const long EvaluateStateMachine = 0x1dc;
        public const long FloorMaterial = 0x190;
        public const long Health = 0x194;
        public const long HealthDisplayDistance = 0x198;
        public const long HealthDisplayType = 0x19c;
        public const long HipHeight = 0x1a0;
        public const long HumanoidRootPart = 0x4c0;
        public const long HumanoidState = 0x8d8;
        public const long HumanoidStateID = 0x20;
        public const long IsWalking = 0x956;
        public const long Jump = 0x1dd;
        public const long JumpHeight = 0x1ac;
        public const long JumpPower = 0x1b0;
        public const long MaxHealth = 0x1b4;
        public const long MaxSlopeAngle = 0x1b8;
        public const long MoveDirection = 0x158;
        public const long MoveToPart = 0x130;
        public const long MoveToPoint = 0x17c;
        public const long NameDisplayDistance = 0x1bc;
        public const long NameOcclusion = 0x1c0;
        public const long PlatformStand = 0x1df;
        public const long RequiresNeck = 0x1e0;
        public const long RigType = 0x1c8;
        public const long SeatPart = 0x120;
        public const long Sit = 0x1e0;
        public const long TargetPoint = 0x164;
        public const long Walkspeed = 0x1d4;
        public const long WalkspeedCheck = 0x3c0;
    }

    public static class Instance
    {
        public const long AttributeContainer = 0x48;
        public const long AttributeList = 0x18;
        public const long AttributeToNext = 0x58;
        public const long AttributeToValue = 0x18;
        public const long ChildrenEnd = 0x8;
        public const long ChildrenStart = 0x70;
        public const long ClassBase = 0xcf0;
        public const long ClassDescriptor = 0x18;
        public const long ClassName = 0x8;
        public const long Name = 0xb0;
        public const long Parent = 0x68;
        public const long This = 0x8;
    }

    public static class Lighting
    {
        public const long Ambient = 0xd8;
        public const long Brightness = 0x120;
        public const long ClockTime = 0x1b8;
        public const long ColorShift_Bottom = 0xf0;
        public const long ColorShift_Top = 0xe4;
        public const long EnvironmentDiffuseScale = 0x124;
        public const long EnvironmentSpecularScale = 0x128;
        public const long ExposureCompensation = 0x12c;
        public const long FogColor = 0xfc;
        public const long FogEnd = 0x134;
        public const long FogStart = 0x138;
        public const long GeographicLatitude = 0x190;
        public const long GlobalShadows = 0x148;
        public const long GradientBottom = 0x194;
        public const long GradientTop = 0x150;
        public const long LightColor = 0x15c;
        public const long LightDirection = 0x168;
        public const long MoonPosition = 0x184;
        public const long OutdoorAmbient = 0x108;
        public const long Sky = 0x1d8;
        public const long Source = 0x174;
        public const long SunPosition = 0x178;
    }

    public static class LocalScript
    {
        public const long ByteCode = 0x1a8;
        public const long GUID = 0xe8;
        public const long Hash = 0x1b8;
    }

    public static class MaterialColors
    {
        public const long Asphalt = 0x30;
        public const long Basalt = 0x27;
        public const long Brick = 0xf;
        public const long Cobblestone = 0x33;
        public const long Concrete = 0xc;
        public const long CrackedLava = 0x2d;
        public const long Glacier = 0x1b;
        public const long Grass = 0x6;
        public const long Ground = 0x2a;
        public const long Ice = 0x36;
        public const long LeafyGrass = 0x39;
        public const long Limestone = 0x3f;
        public const long Mud = 0x24;
        public const long Pavement = 0x42;
        public const long Rock = 0x18;
        public const long Salt = 0x3c;
        public const long Sand = 0x12;
        public const long Sandstone = 0x21;
        public const long Slate = 0x9;
        public const long Snow = 0x1e;
        public const long WoodPlanks = 0x15;
    }

    public static class MeshPart
    {
        public const long MeshId = 0x2e8;
        public const long Texture = 0x318;
    }

    public static class Misc
    {
        public const long Adornee = 0x108;
        public const long AnimationId = 0xd0;
        public const long StringLength = 0x10;
        public const long Value = 0xd0;
    }

    public static class Model
    {
        public const long PrimaryPart = 0x278;
        public const long Scale = 0x164;
    }

    public static class ModuleScript
    {
        public const long ByteCode = 0x150;
        public const long GUID = 0xe8;
        public const long Hash = 0x160;
    }

    public static class MouseService
    {
        public const long InputObject = 0x100;
        public const long MousePosition = 0xec;
        public const long SensitivityPointer = 0x7ebd660;
    }

    public static class Player
    {
        public const long CameraMode = 0x318;
        public const long Country = 0x110;
        public const long DisplayName = 0x130;
        public const long Gender = 0x0;
        public const long HealthDisplayDistance = 0x338;
        public const long LocalPlayer = 0x130;
        public const long MaxZoomDistance = 0x310;
        public const long MinZoomDistance = 0x314;
        public const long ModelInstance = 0x380;
        public const long Mouse = 0xf78;
        public const long NameDisplayDistance = 0x344;
        public const long Team = 0x290;
        public const long UserId = 0x2b8;
    }

    public static class PlayerConfigurer
    {
        public const long Pointer = 0x7e12fd0;
    }

    public static class PlayerMouse
    {
        public const long Icon = 0xe0;
        public const long Workspace = 0x168;
    }

    public static class Primitive
    {
        public const long AssemblyAngularVelocity = 0xfc;
        public const long AssemblyLinearVelocity = 0xf0;
        public const long Flags = 0x1ae;
        public const long Material = 0x248;
        public const long Owner = 0x210;
        public const long Position = 0xe4;
        public const long Rotation = 0xc0;
        public const long Size = 0x1b0;
        public const long Validate = 0x6;
    }

    public static class PrimitiveFlags
    {
        public const long Anchored = 0x2;
        public const long CanCollide = 0x8;
        public const long CanTouch = 0x10;
    }

    public static class ProximityPrompt
    {
        public const long ActionText = 0xd0;
        public const long Enabled = 0x156;
        public const long GamepadKeyCode = 0x13c;
        public const long HoldDuration = 0x140;
        public const long KeyCode = 0x144;
        public const long MaxActivationDistance = 0x148;
        public const long ObjectText = 0xf0;
        public const long RequiresLineOfSight = 0x157;
    }

    public static class RenderJob
    {
        public const long FakeDataModel = 0x38;
        public const long RealDataModel = 0x1b0;
        public const long RenderView = 0x218;
    }

    public static class RenderView
    {
        public const long DeviceD3D11 = 0x8;
        public const long LightingValid = 0x148;
        public const long SkyValid = 0x2cd;
        public const long VisualEngine = 0x10;
    }

    public static class RunService
    {
        public const long HeartbeatFPS = 0xc0;
        public const long HeartbeatTask = 0xe8;
    }

    public static class Seat
    {
        public const long Occupant = 0x220;
    }

    public static class Sky
    {
        public const long MoonAngularSize = 0x25c;
        public const long MoonTextureId = 0xe0;
        public const long SkyboxBk = 0x110;
        public const long SkyboxDn = 0x140;
        public const long SkyboxFt = 0x170;
        public const long SkyboxLf = 0x1a0;
        public const long SkyboxOrientation = 0x250;
        public const long SkyboxRt = 0x1d0;
        public const long SkyboxUp = 0x200;
        public const long StarCount = 0x260;
        public const long SunAngularSize = 0x254;
        public const long SunTextureId = 0x230;
    }

    public static class Sound
    {
        public const long Looped = 0x152;
        public const long PlaybackSpeed = 0x130;
        public const long RollOffMaxDistance = 0x134;
        public const long RollOffMinDistance = 0x138;
        public const long SoundGroup = 0x100;
        public const long SoundId = 0xe0;
        public const long Volume = 0x144;
    }

    public static class SpawnLocation
    {
        public const long AllowTeamChangeOnTouch = 0x45;
        public const long Enabled = 0x1f9;
        public const long ForcefieldDuration = 0x1f0;
        public const long Neutral = 0x1fa;
        public const long TeamColor = 0x1f4;
    }

    public static class SpecialMesh
    {
        public const long MeshId = 0x108;
        public const long Scale = 0xdc;
    }

    public static class StatsItem
    {
        public const long Value = 0xc8;
    }

    public static class SunRaysEffect
    {
        public const long Enabled = 0xc8;
        public const long Intensity = 0xd0;
        public const long Spread = 0xd4;
    }

    public static class TaskScheduler
    {
        public const long JobEnd = 0x1d8;
        public const long JobName = 0x18;
        public const long JobStart = 0x1d0;
        public const long MaxFPS = 0x1b0;
        public const long Pointer = 0x7ef3c48;
    }

    public static class Team
    {
        public const long BrickColor = 0xd0;
    }

    public static class Terrain
    {
        public const long GrassLength = 0x1f8;
        public const long MaterialColors = 0x280;
        public const long WaterColor = 0x1e8;
        public const long WaterReflectance = 0x200;
        public const long WaterTransparency = 0x204;
        public const long WaterWaveSize = 0x208;
        public const long WaterWaveSpeed = 0x20c;
    }

    public static class Textures
    {
        public const long Decal_Texture = 0x198;
        public const long Texture_Texture = 0x198;
    }

    public static class Tool
    {
        public const long CanBeDropped = 0x4a0;
        public const long Enabled = 0x34d;
        public const long Grip = 0x494;
        public const long ManualActivationOnly = 0x2b5;
        public const long RequiresHandle = 0x4a3;
        public const long TextureId = 0x348;
        public const long Tooltip = 0x450;
    }

    public static class VisualEngine
    {
        public const long Dimensions = 0x720;
        public const long FakeDataModel = 0x700;
        public const long Pointer = 0x79e9468;
        public const long RenderView = 0x800;
        public const long ViewMatrix = 0x120;
    }

    public static class Workspace
    {
        public const long CurrentCamera = 0x4a0;
        public const long DistributedGameTime = 0x4c0;
        public const long ReadOnlyGravity = 0xa28;
        public const long World = 0x3d8;
    }

    public static class World
    {
        public const long AirProperties = 0x1d8;
        public const long FallenPartsDestroyHeight = 0x1c8;
        public const long Gravity = 0x1d0;
        public const long Primitives = 0x240;
        public const long worldStepsPerSec = 0x658;
    }

}
