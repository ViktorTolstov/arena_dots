using UnityEngine;

namespace ArenaGames
{
    public static class AGCore
    {
        public static bool IsInitialized = false;
        public static ArenaGamesSettings Settings;

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                Settings = (ArenaGamesSettings)Resources.Load("AGSettings", typeof(ArenaGamesSettings));
                AGHelperURIs.UpdateUris(Settings.IsProd, Settings.GameAlias);
                AGErrorMessages.Initialize();
                IsInitialized = true;
            }
        }
    }
}