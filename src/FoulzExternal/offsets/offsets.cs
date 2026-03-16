using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using FoulzExternal.storage;
using FoulzExternal.logging;

// Yes, this does ping to my site(axiom softworks) BUT these are THEO's OFFSETS!!! He dumps them, I only have them posted on my site because some people can have problems connecting to his.

namespace Offsets
{
    public static class Info
    {
        public static string ClientVersion = "Fetching...";
    }

    public static class AirProperties { public static long AirDensity, GlobalWind; }
    public static class AnimationTrack { public static long Animation, Animator, IsPlaying, Looped, Speed, TimePosition; }
    public static class Animator { public static long ActiveAnimations; }
    public static class Atmosphere { public static long Color, Decay, Density, Glare, Haze, Offset; }
    public static class Attachment { public static long Position; }
    public static class BasePart { public static long CastShadow, Color3, Locked, Massless, Primitive, Reflectance, Shape, Transparency; }
    public static class Beam { public static long Attachment0, Attachment1, Brightness, CurveSize0, CurveSize1, LightEmission, LightInfluence, Texture, TextureLength, TextureSpeed, Width0, Width1, ZOffset; }
    public static class BloomEffect { public static long Enabled, Intensity, Size, Threshold; }
    public static class BlurEffect { public static long Enabled, Size; }
    public static class ByteCode { public static long Pointer, Size; }
    public static class Camera { public static long CameraSubject, CameraType, FieldOfView, Position, Rotation, Viewport, ViewportSize; }
    public static class CharacterMesh { public static long BaseTextureId, BodyPart, MeshId, OverlayTextureId; }
    public static class ClickDetector { public static long MaxActivationDistance, MouseIcon; }
    public static class Clothing { public static long Color3, Template; }
    public static class ColorCorrectionEffect { public static long Brightness, Contrast, Enabled, TintColor; }
    public static class ColorGradingEffect { public static long Enabled, TonemapperPreset; }
    public static class DataModel { public static long CreatorId, GameId, GameLoaded, JobId, PlaceId, PlaceVersion, PrimitiveCount, ScriptContext, ServerIP, ToRenderView1, ToRenderView2, ToRenderView3, Workspace; }
    public static class DepthOfFieldEffect { public static long Enabled, FarIntensity, FocusDistance, InFocusRadius, NearIntensity; }
    public static class FakeDataModel { public static long Pointer, RealDataModel; }
    public static class GuiBase2D { public static long AbsolutePosition, AbsoluteRotation, AbsoluteSize; }
    public static class GuiObject { public static long BackgroundColor3, BackgroundTransparency, BorderColor3, Image, LayoutOrder, Position, RichText, Rotation, ScreenGui_Enabled, Size, Text, TextColor3, Visible, ZIndex; }
    public static class Humanoid { public static long AutoJumpEnabled, AutoRotate, AutomaticScalingEnabled, BreakJointsOnDeath, CameraOffset, DisplayDistanceType, DisplayName, EvaluateStateMachine, FloorMaterial, Health, HealthDisplayDistance, HealthDisplayType, HipHeight, HumanoidRootPart, HumanoidState, HumanoidStateID, IsWalking, Jump, JumpHeight, JumpPower, MaxHealth, MaxSlopeAngle, MoveDirection, MoveToPart, MoveToPoint, NameDisplayDistance, NameOcclusion, PlatformStand, RequiresNeck, RigType, SeatPart, Sit, TargetPoint, UseJumpPower, WalkTimer, Walkspeed, WalkspeedCheck; }
    public static class Instance { public static long AttributeContainer, AttributeList, AttributeToNext, AttributeToValue, ChildrenEnd, ChildrenStart, ClassBase, ClassDescriptor, ClassName, Name, Parent, This; }
    public static class Lighting { public static long Ambient, Brightness, ClockTime, ColorShift_Bottom, ColorShift_Top, EnvironmentDiffuseScale, EnvironmentSpecularScale, ExposureCompensation, FogColor, FogEnd, FogStart, GeographicLatitude, GlobalShadows, GradientBottom, GradientTop, LightColor, LightDirection, MoonPosition, OutdoorAmbient, Sky, Source, SunPosition; }
    public static class LocalScript { public static long ByteCode, GUID, Hash; }
    public static class MaterialColors { public static long Asphalt, Basalt, Brick, Cobblestone, Concrete, CrackedLava, Glacier, Grass, Ground, Ice, LeafyGrass, Limestone, Mud, Pavement, Rock, Salt, Sand, Sandstone, Slate, Snow, WoodPlanks; }
    public static class MeshPart { public static long MeshId, Texture; }
    public static class Misc { public static long Adornee, AnimationId, StringLength, Value; }
    public static class Model { public static long PrimaryPart, Scale; }
    public static class ModuleScript { public static long ByteCode, GUID, Hash, IsCoreScript; }
    public static class MouseService { public static long InputObject, InputObject2, MousePosition, SensitivityPointer; }
    public static class ParticleEmitter { public static long Acceleration, Brightness, Drag, Lifetime, LightEmission, LightInfluence, Rate, RotSpeed, Rotation, Speed, SpreadAngle, Texture, TimeScale, VelocityInheritance, ZOffset; }
    public static class Player { public static long AccountAge, CameraMode, DisplayName, HealthDisplayDistance, LocalPlayer, LocaleId, MaxZoomDistance, MinZoomDistance, ModelInstance, Mouse, NameDisplayDistance, Team, TeamColor, UserId; }
    public static class PlayerConfigurer { public static long Pointer; }
    public static class PlayerMouse { public static long Icon, Workspace; }
    public static class Primitive { public static long AssemblyAngularVelocity, AssemblyLinearVelocity, Flags, Material, Owner, Position, Rotation, Size, Validate; }
    public static class PrimitiveFlags { public static long Anchored, CanCollide, CanQuery, CanTouch; }
    public static class ProximityPrompt { public static long ActionText, Enabled, GamepadKeyCode, HoldDuration, KeyCode, MaxActivationDistance, ObjectText, RequiresLineOfSight; }
    public static class RenderJob { public static long FakeDataModel, RealDataModel, RenderView; }
    public static class RenderView { public static long DeviceD3D11, LightingValid, SkyValid, VisualEngine; }
    public static class RunService { public static long HeartbeatFPS, HeartbeatTask; }
    public static class Script { public static long ByteCode, GUID, Hash; }
    public static class ScriptContext { public static long RequireBypass; }
    public static class Seat { public static long Occupant; }
    public static class Sky { public static long MoonAngularSize, MoonTextureId, SkyboxBk, SkyboxDn, SkyboxFt, SkyboxLf, SkyboxOrientation, SkyboxRt, SkyboxUp, StarCount, SunAngularSize, SunTextureId; }
    public static class Sound { public static long Looped, PlaybackSpeed, RollOffMaxDistance, RollOffMinDistance, SoundGroup, SoundId, Volume; }
    public static class SpawnLocation { public static long AllowTeamChangeOnTouch, Enabled, ForcefieldDuration, Neutral, TeamColor; }
    public static class SpecialMesh { public static long MeshId, Scale; }
    public static class StatsItem { public static long Value; }
    public static class SunRaysEffect { public static long Enabled, Intensity, Spread; }
    public static class SurfaceAppearance { public static long AlphaMode, Color, ColorMap, EmissiveMaskContent, EmissiveStrength, EmissiveTint, MetalnessMap, NormalMap, RoughnessMap; }
    public static class TaskScheduler { public static long JobEnd, JobName, JobStart, MaxFPS, Pointer; }
    public static class Team { public static long BrickColor; }
    public static class Terrain { public static long GrassLength, MaterialColors, WaterColor, WaterReflectance, WaterTransparency, WaterWaveSize, WaterWaveSpeed; }
    public static class Textures { public static long Decal_Texture, Texture_Texture; }
    public static class Tool { public static long CanBeDropped, Enabled, Grip, ManualActivationOnly, RequiresHandle, TextureId, Tooltip; }
    public static class UserInputService { public static long WindowInputState; }
    public static class VehicleSeat { public static long MaxSpeed, SteerFloat, ThrottleFloat, Torque, TurnSpeed; }
    public static class VisualEngine { public static long Dimensions, FakeDataModel, Pointer, RenderView, ViewMatrix; }
    public static class Weld { public static long Part0, Part1; }
    public static class WeldConstraint { public static long Part0, Part1; }
    public static class WindowInputState { public static long CapsLock, CurrentTextBox; }
    public static class Workspace { public static long CurrentCamera, DistributedGameTime, ReadOnlyGravity, World; }
    public static class World { public static long AirProperties, FallenPartsDestroyHeight, Gravity, Primitives, worldStepsPerSec; }

    public static class Sync
    {
        public static bool Fetch()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string fullVersion = "version-" + Storage.RobloxVersion;
                    string url = $"https://axiomsoftworks.com/Offsets/{fullVersion}/Offsets.cs";
                    string fallbackUrl = "https://axiomsoftworks.com/Offsets/Offsets.cs";

                    string data = "";
                    try
                    {
                        data = client.DownloadString(url);
                    }
                    catch
                    {
                        data = client.DownloadString(fallbackUrl);
                    }

                    if (string.IsNullOrEmpty(data)) return false;

                    var vMatch = Regex.Match(data, @"ClientVersion = ""([^""]+)""");
                    if (vMatch.Success) Info.ClientVersion = vMatch.Groups[1].Value;

                    Assembly assembly = typeof(Info).Assembly;
                    var classMatches = Regex.Matches(data, @"public static class (\w+)\s*\{([\s\S]*?)\}");

                    foreach (Match classMatch in classMatches)
                    {
                        string className = classMatch.Groups[1].Value;
                        string classContent = classMatch.Groups[2].Value;
                        Type targetType = assembly.GetType("Offsets." + className);
                        if (targetType == null) continue;

                        var fieldMatches = Regex.Matches(classContent, @"public const long\s+(\w+)\s*=\s*(0x[0-9a-fA-F]+|\d+);");
                        foreach (Match fieldMatch in fieldMatches)
                        {
                            string fieldName = fieldMatch.Groups[1].Value;
                            string valStr = fieldMatch.Groups[2].Value;
                            long val = valStr.StartsWith("0x") ? Convert.ToInt64(valStr, 16) : Convert.ToInt64(valStr);

                            FieldInfo field = targetType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
                            if (field != null) field.SetValue(null, val);
                        }
                    }
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
