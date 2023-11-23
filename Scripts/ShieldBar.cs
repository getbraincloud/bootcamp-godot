// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ShieldBar : StatusBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float pct = (float)Game.sharedInstance.GetShip().GetShield().GetHealth() / (float)Constants.kShipInitialShieldHealth;
		UpdateBar(delta, pct);
	}
}
