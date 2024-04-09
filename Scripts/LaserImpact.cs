// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class LaserImpact : BaseNode
{
    private double m_Lifetime;
    private Laser.Color m_Color;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetType(Constants.kLaserImpactType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        double timeScale = Game.sharedInstance.TimeScale;

		if (m_Lifetime > 0.0)
        {
            m_Lifetime -= delta * timeScale;

            if (m_Lifetime <= 0.0)
            {
                m_Lifetime = 0.0;
				Disable();
            }
        }
	}

	public void Spawn(Vector2 position, Laser.Color color)
    {
        Enable();

        this.Position = position;
        m_Color = color;
        m_Lifetime = Constants.kLaserImpactLifetime;

        if (m_Color != Laser.Color.Unknown)
        {
            string texture = m_Color == Laser.Color.Blue ? "LaserImpact-Blue" : "LaserImpact-Red";
			string path = "res://Textures/" + texture + ".png";
			Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
			sprite.Texture = ResourceLoader.Load<Texture2D>(path);
        }
        else
            Disable();
    }
}
