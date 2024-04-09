// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Pickup : BaseNode
{
	private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
	private double m_Lifetime;

	public enum Variant //TODO: BF Fix name class with BaseNode
	{
		Unknown = -1,
		Shield
	};

	private Variant m_Variant;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetType(Constants.kPickupType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;
		
		HandleFade(delta * timeScale);

		if (m_Lifetime > 0.0)
		{
			m_Lifetime -= delta * timeScale;
			if (m_Lifetime <= 0.0)
			{
				m_Lifetime = 0.0;
				FadeOut(Constants.kPickupFadeOutTime);
			}
		}

		Vector2 position = this.Position;
		position += m_LinearVelocity * (float)(delta * timeScale);
		this.Position = position;

		if (position.X < -m_SpriteSize.X)
			Disable();
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, Variant variant)
	{
		Enable();

		m_Variant = variant;
		this.Position = position;
		m_LinearVelocity = linearVelocity;
		m_Lifetime = Constants.kPickupLifetime;
	}

	public Variant GetVariant()
	{
		return m_Variant;
	}
}
