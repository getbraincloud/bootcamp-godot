// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class StatisticControl : Control
{
	[Export] private Label _TitleLabel;
	[Export] private Label _ValueLabel;

	public void Reset()
	{
		_TitleLabel.Text = "";
		_ValueLabel.Text = "";
	}

	public void Set(Statistic statistic)
	{
		_TitleLabel.Text = statistic.Description;
		_ValueLabel.Text = statistic.Value.ToString();
	}
}
