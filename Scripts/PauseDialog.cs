// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class PauseDialog : Dialog
{
	[Export] private TextureButton _ResumeButton;
	[Export] private TextureButton _BrainCloudButton;
	[Export] private TextureButton _MainMenuButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ResumeButton.Connect("pressed", new Callable(this, "OnResumeButtonClicked"));
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

	public void OnResumeButtonClicked()
	{
		Hide();
	}

	public void OnBrainCloudButtonClicked()
	{
		DialogManager.sharedInstance.ShowBrainCloudDialog(true);
	}

	public void OnMainMenuButtonClicked()
	{
		Hide();
		DialogManager.sharedInstance.ShowMainMenuDialog();
		Game.sharedInstance.Reset(false);
	}
}
