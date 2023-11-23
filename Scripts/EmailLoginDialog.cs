// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class EmailLoginDialog : Dialog
{
	[Export] private TextureButton _LoginButton;
	[Export] private TextureButton _AnonymousButton;
	[Export] private TextureButton _TwitchButton;
	[Export] private LineEdit _EmailLineEdit;
	[Export] private LineEdit _PasswordLineEdit;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_LoginButton.Connect("pressed", new Callable(this, "OnLoginButtonClicked"));

		if(_AnonymousButton != null)
			_AnonymousButton.Connect("pressed", new Callable(this, "OnAnonymousButtonClicked"));

		if(_TwitchButton != null)
			_TwitchButton.Connect("pressed", new Callable(this, "OnTwitchButtonClicked"));

		base._Ready();
	}

	public void OnLoginButtonClicked()
	{
		// TODO: Implement email authentication
	}

	public void OnAnonymousButtonClicked()
	{
		// TODO: Implement anonymous external authentication
	}

	public void OnTwitchButtonClicked()
	{
		// TODO: Implement Twitch external authentication
	}

	protected override void OnClose()
	{
		// Dialog closed without logging in, show the main menu dialog
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
