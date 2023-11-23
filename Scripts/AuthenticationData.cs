// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


public class AuthenticationData
{
	Leaderboard m_MainLeaderboard = null;
	Leaderboard m_DailyLeaderboard = null;
	Leaderboard m_CountryLeaderboard = null;
	List<Statistic> m_StatisticsList = null;
	List<LevelData> m_LevelDataList = null;
	List<Achievement> m_AchievementsList = null;
	UserProgress m_UserProgress = null;

	public Leaderboard MainLeaderboard
	{
		get { return m_MainLeaderboard; }
		set { m_MainLeaderboard = value; }
	}

	public Leaderboard DailyLeaderboard
	{
		get { return m_DailyLeaderboard; }
		set { m_DailyLeaderboard = value; }
	}

	public Leaderboard CountryLeaderboard
	{
		get { return m_CountryLeaderboard; }
		set { m_CountryLeaderboard = value; }
	}

	public List<Statistic> StatisticsList
	{
		get { return m_StatisticsList; }
		set { m_StatisticsList = value; }
	}

	public List<LevelData> LevelDataList
	{
		get { return m_LevelDataList; }
		set { m_LevelDataList = value; }
	}

	public List<Achievement> AchievementsList
	{
		get { return m_AchievementsList; }
		set { m_AchievementsList = value; }
	}

	public UserProgress UserProgressData
	{
		get { return m_UserProgress; }
		set { m_UserProgress = value; }
	}
}