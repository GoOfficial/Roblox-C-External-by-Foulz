#pragma once
/* =============================================================
/*                       RbxDumperV2                            
/* -------------------------------------------------------------
/*  Dumped By       : theo (https://imtheo.lol)                 
/*  Roblox Version  : version-760d064d05424689
/*  Dumper Version  : 2.1.2
/*  Dumped At       : 07:58 26/02/2026 (GMT)
/*  Total Offsets   : 300
/* -------------------------------------------------------------
/*  Join the discord!                                           
/*  https://discord.gg/rbxoffsets                               
/* =============================================================
*/

#include <cstdint>
#include <string>
namespace Offsets {
    inline std::string ClientVersion = "version-760d064d05424689";

    namespace AirProperties {
         inline constexpr uintptr_t AirDensity = 0x18;
         inline constexpr uintptr_t GlobalWind = 0x3c;
    }

    namespace AnimationTrack {
         inline constexpr uintptr_t Animation = 0xd0;
         inline constexpr uintptr_t Animator = 0x118;
         inline constexpr uintptr_t IsPlaying = 0x4b8;
         inline constexpr uintptr_t Looped = 0xf5;
         inline constexpr uintptr_t Speed = 0xe4;
    }

    namespace Animator {
         inline constexpr uintptr_t ActiveAnimations = 0x650;
    }

    namespace Atmosphere {
         inline constexpr uintptr_t Color = 0xd0;
         inline constexpr uintptr_t Decay = 0xdc;
         inline constexpr uintptr_t Density = 0xe8;
         inline constexpr uintptr_t Glare = 0xec;
         inline constexpr uintptr_t Haze = 0xf0;
         inline constexpr uintptr_t Offset = 0xf4;
    }

    namespace Attachment {
         inline constexpr uintptr_t Position = 0xdc;
    }

    namespace BasePart {
         inline constexpr uintptr_t Color3 = 0x194;
         inline constexpr uintptr_t Primitive = 0x148;
         inline constexpr uintptr_t Shape = 0x1b1;
         inline constexpr uintptr_t Transparency = 0xf0;
    }

    namespace BloomEffect {
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t Intensity = 0xd0;
         inline constexpr uintptr_t Size = 0xd4;
         inline constexpr uintptr_t Threshold = 0xd8;
    }

    namespace BlurEffect {
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t Size = 0xd0;
    }

    namespace ByteCode {
         inline constexpr uintptr_t Pointer = 0x10;
         inline constexpr uintptr_t Size = 0x20;
    }

    namespace Camera {
         inline constexpr uintptr_t CameraSubject = 0xe8;
         inline constexpr uintptr_t CameraType = 0x158;
         inline constexpr uintptr_t FieldOfView = 0x160;
         inline constexpr uintptr_t Position = 0x11c;
         inline constexpr uintptr_t Rotation = 0xf8;
         inline constexpr uintptr_t Viewport = 0x2ac;
         inline constexpr uintptr_t ViewportSize = 0x2e8;
    }

    namespace CharacterMesh {
         inline constexpr uintptr_t BaseTextureId = 0xe0;
         inline constexpr uintptr_t BodyPart = 0x160;
         inline constexpr uintptr_t MeshId = 0x110;
         inline constexpr uintptr_t OverlayTextureId = 0x140;
    }

    namespace ClickDetector {
         inline constexpr uintptr_t MaxActivationDistance = 0x100;
         inline constexpr uintptr_t MouseIcon = 0xe0;
    }

    namespace Clothing {
         inline constexpr uintptr_t Color3 = 0x128;
         inline constexpr uintptr_t Template = 0x108;
    }

    namespace ColorCorrectionEffect {
         inline constexpr uintptr_t Brightness = 0xdc;
         inline constexpr uintptr_t Contrast = 0xe0;
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t TintColor = 0xd0;
    }

    namespace ColorGradingEffect {
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t TonemapperPreset = 0xd0;
    }

    namespace DataModel {
         inline constexpr uintptr_t CreatorId = 0x188;
         inline constexpr uintptr_t GameId = 0x190;
         inline constexpr uintptr_t GameLoaded = 0x5f8;
         inline constexpr uintptr_t JobId = 0x138;
         inline constexpr uintptr_t PlaceId = 0x198;
         inline constexpr uintptr_t PlaceVersion = 0x1b4;
         inline constexpr uintptr_t PrimitiveCount = 0x438;
         inline constexpr uintptr_t ScriptContext = 0x3f0;
         inline constexpr uintptr_t ServerIP = 0x5e0;
         inline constexpr uintptr_t Workspace = 0x178;
    }

    namespace DepthOfFieldEffect {
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t FarIntensity = 0xd0;
         inline constexpr uintptr_t FocusDistance = 0xd4;
         inline constexpr uintptr_t InFocusRadius = 0xd8;
         inline constexpr uintptr_t NearIntensity = 0xdc;
    }

    namespace FakeDataModel {
         inline constexpr uintptr_t Pointer = 0x7e83168;
         inline constexpr uintptr_t RealDataModel = 0x1c0;
    }

    namespace GuiBase2D {
         inline constexpr uintptr_t AbsolutePosition = 0x110;
         inline constexpr uintptr_t AbsoluteRotation = 0x188;
         inline constexpr uintptr_t AbsoluteSize = 0x118;
    }

    namespace GuiObject {
         inline constexpr uintptr_t BackgroundColor3 = 0x538;
         inline constexpr uintptr_t BackgroundTransparency = 0x544;
         inline constexpr uintptr_t BorderColor3 = 0x544;
         inline constexpr uintptr_t Image = 0x9f0;
         inline constexpr uintptr_t LayoutOrder = 0x574;
         inline constexpr uintptr_t Position = 0x508;
         inline constexpr uintptr_t RichText = 0xa98;
         inline constexpr uintptr_t Rotation = 0x188;
         inline constexpr uintptr_t ScreenGui_Enabled = 0x4bc;
         inline constexpr uintptr_t Size = 0x528;
         inline constexpr uintptr_t Text = 0xdf8;
         inline constexpr uintptr_t TextColor3 = 0xea8;
         inline constexpr uintptr_t Visible = 0x5a1;
         inline constexpr uintptr_t ZIndex = 0x598;
    }

    namespace Humanoid {
         inline constexpr uintptr_t AutoJumpEnabled = 0x1d8;
         inline constexpr uintptr_t AutoRotate = 0x1d9;
         inline constexpr uintptr_t BreakJointsOnDeath = 0x1db;
         inline constexpr uintptr_t CameraOffset = 0x140;
         inline constexpr uintptr_t DisplayDistanceType = 0x18c;
         inline constexpr uintptr_t DisplayName = 0xd0;
         inline constexpr uintptr_t EvaluateStateMachine = 0x1dc;
         inline constexpr uintptr_t FloorMaterial = 0x190;
         inline constexpr uintptr_t Health = 0x194;
         inline constexpr uintptr_t HealthDisplayDistance = 0x198;
         inline constexpr uintptr_t HealthDisplayType = 0x19c;
         inline constexpr uintptr_t HipHeight = 0x1a0;
         inline constexpr uintptr_t HumanoidRootPart = 0x4c0;
         inline constexpr uintptr_t HumanoidState = 0x8d8;
         inline constexpr uintptr_t HumanoidStateID = 0x20;
         inline constexpr uintptr_t IsWalking = 0x956;
         inline constexpr uintptr_t Jump = 0x1dd;
         inline constexpr uintptr_t JumpHeight = 0x1ac;
         inline constexpr uintptr_t JumpPower = 0x1b0;
         inline constexpr uintptr_t MaxHealth = 0x1b4;
         inline constexpr uintptr_t MaxSlopeAngle = 0x1b8;
         inline constexpr uintptr_t MoveDirection = 0x158;
         inline constexpr uintptr_t MoveToPart = 0x130;
         inline constexpr uintptr_t MoveToPoint = 0x17c;
         inline constexpr uintptr_t NameDisplayDistance = 0x1bc;
         inline constexpr uintptr_t NameOcclusion = 0x1c0;
         inline constexpr uintptr_t PlatformStand = 0x1df;
         inline constexpr uintptr_t RequiresNeck = 0x1e0;
         inline constexpr uintptr_t RigType = 0x1c8;
         inline constexpr uintptr_t SeatPart = 0x120;
         inline constexpr uintptr_t Sit = 0x1e0;
         inline constexpr uintptr_t TargetPoint = 0x164;
         inline constexpr uintptr_t Walkspeed = 0x1d4;
         inline constexpr uintptr_t WalkspeedCheck = 0x3c0;
    }

    namespace Instance {
         inline constexpr uintptr_t AttributeContainer = 0x48;
         inline constexpr uintptr_t AttributeList = 0x18;
         inline constexpr uintptr_t AttributeToNext = 0x58;
         inline constexpr uintptr_t AttributeToValue = 0x18;
         inline constexpr uintptr_t ChildrenEnd = 0x8;
         inline constexpr uintptr_t ChildrenStart = 0x70;
         inline constexpr uintptr_t ClassBase = 0x10b0;
         inline constexpr uintptr_t ClassDescriptor = 0x18;
         inline constexpr uintptr_t ClassName = 0x8;
         inline constexpr uintptr_t Name = 0xb0;
         inline constexpr uintptr_t Parent = 0x68;
         inline constexpr uintptr_t This = 0x8;
    }

    namespace Lighting {
         inline constexpr uintptr_t Ambient = 0xd8;
         inline constexpr uintptr_t Brightness = 0x120;
         inline constexpr uintptr_t ClockTime = 0x1b8;
         inline constexpr uintptr_t ColorShift_Bottom = 0xf0;
         inline constexpr uintptr_t ColorShift_Top = 0xe4;
         inline constexpr uintptr_t EnvironmentDiffuseScale = 0x124;
         inline constexpr uintptr_t EnvironmentSpecularScale = 0x128;
         inline constexpr uintptr_t ExposureCompensation = 0x12c;
         inline constexpr uintptr_t FogColor = 0xfc;
         inline constexpr uintptr_t FogEnd = 0x134;
         inline constexpr uintptr_t FogStart = 0x138;
         inline constexpr uintptr_t GeographicLatitude = 0x190;
         inline constexpr uintptr_t GlobalShadows = 0x148;
         inline constexpr uintptr_t GradientBottom = 0x194;
         inline constexpr uintptr_t GradientTop = 0x150;
         inline constexpr uintptr_t LightColor = 0x15c;
         inline constexpr uintptr_t LightDirection = 0x168;
         inline constexpr uintptr_t MoonPosition = 0x184;
         inline constexpr uintptr_t OutdoorAmbient = 0x108;
         inline constexpr uintptr_t Sky = 0x1d8;
         inline constexpr uintptr_t Source = 0x174;
         inline constexpr uintptr_t SunPosition = 0x178;
    }

    namespace LocalScript {
         inline constexpr uintptr_t ByteCode = 0x1a8;
         inline constexpr uintptr_t GUID = 0xe8;
         inline constexpr uintptr_t Hash = 0x1b8;
    }

    namespace MaterialColors {
         inline constexpr uintptr_t Asphalt = 0x30;
         inline constexpr uintptr_t Basalt = 0x27;
         inline constexpr uintptr_t Brick = 0xf;
         inline constexpr uintptr_t Cobblestone = 0x33;
         inline constexpr uintptr_t Concrete = 0xc;
         inline constexpr uintptr_t CrackedLava = 0x2d;
         inline constexpr uintptr_t Glacier = 0x1b;
         inline constexpr uintptr_t Grass = 0x6;
         inline constexpr uintptr_t Ground = 0x2a;
         inline constexpr uintptr_t Ice = 0x36;
         inline constexpr uintptr_t LeafyGrass = 0x39;
         inline constexpr uintptr_t Limestone = 0x3f;
         inline constexpr uintptr_t Mud = 0x24;
         inline constexpr uintptr_t Pavement = 0x42;
         inline constexpr uintptr_t Rock = 0x18;
         inline constexpr uintptr_t Salt = 0x3c;
         inline constexpr uintptr_t Sand = 0x12;
         inline constexpr uintptr_t Sandstone = 0x21;
         inline constexpr uintptr_t Slate = 0x9;
         inline constexpr uintptr_t Snow = 0x1e;
         inline constexpr uintptr_t WoodPlanks = 0x15;
    }

    namespace MeshPart {
         inline constexpr uintptr_t MeshId = 0x2e8;
         inline constexpr uintptr_t Texture = 0x318;
    }

    namespace Misc {
         inline constexpr uintptr_t Adornee = 0x108;
         inline constexpr uintptr_t AnimationId = 0xd0;
         inline constexpr uintptr_t StringLength = 0x10;
         inline constexpr uintptr_t Value = 0xd0;
    }

    namespace Model {
         inline constexpr uintptr_t PrimaryPart = 0x278;
         inline constexpr uintptr_t Scale = 0x164;
    }

    namespace ModuleScript {
         inline constexpr uintptr_t ByteCode = 0x150;
         inline constexpr uintptr_t GUID = 0xe8;
         inline constexpr uintptr_t Hash = 0x160;
    }

    namespace MouseService {
         inline constexpr uintptr_t InputObject = 0x100;
         inline constexpr uintptr_t MousePosition = 0xec;
         inline constexpr uintptr_t SensitivityPointer = 0x7ef6f60;
    }

    namespace Player {
         inline constexpr uintptr_t CameraMode = 0x318;
         inline constexpr uintptr_t Country = 0x110;
         inline constexpr uintptr_t DisplayName = 0x130;
         inline constexpr uintptr_t Gender = 0x0;
         inline constexpr uintptr_t HealthDisplayDistance = 0x338;
         inline constexpr uintptr_t LocalPlayer = 0x130;
         inline constexpr uintptr_t MaxZoomDistance = 0x310;
         inline constexpr uintptr_t MinZoomDistance = 0x314;
         inline constexpr uintptr_t ModelInstance = 0x380;
         inline constexpr uintptr_t Mouse = 0xf78;
         inline constexpr uintptr_t NameDisplayDistance = 0x344;
         inline constexpr uintptr_t Team = 0x290;
         inline constexpr uintptr_t UserId = 0x2b8;
    }

    namespace PlayerConfigurer {
         inline constexpr uintptr_t Pointer = 0x7e60b10;
    }

    namespace PlayerMouse {
         inline constexpr uintptr_t Icon = 0xe0;
         inline constexpr uintptr_t Workspace = 0x168;
    }

    namespace Primitive {
         inline constexpr uintptr_t AssemblyAngularVelocity = 0xfc;
         inline constexpr uintptr_t AssemblyLinearVelocity = 0xf0;
         inline constexpr uintptr_t Flags = 0x1ae;
         inline constexpr uintptr_t Material = 0x0;
         inline constexpr uintptr_t Owner = 0x210;
         inline constexpr uintptr_t Position = 0xe4;
         inline constexpr uintptr_t Rotation = 0xc0;
         inline constexpr uintptr_t Size = 0x1b0;
         inline constexpr uintptr_t Validate = 0x6;
    }

    namespace PrimitiveFlags {
         inline constexpr uintptr_t Anchored = 0x2;
         inline constexpr uintptr_t CanCollide = 0x8;
         inline constexpr uintptr_t CanTouch = 0x10;
    }

    namespace ProximityPrompt {
         inline constexpr uintptr_t ActionText = 0xd0;
         inline constexpr uintptr_t Enabled = 0x156;
         inline constexpr uintptr_t GamepadKeyCode = 0x13c;
         inline constexpr uintptr_t HoldDuration = 0x140;
         inline constexpr uintptr_t KeyCode = 0x144;
         inline constexpr uintptr_t MaxActivationDistance = 0x148;
         inline constexpr uintptr_t ObjectText = 0xf0;
         inline constexpr uintptr_t RequiresLineOfSight = 0x157;
    }

    namespace RenderJob {
         inline constexpr uintptr_t FakeDataModel = 0x38;
         inline constexpr uintptr_t RealDataModel = 0x1b0;
         inline constexpr uintptr_t RenderView = 0x1d0;
    }

    namespace RenderView {
         inline constexpr uintptr_t DeviceD3D11 = 0x8;
         inline constexpr uintptr_t LightingValid = 0x148;
         inline constexpr uintptr_t SkyValid = 0x2cd;
         inline constexpr uintptr_t VisualEngine = 0x10;
    }

    namespace RunService {
         inline constexpr uintptr_t HeartbeatFPS = 0xb8;
         inline constexpr uintptr_t HeartbeatTask = 0xe8;
    }

    namespace Seat {
         inline constexpr uintptr_t Occupant = 0x220;
    }

    namespace Sky {
         inline constexpr uintptr_t MoonAngularSize = 0x25c;
         inline constexpr uintptr_t MoonTextureId = 0xe0;
         inline constexpr uintptr_t SkyboxBk = 0x110;
         inline constexpr uintptr_t SkyboxDn = 0x140;
         inline constexpr uintptr_t SkyboxFt = 0x170;
         inline constexpr uintptr_t SkyboxLf = 0x1a0;
         inline constexpr uintptr_t SkyboxOrientation = 0x250;
         inline constexpr uintptr_t SkyboxRt = 0x1d0;
         inline constexpr uintptr_t SkyboxUp = 0x200;
         inline constexpr uintptr_t StarCount = 0x260;
         inline constexpr uintptr_t SunAngularSize = 0x254;
         inline constexpr uintptr_t SunTextureId = 0x230;
    }

    namespace Sound {
         inline constexpr uintptr_t Looped = 0x152;
         inline constexpr uintptr_t PlaybackSpeed = 0x130;
         inline constexpr uintptr_t RollOffMaxDistance = 0x134;
         inline constexpr uintptr_t RollOffMinDistance = 0x138;
         inline constexpr uintptr_t SoundGroup = 0x100;
         inline constexpr uintptr_t SoundId = 0xe0;
         inline constexpr uintptr_t Volume = 0x144;
    }

    namespace SpawnLocation {
         inline constexpr uintptr_t AllowTeamChangeOnTouch = 0x45;
         inline constexpr uintptr_t Enabled = 0x1f9;
         inline constexpr uintptr_t ForcefieldDuration = 0x1f0;
         inline constexpr uintptr_t Neutral = 0x1fa;
         inline constexpr uintptr_t TeamColor = 0x1f4;
    }

    namespace SpecialMesh {
         inline constexpr uintptr_t MeshId = 0x108;
         inline constexpr uintptr_t Scale = 0xdc;
    }

    namespace StatsItem {
         inline constexpr uintptr_t Value = 0xc8;
    }

    namespace SunRaysEffect {
         inline constexpr uintptr_t Enabled = 0xc8;
         inline constexpr uintptr_t Intensity = 0xd0;
         inline constexpr uintptr_t Spread = 0xd4;
    }

    namespace TaskScheduler {
         inline constexpr uintptr_t JobEnd = 0xd0;
         inline constexpr uintptr_t JobName = 0x18;
         inline constexpr uintptr_t JobStart = 0xc8;
         inline constexpr uintptr_t MaxFPS = 0xb0;
         inline constexpr uintptr_t Pointer = 0x7f25e08;
    }

    namespace Team {
         inline constexpr uintptr_t BrickColor = 0xd0;
    }

    namespace Terrain {
         inline constexpr uintptr_t GrassLength = 0x1f8;
         inline constexpr uintptr_t MaterialColors = 0x280;
         inline constexpr uintptr_t WaterColor = 0x1e8;
         inline constexpr uintptr_t WaterReflectance = 0x200;
         inline constexpr uintptr_t WaterTransparency = 0x204;
         inline constexpr uintptr_t WaterWaveSize = 0x208;
         inline constexpr uintptr_t WaterWaveSpeed = 0x20c;
    }

    namespace Textures {
         inline constexpr uintptr_t Decal_Texture = 0x198;
         inline constexpr uintptr_t Texture_Texture = 0x198;
    }

    namespace Tool {
         inline constexpr uintptr_t CanBeDropped = 0xdd;
         inline constexpr uintptr_t Enabled = 0x345;
         inline constexpr uintptr_t Grip = 0x494;
         inline constexpr uintptr_t ManualActivationOnly = 0x4a2;
         inline constexpr uintptr_t RequiresHandle = 0x34d;
         inline constexpr uintptr_t TextureId = 0x348;
         inline constexpr uintptr_t Tooltip = 0x450;
    }

    namespace VisualEngine {
         inline constexpr uintptr_t Dimensions = 0x720;
         inline constexpr uintptr_t FakeDataModel = 0x700;
         inline constexpr uintptr_t Pointer = 0x7a36cd8;
         inline constexpr uintptr_t RenderView = 0x800;
         inline constexpr uintptr_t ViewMatrix = 0x120;
    }

    namespace Workspace {
         inline constexpr uintptr_t CurrentCamera = 0x460;
         inline constexpr uintptr_t DistributedGameTime = 0x480;
         inline constexpr uintptr_t ReadOnlyGravity = 0x940;
         inline constexpr uintptr_t World = 0x3d8;
    }

    namespace World {
         inline constexpr uintptr_t AirProperties = 0x1d8;
         inline constexpr uintptr_t FallenPartsDestroyHeight = 0x1c8;
         inline constexpr uintptr_t Gravity = 0x1d0;
         inline constexpr uintptr_t Primitives = 0x240;
         inline constexpr uintptr_t worldStepsPerSec = 0x660;
    }

}
