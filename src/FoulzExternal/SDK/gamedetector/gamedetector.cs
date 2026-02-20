using System;
using FoulzExternal.storage;
using System.Diagnostics;

// this is my game detector i ues for my C++ code, but implemented into C#, so have fun!

namespace FoulzExternal.SDK.gamedetector
{
    internal enum GameType
    {
        checking,
        unknownshi,
        pf,
        rivals,
        universal
    }

    internal static class finder
    {
        public static GameType whatgame()
        {
            try
            {
                var dm = Storage.DataModelInstance;
                if (!dm.IsValid) return GameType.checking;

                long id = dm.GetGameID();
                if (id == 0) return GameType.checking;

                if (id == 113491250) return GameType.pf;
                if (id == 6035872082) return GameType.rivals;

                return GameType.universal;
            }
            catch { return GameType.checking; }
        }

        public static string namethis(GameType g)
        {
            switch (g)
            {
                case GameType.checking: return "Detecting...";
                case GameType.pf: return "Phantom Forces";
                case GameType.rivals: return "Rivals";
                case GameType.universal: return "Universal";
                default: return "Unknown";
            }
        }

        public static bool isrunnin(string p)
        {
            try
            {
                var list = Process.GetProcessesByName(p);
                return list != null && list.Length > 0;
            }
            catch { return false; }
        }

        public static bool iswindow(string t) => false;
    }
}