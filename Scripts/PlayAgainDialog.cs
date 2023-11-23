// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class PlayAgainDialog : Dialog
{
	[Export] private TextureButton _NewGameButton;
	[Export] private TextureButton _BrainCloudButton;
	[Export] private TextureButton _MainMenuButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_NewGameButton.Connect("pressed", new Callable(this, "OnPlayAgainButtonClicked"));
		_BrainCloudButton.Connect("pressed", new Callable(this, "OnBrainCloudButtonClicked"));
		_MainMenuButton.Connect("pressed", new Callable(this, "OnMainMenuButtonClicked"));

		base._Ready();
	}

	protected override void OnShow()
	{
		if (GetNetwork.IsAuthenticated())
			_BrainCloudButton.Disabled = false;
		else
			_BrainCloudButton.Disabled = true;
	}

	public void OnPlayAgainButtonClicked()
	{
		Game.sharedInstance.Reset(true);
		Hide();
	}

	public void OnBrainCloudButtonClicked()
	{
		DialogManager.sharedInstance.ShowBrainCloudDialog();
	}

	public void OnMainMenuButtonClicked()
	{
		Game.sharedInstance.Reset(false);
		Hide();
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
