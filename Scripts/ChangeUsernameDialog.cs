// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ChangeUsernameDialog : Dialog
{
	[Export] private TextureButton _ChangeUsernameButton;
	[Export] private LineEdit _UsernameLineEdit;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ChangeUsernameButton.Connect("pressed", new Callable(this, "OnChangeButtonClicked"));
		base._Ready();
	}

	public void OnChangeButtonClicked()
	{
		// TODO: Implement change username request
		Hide();
	}
}
