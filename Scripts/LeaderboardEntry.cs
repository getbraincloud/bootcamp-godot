// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;

public class LeaderboardEntry
{
    private string m_Nickname;
    private double m_Time;
    private int m_Rank;
    private bool m_IsUserScore;


    public LeaderboardEntry(string nickname, int rank, double time)
    {
        m_Nickname = nickname;
        m_Rank = rank;
        m_Time = time;
    }

    public string Nickname
    {
        get { return m_Nickname; }
        set { m_Nickname = value; }
    }

    public double Time
    {
        get { return m_Time; }
        set { m_Time = value; }
    }

    public int Rank
    {
        get { return m_Rank; }
        set { m_Rank = value; }
    }

    public bool IsUserScore
    {
        get { return m_IsUserScore; }
        set { m_IsUserScore = value; }
    }
}
