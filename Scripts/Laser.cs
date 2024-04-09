// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Reflection.Metadata;

public partial class Laser : BaseNode
{
	public enum Color
	{
		Unknown = -1,
		Blue,
		Red
	};

	private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
	private Color m_Color;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetType(Constants.kLaserType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;

		Vector2 position = this.Position;
		position += m_LinearVelocity * (float)(delta * timeScale);
		this.Position = position;

		float windowWidth = DisplayServer.WindowGetSize().X;
		if (position.X > windowWidth || position.X < -m_SpriteSize.X)
			Disable();
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, Color color)
	{
		Enable();
 
		this.Position = position;
	   	this.Rotation = (float)Math.Atan2(linearVelocity.Y, linearVelocity.X);
		m_LinearVelocity = linearVelocity;
		m_Color = color;

		if (m_Color != Color.Unknown)
		{
			string texture = m_Color == Color.Blue ? "Laser-Blue" : "Laser-Red";
			string path = "res://Textures/" + texture + ".png";
			Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
			sprite.Texture = ResourceLoader.Load<Texture2D>(path);

			m_SpriteSize = new Vector2(sprite.Texture.GetWidth(), sprite.Texture.GetHeight());

			sprite.Offset = new Vector2(0.0f, -m_SpriteSize.Y * 0.5f);
		}
		else
			Disable();
	}

	public Vector2 GetFront()
	{
		Vector2 position = this.Position;
		Vector2 displacement = m_LinearVelocity.Normalized() * m_SpriteSize.X;
		return position + displacement;
	}

	public Color GetLaserColor()
	{
		return m_Color;
	}
}
