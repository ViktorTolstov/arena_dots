using System.Collections.Generic;

public class AchievementEntryStruct
{
    public string alias;
    public object dateReceipt;
    public string description;
    public string name;
    public bool completed;
}

public class AchievementsStruct
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

