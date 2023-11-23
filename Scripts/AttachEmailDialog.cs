// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class AttachEmailDialog : Dialog
{
	[Export] private TextureButton _AttachButton;
	[Export] private LineEdit _EmailLineEdit;
	[Export] private LineEdit _PasswordLineEdit;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AttachButton.Connect("pressed", new Callable(this, "OnAttachButtonClicked"));

		base._Ready();
	}

	public void OnAttachButtonClicked()
	{
		Hide();
	}
}
