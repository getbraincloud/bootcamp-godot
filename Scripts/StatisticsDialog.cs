// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using Microsoft.VisualBasic;
using System;

public partial class StatisticsDialog : Dialog
{
	[Export] private Godot.Collections.Array<Container> _Containers = new Godot.Collections.Array<Container>();

	private readonly PackedScene m_StatisticControlPrefab = ResourceLoader.Load("res://Prefabs/StatisticControl.tscn") as PackedScene;

	private Godot.Collections.Array<StatisticControl> _StatisticControls = new Godot.Collections.Array<StatisticControl>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < _Containers.Count; i++)
		{
			StatisticControl statisticControl = m_StatisticControlPrefab.Instantiate<StatisticControl>();
			statisticControl.Position = Vector2.Zero;
			_Containers[i].AddChild(statisticControl);
			_StatisticControls.Add(statisticControl);
		}

		base._Ready();
	}

	protected override void OnShow()
	{
		foreach (StatisticControl statisticControl in _StatisticControls)
		{
			if(statisticControl != null)
				statisticControl.Reset();
		}

		// TODO: Implement statistics
	}
}
