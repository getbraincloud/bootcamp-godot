// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Shield : BaseNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetHealth(Constants.kShipInitialShieldHealth);
		SetType(Constants.kShieldType);
	}
}
