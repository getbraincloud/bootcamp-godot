// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class UniversalLoginDialog : Dialog
{
	[Export] private TextureButton _LoginButton;
	[Export] private LineEdit _UsernameLineEdit;
	[Export] private LineEdit _PasswordLineEdit;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_LoginButton.Connect("pressed", new Callable(this, "OnLoginButtonClicked"));

		base._Ready();
	}

	public void OnLoginButtonClicked()
	{
		// TODO: Implement Universal authentication
	}

	protected override void OnClose()
	{
		// Dialog closed without logging in, show the main menu dialog
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
