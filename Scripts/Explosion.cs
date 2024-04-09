// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Explosion : BaseNode
{
	private double m_Delay;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D animatedSprite = this.GetChild<AnimatedSprite2D>(0, true);
		animatedSprite.Connect("animation_finished", new Callable(this, "OnExplosionEnd"));

        SetType(Constants.kExplosionType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
 		double timeScale = Game.sharedInstance.TimeScale;

		if (m_Delay > 0.0)
        {
            m_Delay -= delta * timeScale;

            if (m_Delay < 0.0)
            {
                m_Delay = 0.0;
                Explode();
            }
        }
	}

    public void Spawn(Vector2 position, Vector2 scale, double delay)
    {
        m_Delay = delay;
        this.Position = position;
        this.Scale = scale;

        if (m_Delay == 0.0)
            Explode();
    }

	 private void Explode()
    {
		Enable();

		AnimatedSprite2D animatedSprite = this.GetChild<AnimatedSprite2D>(0, true);
		animatedSprite.Play();
    }

	public void OnExplosionEnd()
    {
        Disable();
    }
}
