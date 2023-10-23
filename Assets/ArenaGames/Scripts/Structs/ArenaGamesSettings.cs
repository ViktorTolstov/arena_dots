using UnityEngine;

namespace ArenaGames
{
    public class ArenaGamesSettings : ScriptableObject
    {
        public string GameName;
        public string GameAlias;
        public bool IsProd;
        public bool UseWebViewForSignIn;
    }
}