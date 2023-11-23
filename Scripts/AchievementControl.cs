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
}
