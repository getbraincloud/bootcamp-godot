// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class AchievementsDialog : Dialog
{
	[Export] private Godot.Collections.Array<Container> _Containers = new Godot.Collections.Array<Container>();

	private readonly PackedScene m_AchievementControlPrefab = ResourceLoader.Load("res://Prefabs/AchievementControl.tscn") as PackedScene;

	private Godot.Collections.Array<AchievementControl> _AchievementControls = new Godot.Collections.Array<AchievementControl>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < _Containers.Count; i++)
		{
			AchievementControl achievementControl = m_AchievementControlPrefab.Instantiate<AchievementControl>();
			achievementControl.Position = Vector2.Zero;
			_Containers[i].AddChild(achievementControl);
			_AchievementControls.Add(achievementControl);
		}

		base._Ready();
	}

	protected override void OnShow()
	{
		foreach (AchievementControl achievementControl in _AchievementControls)
		{
			if(achievementControl != null)
				achievementControl.Reset();
		}

		// TODO: Implement Achievements
	}
}
