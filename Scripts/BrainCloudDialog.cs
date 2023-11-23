// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class BrainCloudDialog : Dialog
{
	[Export] private TextureButton _AttachEmailButton;
	[Export] private TextureButton _ChangeUsernameButton;
	[Export] private TextureButton _LeaderboardsButton;
	[Export] private TextureButton _StatisticsButton;
	[Export] private TextureButton _AchievementsButton;
	[Export] private TextureButton _LogOutButton;
	[Export] private TextureButton _LogInButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AttachEmailButton.Connect("pressed", new Callable(this, "OnAttachEmailClicked"));
		_ChangeUsernameButton.Connect("pressed", new Callable(this, "OnSetUsernameButtonClicked"));
		_LeaderboardsButton.Connect("pressed", new Callable(this, "OnLeaderboardsButtonClicked"));
		_StatisticsButton.Connect("pressed", new Callable(this, "OnStatisticsButtonClicked"));
		_AchievementsButton.Connect("pressed", new Callable(this, "OnAchievementsButtonClicked"));
		_LogOutButton.Connect("pressed", new Callable(this, "OnLogOutButtonClicked"));
		_LogInButton.Connect("pressed", new Callable(this, "OnLogInButtonClicked"));

		base._Ready();
	}

	protected override void OnShow()
	{
		Refresh();
	}

	public void OnAttachEmailClicked()
	{
		// TODO: Implement attaching an email identity
	}

	public void OnSetUsernameButtonClicked()
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

	public void OnLogOutButtonClicked()
	{
		Game.sharedInstance.GetNetwork.LogOut(OnBrainCloudLogOutCompleted);
	}

	public void OnLogInButtonClicked()
	{
		Hide();
		Game.sharedInstance.HandleAuthentication();
	}

	private void OnBrainCloudLogOutCompleted()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (GetNetwork.IsAuthenticated())
		{
			_AttachEmailButton.Visible = true;
			_ChangeUsernameButton.Visible = true;
			_LeaderboardsButton.Visible = true;
			_StatisticsButton.Visible = true;
			_AchievementsButton.Visible = true;
			_LogOutButton.Visible = true;
			_LogInButton.Visible = false;
		}
		else
		{
			_AttachEmailButton.Visible = false;
			_ChangeUsernameButton.Visible = false;
			_LeaderboardsButton.Visible = false;
			_StatisticsButton.Visible = false;
			_AchievementsButton.Visible = false;
			_LogOutButton.Visible = false;
			_LogInButton.Visible = true;
		}
	}
}
