// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ShipWing : BaseNode
{
    private Vector2 m_LinearVelocity;
    private float m_AngularVelocity;
    private float m_SpinDirection;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetType(Constants.kShipWingType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        double timeScale = Game.sharedInstance.TimeScale;
		HandleFade(delta * timeScale);

        // Move the asteroid
        Vector2 position = this.Position;
        position += m_LinearVelocity * (float)(delta * timeScale);
        this.Position = position;

        float angle = this.Rotation;
        angle += m_AngularVelocity * m_SpinDirection * (float)(delta * timeScale);
        if (m_SpinDirection > 0.0f && angle > MathF.PI * 2.0f)
        {
            angle -= MathF.PI * 2.0f;
        }
        else if (m_SpinDirection < 0.0f && angle < -(MathF.PI * 2.0f))
        {
            angle += MathF.PI * 2.0f;
        }

        this.Rotation = angle;
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, float radiansZ, string texture)
    {
		Enable();
	
		string path = "res://Textures/" + texture + ".png";
		Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
		sprite.Texture = ResourceLoader.Load<Texture2D>(path);

        this.Rotation = radiansZ;
        this.Position = position;
        m_LinearVelocity = linearVelocity;
        m_AngularVelocity = (float)GD.RandRange(Constants.kShipWingMinAngularVelocity, Constants.kShipWingMaxAngularVelocity);
        m_SpinDirection = GD.RandRange(0, 1) == 1 ? 1.0f : -1.0f;

        SetAlpha(1.0f);
    }
}
