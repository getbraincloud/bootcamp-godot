// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class StatusBar : Node2D
{
	[Export] private Sprite2D _fillBar = null;
	[Export] private Sprite2D _dangerFillBar = null;
    private float m_DangerAlpha;
    private float m_DangerRadians;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected void UpdateBar(double delta, float pct)
    {
        m_DangerRadians += Constants.kHudDangerFlashIncrement * (float)delta;

        if (m_DangerRadians >= MathF.PI * 2.0f)
            m_DangerRadians -= MathF.PI * 2.0f;

        m_DangerAlpha = MathF.Abs(MathF.Sin(m_DangerRadians));

        // If the pct is only a quarter or less, change the bar to red (danger)
        if (pct <= 0.25f)
        {
			_dangerFillBar.Modulate = new Color(1.0f, 1.0f, 1.0f, m_DangerAlpha);
			_fillBar.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        else
        {
			_dangerFillBar.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			_fillBar.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Scale the framesize to match the pct left
		_fillBar.Scale = new Vector2(pct, 1.0f);
		_dangerFillBar.Scale = new Vector2(pct, 1.0f);
    }
}
