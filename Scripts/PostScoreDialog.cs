// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class PostScoreDialog : Dialog
{
	[Export] private TextureButton _SubmitScoreButton;
	[Export] private LineEdit _UsernameLineEdit;

	private Network.PostScoreRequestCompleted m_PostScoreRequestCompleted;
	private Network.PostScoreRequestFailed m_PostScoreRequestFailed;
	private double m_Time;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_SubmitScoreButton.Connect("pressed", new Callable(this, "OnSubmitButtonClicked"));

		base._Ready();
	}

	public void Set(double time, Network.PostScoreRequestCompleted postScoreRequestCompleted, Network.PostScoreRequestFailed postScoreRequestFailed)
	{
		m_Time = time;
		m_PostScoreRequestCompleted = postScoreRequestCompleted;
		m_PostScoreRequestFailed = postScoreRequestFailed;
	}

	public void OnSubmitButtonClicked()
	{
		GetNetwork.PostScoreToLeaderboard(Constants.kBrainCloudMainLeaderboardID, m_Time, _UsernameLineEdit.Text.ToString(), m_PostScoreRequestCompleted, m_PostScoreRequestFailed);
		GetNetwork.UpdateUsername(_UsernameLineEdit.Text.ToString());		
		Hide();
	}
}
