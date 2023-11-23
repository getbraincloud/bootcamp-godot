// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class AchievementControl : Control
{
	[Export] private Label _DescriptionLabel;
	[Export] private Label _StatusLabel;


	public void Reset()
	{
		_DescriptionLabel.Text = "";
		_StatusLabel.Text = "";
	}

	public void Set(Achievement achievement)
	{
		_DescriptionLabel.Text = achievement.Description;
		_StatusLabel.Text = achievement.GetStatusString();

		if (achievement.IsAwarded)
		{
			Color green = new Color(207.0f / 255.0f, 198.0f / 255.0f, 0.0f, 1.0f);
		   	_DescriptionLabel.Modulate = green;
			_StatusLabel.Modulate = green;
		}
		else
		{
			Color white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		   	_DescriptionLabel.Modulate = white;
			_StatusLabel.Modulate = white;
		}
	}
}
