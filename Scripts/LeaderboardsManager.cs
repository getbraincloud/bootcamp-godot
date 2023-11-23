// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class LeaderboardsManager : Node
{
	private List<Leaderboard> m_Leaderboards = new List<Leaderboard>();
	private double m_UserTime;

	public void AddLeaderboard(ref Leaderboard leaderboard)
	{
		if (m_UserTime > 0.0)
		{
			for (int i = 0; i < leaderboard.GetCount(); i++)
			{
				if (leaderboard.GetLeaderboardEntryAtIndex(i).Time == m_UserTime)
				{
					leaderboard.GetLeaderboardEntryAtIndex(i).IsUserScore = true;
					break;
				}
			}
		}

		// Remove all existing leaderboards with the same name
		string name = leaderboard.Name;
		m_Leaderboards.RemoveAll(p => p.Name == name);

		// Add the Leaderboard object to the list
		m_Leaderboards.Add(leaderboard);
	}

	public Leaderboard GetLeaderboardByName(string name)
	{
		for (int i = 0; i < m_Leaderboards.Count; i++)
		{
			if (m_Leaderboards[i].Name == name)
				return m_Leaderboards[i];
		}
		return null;
	}

	public int GetCount()
	{
		return m_Leaderboards.Count;
	}

	public void SetUserTime(double userTime)
	{
		long ms = (long)(userTime * 1000.0);       // Convert the time from seconds to milleseconds
		m_UserTime = (double)(ms) / 1000.0;
	}
}
