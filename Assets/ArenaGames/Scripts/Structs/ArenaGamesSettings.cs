using UnityEngine;

namespace ArenaGames
{
    public class ArenaGamesSettings : ScriptableObject
    {
        [Header("GameSettings")]
        public string GameName;
        public string GameAlias;
        public bool IsProd;
        public bool UseWebViewForSignIn;
        
        [Header("DevSettings")]
        public bool DebugMode;
        public int MaxConnectionErrorsCount = 5;
    }
}