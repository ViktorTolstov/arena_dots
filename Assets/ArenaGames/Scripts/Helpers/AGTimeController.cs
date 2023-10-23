using System;

namespace ArenaGames
{
    public static class AGTimeController
    {
        public static long TimestampMS => ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds();
        public static int Timestamp => (int) ((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds();
    }
}