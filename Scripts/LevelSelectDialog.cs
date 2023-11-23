// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class LevelSelectDialog : Dialog
{
	[Export] private TextureButton _LevelOneButton;
	[Export] private TextureButton _LevelTwoButton;
	[Export] private TextureButton _LevelThreeButton;
	[Export] private TextureButton _BossLevelButton;
	[Export] private TextureButton _MainMenuButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_LevelOneButton.Connect("pressed", new Callable(this, "OnLevelOneButtonClicked"));
		_LevelTwoButton.Connect("pressed", new Callable(this, "OnLevelTwoButtonClicked"));
		_LevelThreeButton.Connect("pressed", new Callable(this, "OnLevelThreeButtonClicked"));
		_BossLevelButton.Connect("pressed", new Callable(this, "OnLevelBossButtonClicked"));
		_MainMenuButton.Connect("pressed", new Callable(this, "OnMainMenuButtonClicked"));

		base._Ready();
	}

	protected override void OnShow()
	{
		_LevelTwoButton.Disabled = !Game.sharedInstance.GetUserProgress().LevelOneCompleted;
		_LevelThreeButton.Disabled = !Game.sharedInstance.GetUserProgress().LevelTwoCompleted;
		_BossLevelButton.Disabled = !Game.sharedInstance.GetUserProgress().LevelThreeCompleted;
	}

	public void OnLevelOneButtonClicked()
	{
		Hide();
		Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelOne);
	}

	public void OnLevelTwoButtonClicked()
	{
		Hide();
		Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelTwo);
	}

	public void OnLevelThreeButtonClicked()
	{
		Hide();
		Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelThree);
	}

	public void OnLevelBossButtonClicked()
	{
		Hide();
		Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelBoss);
	}

	public void OnMainMenuButtonClicked()
	{
		Hide();
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
