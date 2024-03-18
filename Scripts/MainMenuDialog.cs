// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class MainMenuDialog : Dialog
{
	[Export] private TextureButton _EndlessModeButton;
	[Export] private TextureButton _HordeModeButton;
	[Export] private TextureButton _BrainCloudButton;
	[Export] private TextureButton _ExitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EndlessModeButton.Connect("pressed", new Callable(this, "OnEndlessButtonClicked"));
		_HordeModeButton.Connect("pressed", new Callable(this, "OnHordeModeButtonClicked"));
		_BrainCloudButton.Connect("pressed", new Callable(this, "OnBrainCloudButtonClicked"));
		_ExitButton.Connect("pressed", new Callable(this, "OnExitButtonClicked"));

		base._Ready();
	}

	protected override void OnShow()
	{
		if (GetNetwork.IsAuthenticated())
			_HordeModeButton.Disabled = false;	
		else
			_HordeModeButton.Disabled = true;	
}

	public void OnEndlessButtonClicked()
	{
		Hide();
		Game.sharedInstance.StartEndlessMode();
	}

	public void OnHordeModeButtonClicked()
	{
		Hide();
		DialogManager.sharedInstance.ShowLevelSelectDialog();
	}

	public void OnBrainCloudButtonClicked()
	{
		DialogManager.sharedInstance.ShowBrainCloudDialog();
	}

	public void OnExitButtonClicked()
	{
		Game.sharedInstance.GetNetwork.EndSession();
		this.GetTree().Quit();
	}
}
