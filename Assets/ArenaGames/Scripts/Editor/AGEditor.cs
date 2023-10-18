using UnityEditor;

using UnityEngine;

namespace ArenaGames.Editor
{
    public class AGEditor : EditorWindow
    {
        [MenuItem("Window/Arena Games/Highlight ArenaGames Settings", false, 1)]
        protected static void MenuItemHighlightSettings()
        {
            HighlightSettings();
        }

        private static void HighlightSettings()
        {
            ArenaGamesSettings serverSettings = (ArenaGamesSettings)Resources.Load("AGSettings", typeof(ArenaGamesSettings));
            Selection.objects = new UnityEngine.Object[] { serverSettings };
            EditorGUIUtility.PingObject(serverSettings);
        }
    }
}