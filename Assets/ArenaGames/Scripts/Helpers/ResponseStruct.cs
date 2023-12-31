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
            [JsonProperty("params")] public List<string> messageParams;
        }
        
        [Serializable]
        public class CodeAttemptResponseStruct : BaseStruct
        {
            public string nextAttemptTime;
        }
        
        [Serializable]
        public class StatusStruct : BaseStruct
        {
            public bool ok;
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
        
        [Serializable]
        public class RefreshAuthStruct : BaseStruct
        {
            public TokenStruct accessToken;
        }
        
        [Serializable]
        public class TokenStruct : BaseStruct
        {
            public string token;
            public long expiresIn;
        }
        
        [Serializable]
        public class EventsStruct : BaseStruct
        {
            public List<EventStruct> events;
        }
        
        [Serializable]
        public class EventStruct : BaseStruct
        {
            public string gameId;
            public string userId;
            public string eventType;
            public long timestamp;
        }
        
        [Serializable]
        public class Leaderboard: BaseStruct
        {
            public string profileId;
            public string username;
            public int position;
            public int score;
            public object createdAt;
        }
        
        [Serializable]
        public class LeaderboardsStruct: BaseStruct
        {
            public List<Leaderboard> leaderboards = new ();
            public List<Leaderboard> aroundLeaderboards = new ();
        }

        [Serializable]
        public class AchievementEntryStruct : BaseStruct
        {
            public string alias;
            public object dateReceipt;
            public string description;
            public string name;
            public bool completed;
        }

        [Serializable]
        public class AchievementsStruct : BaseStruct
        {
            public int limit;
            public int offset;
            public int totalDocs;
            public int totalPages;
            public bool hasNextPage;
            public bool hasPrevPage;
            public object nextPage;
            public object prevPage;
            public int page;
            public List<AchievementEntryStruct> docs;
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