// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Missile : BaseNode
{
    public enum Size
    {
        Unknown = -1,
        Small,
        Big
    };

    private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
    private Size m_Size;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetType(Constants.kMissileType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        double timeScale = Game.sharedInstance.TimeScale;

		Vector2 position = this.Position;
        position += m_LinearVelocity * (float)(delta * timeScale);
        this.Position = position;

        if (position.X < -m_SpriteSize.X)
            Disable();
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, Size size)
    {
        Enable();

        this.Position = position;
        this.Rotation = MathF.Atan2(linearVelocity.Y, linearVelocity.X);

        m_LinearVelocity = linearVelocity;
        m_Size = size;

        if (m_Size != Size.Unknown)
        {
            string texture = m_Size == Size.Small ? "Missile-Small" : "Missile-Big";
			string path = "res://Textures/" + texture + ".png";
			Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
			sprite.Texture = ResourceLoader.Load<Texture2D>(path);

			m_SpriteSize = new Vector2(sprite.Texture.GetWidth(), sprite.Texture.GetHeight());

			sprite.Offset = new Vector2(0.0f, -m_SpriteSize.Y * 0.5f);

            if(m_Size == Size.Small)
                SetAttackDamage(Constants.kMissileSmallAttackDamage);
            else
                SetAttackDamage(Constants.kMissileBigAttackDamage);
        }
        else
            Disable();
    }

    public Vector2 GetFront()
    {
        Vector2 displacement = m_LinearVelocity.Normalized() * m_SpriteSize.X;
        return this.Position + displacement;
    }

    public Vector2 GetMiddle()
    {
        Vector2 displacement = m_LinearVelocity.Normalized() * (m_SpriteSize.X * 0.5f);
        return this.Position + displacement;
    }

    public Size GetMissleSize()
    {
        return m_Size;
    }
}
