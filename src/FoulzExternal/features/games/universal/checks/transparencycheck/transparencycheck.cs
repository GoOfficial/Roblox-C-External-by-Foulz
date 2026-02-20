using FoulzExternal.SDK;
using FoulzExternal.SDK.caches;

namespace FoulzExternal.features.games.universal.checks.transparencycheck
{
    public static class TransparencyCheck
    {
        public static bool is_clear(RobloxPlayer p)
        {
            var parts = new[] {
                p.Head, p.Torso, p.HumanoidRootPart,
                p.Left_Arm, p.Right_Arm, p.Left_Leg, p.Right_Leg,
                p.Upper_Torso, p.Lower_Torso,
                p.Left_Upper_Arm, p.Left_Lower_Arm, p.Left_Hand,
                p.Right_Upper_Arm, p.Right_Lower_Arm, p.Right_Hand,
                p.Left_Upper_Leg, p.Left_Lower_Leg, p.Left_Foot,
                p.Right_Upper_Leg, p.Right_Lower_Leg, p.Right_Foot
            };
            foreach (var part in parts)
            {
                if (part.IsValid)
                {
                    float t = Instance.Mem.Read<float>(part.Address + Offsets.BasePart.Transparency);
                    if (t < 1f) return false;
                }
            }
            return true;
        }
    }
}