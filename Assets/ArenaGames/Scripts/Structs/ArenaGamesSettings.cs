using UnityEngine;

namespace ArenaGames
{
    public class ArenaGamesSettings : ScriptableObject
    {
        public string GameName;
        public string GameAlias;
        public string LeaderboardId;
        public string ServerToken;
        public bool IsProd;
        public bool UseWebViewForSignIn;
    }
}