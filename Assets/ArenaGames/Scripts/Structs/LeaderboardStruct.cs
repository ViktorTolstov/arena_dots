using System.Collections.Generic;

public class AroundLeaderboard
{
    public string profileId;
    public string username;
    public int position;
    public int score;
    public object createdAt;
}

public class Leaderboard
{
    public string profileId;
    public string username;
    public int position;
    public int score;
    public object createdAt;
}

public class LeaderboardsStruct
{
    public List<Leaderboard> leaderboards = new List<Leaderboard>();
    public List<AroundLeaderboard> aroundLeaderboards = new List<AroundLeaderboard>();
}