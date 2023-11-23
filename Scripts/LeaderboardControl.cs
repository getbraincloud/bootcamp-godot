// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class LeaderboardControl : Control
{
	[Export] private Label _RankLabel;
	[Export] private Label _UsernameLabel;
	[Export] private Label _TimeLabel;

	public void Reset()
	{
		_RankLabel.Text = "";
		_UsernameLabel.Text = "";
		_TimeLabel.Text = "";
	}

	public void Set(LeaderboardEntry leaderboardEntry)
	{
		_RankLabel.Text = leaderboardEntry.Rank.ToString() + ".";
		_UsernameLabel.Text = leaderboardEntry.Nickname;
		_TimeLabel.Text = TimeSpan.FromSeconds(leaderboardEntry.Time).ToString(@"mm\:ss");

		if(leaderboardEntry.IsUserScore)
		{
			Color green = new Color(207.0f / 255.0f, 198.0f / 255.0f, 0.0f, 1.0f);
			_RankLabel.Modulate = green;
			_UsernameLabel.Modulate = green;
			_TimeLabel.Modulate = green;
		}
		else
		{
			Color white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			_RankLabel.Modulate = white;
			_UsernameLabel.Modulate = white;
			_TimeLabel.Modulate = white;
		}
	}
}
