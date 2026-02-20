using FoulzExternal.SDK;
using FoulzExternal.SDK.caches;
using FoulzExternal.SDK.gamedetector;
using FoulzExternal.storage;
using Offsets;
using System;
using SDKInstance = FoulzExternal.SDK.Instance;

namespace FoulzExternal.features.games.universal.checks.teamcheck
{
    public static class TeamCheck
    {
        public static bool isteammate(RobloxPlayer target)
        {
            var me = Storage.LocalPlayerInstance;
            if (!me.IsValid || target.address == 0) return false;

            if (me.Address == target.address) return true;

            if (finder.whatgame() == GameType.rivals)
            {
                return rivals_teamcheck(target);
            }

            long myTeam = SDKInstance.Mem.ReadPtr(me.Address + Player.Team);
            long theirTeam = SDKInstance.Mem.ReadPtr(target.address + Player.Team);

            return myTeam != 0 && theirTeam != 0 && myTeam == theirTeam;
        }

        private static bool rivals_teamcheck(RobloxPlayer p)
        {
            return p.TeammateLabel.IsValid;
        }
    }
}