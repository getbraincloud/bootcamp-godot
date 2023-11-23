// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class PostScoreDialog : Dialog
{
	[Export] private TextureButton _SubmitScoreButton;
	[Export] private LineEdit _UsernameLineEdit;

	private double m_Time;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_SubmitScoreButton.Connect("pressed", new Callable(this, "OnSubmitButtonClicked"));

		base._Ready();
	}

	public void Set(double time)
	{
		m_Time = time;
		
		// TODO: Implement Leaderboards
	}

	public void OnSubmitButtonClicked()
	{
		// TODO: Implement Leaderboards

		Hide();
	}
}
