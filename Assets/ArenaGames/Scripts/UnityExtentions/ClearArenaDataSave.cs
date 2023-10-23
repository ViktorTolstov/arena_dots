using UnityEditor;
using UnityEngine;

namespace ArenaGames.UnityExtentions
{
    public class ClearArenaDataSave
    {
        #if UNITY_EDITOR
        [MenuItem("Arena/Clear Data Save")]
        public static void ClearData()
        {
            PlayerPrefs.DeleteAll();
        }
        #endif
    }
}