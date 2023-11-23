// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ScrollingBackground : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;

		Godot.Sprite2D segment1 = this.GetNode<Godot.Sprite2D>("Segment1");
		MoveSegment(segment1, delta * timeScale);

		Godot.Sprite2D segment2 = this.GetNode<Godot.Sprite2D>("Segment2");
		MoveSegment(segment2, delta * timeScale);
	}

	private void MoveSegment(Godot.Sprite2D segment, double delta)
	{
		segment.Translate(Vector2.Left * Constants.kBackgroundSpeed * (float)delta);

		float width = DisplayServer.WindowGetSize().X;
		if (segment.Position.X < -width)
			segment.Translate(new Vector2(width * 2.0f, 0.0f));
	}
}
