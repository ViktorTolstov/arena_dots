using System;
using System.Text.RegularExpressions;

namespace ArenaGames
{
    public static class AGRegexHelper
    {
        public static bool TryParseKey(string target, string pattern, out string key)
        {
            var accessMatch = Regex.Match(target, pattern);
            key = accessMatch.Success ? accessMatch.Groups[1].Value : "";
            return accessMatch.Success;
        }
    }
}