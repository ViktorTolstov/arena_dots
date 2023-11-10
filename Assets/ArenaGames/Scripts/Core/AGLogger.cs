using UnityEngine;

namespace ArenaGames.Scripts.Core
{
    public static class AGLogger
    {
        public static void Log(string message)
        {
            if (!AGCore.Settings.DebugMode) return;
            
            Debug.Log(message);
        }
        
        public static void LogWarning(string message)
        {
            if (!AGCore.Settings.DebugMode) return;
            
            Debug.LogWarning(message);
        }
        
        public static void LogError(string message)
        {
            if (!AGCore.Settings.DebugMode) return;
            
            Debug.LogError(message);
        }
    }
}