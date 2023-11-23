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
}
