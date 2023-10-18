using System;
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