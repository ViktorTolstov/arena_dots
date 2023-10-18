using System;

namespace ArenaGames
{
    public static class AGTimeController
    {
        public static long TimestampMS => ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds();
        public static long Timestamp => ((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds();
    }
}