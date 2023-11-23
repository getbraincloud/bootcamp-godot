// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;

public class Leaderboard
{
    private string m_Name;
    private List<LeaderboardEntry> m_Leaderboard;

    public string Name
    {
        get { return m_Name; }
    }

    public Leaderboard(string name, List<LeaderboardEntry> leaderboard)
    {
        m_Name = name;
        m_Leaderboard = leaderboard;
    }

    public LeaderboardEntry GetLeaderboardEntryAtIndex(int index)
    {
        if (index >= 0 && index < GetCount())
            return m_Leaderboard[index];
        return null;
    }

    public int GetCount()
    {
        return m_Leaderboard.Count;
    }
}