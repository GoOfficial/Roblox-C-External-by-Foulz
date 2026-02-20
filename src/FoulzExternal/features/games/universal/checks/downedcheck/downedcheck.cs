using FoulzExternal.SDK.caches;

namespace FoulzExternal.features.games.universal.checks.downedcheck
{
    public static class DownedCheck
    {
        public static bool is_downed(RobloxPlayer p)
        {
            return p.Health <= 4;
        }
    }
}