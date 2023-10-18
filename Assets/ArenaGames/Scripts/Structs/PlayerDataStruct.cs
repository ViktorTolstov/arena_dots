using System.Collections.Generic;

namespace ArenaGames
{
    [System.Serializable]
    public class PlayfabInfoStruct
    {
        public string playfabId;
        public string token;
        public long tokenExpired;
    }

    [System.Serializable]
    public class PlayerInfoStruct
    {
        public string id;
        public PlayfabInfoStruct playfab;
        public string username;
        public string email;
    }

    [System.Serializable]
    public class CurrencyInfoStruct
    {
        public string currencyId;
        public string symbol;
        public float balanceAmount;
    }

    [System.Serializable]
    public class CurrenciesData
    {
        public List<CurrencyInfoStruct> CurrencyInfo;
    }
}