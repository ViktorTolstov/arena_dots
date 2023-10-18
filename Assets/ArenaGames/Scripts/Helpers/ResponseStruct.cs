using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ArenaGames
{
    public static class ResponseStruct
    {
        [Serializable]
        public class BaseStruct
        {
            
        }
        
        [Serializable]
        public class ErrorResponseStruct : BaseStruct
        {
            public string type;
            public string code;
            public string message;
        }
        
        [Serializable]
        public class CodeAttemptResponseStruct : BaseStruct
        {
            public string nextAttemptTime;
        }
        
        [Serializable]
        public class GameInfoStruct : BaseStruct
        {
            public string id;
            public List<LeaderboardDataStruct> activeLeaderboards;
        }
        
        [Serializable]
        public class LeaderboardDataStruct : BaseStruct
        {
            public string alias;
            public string name;
        }
        
        public static T TryParse<T>(string objectToDeserialize) where T : BaseStruct
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(objectToDeserialize);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}