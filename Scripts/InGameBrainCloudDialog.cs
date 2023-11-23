// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class InGameBrainCloudDialog : Dialog
{
	[Export] private TextureButton _ChangeUsernameButton;
	[Export] private TextureButton _LeaderboardsButton;
	[Export] private TextureButton _StatisticsButton;
	[Export] private TextureButton _AchievementsButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ChangeUsernameButton.Connect("pressed", new Callable(this, "OnChangeUsernameButtonClicked"));
		_LeaderboardsButton.Connect("pressed", new Callable(this, "OnLeaderboardsButtonClicked"));
		_StatisticsButton.Connect("pressed", new Callable(this, "OnStatisticsButtonClicked"));
		_AchievementsButton.Connect("pressed", new Callable(this, "OnAchievementsButtonClicked"));

		base._Ready();
	}

	public void OnChangeUsernameButtonClicked()
	{
		DialogManager.sharedInstance.ShowChangeUsernameDialog();
	}

	public void OnLeaderboardsButtonClicked()
	{
		DialogManager.sharedInstance.ShowLeaderboardsDialog();
	}

	public void OnStatisticsButtonClicked()
	{
		DialogManager.sharedInstance.ShowStatisticsDialog();
	}

	public void OnAchievementsButtonClicked()
	{
		DialogManager.sharedInstance.ShowAchievementsDialog();
	}
}
