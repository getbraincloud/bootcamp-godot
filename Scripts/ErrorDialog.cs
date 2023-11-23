// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ErrorDialog : Dialog
{
	[Export] private Label _ErrorLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public void Set(string message)
	{
		_ErrorLabel.Text = message;
	}

	protected override void OnClose()
	{
		// Dialog closed without logging in, show the main menu dialog
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
