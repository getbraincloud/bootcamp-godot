// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Asteroid : BaseNode
{
	private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
	private float m_AngularVelocity;
	private float m_SpinDirection;

	public enum Size
	{
		Unknown = -1,
		Big,
		Medium,
		Small,
		Tiny
	};

	private Size m_Size;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetType(Constants.kAsteroidType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleFade(delta);

		// Move the asteroid
		Vector2 position = new Vector2(this.Position.X, this.Position.Y);
		position += m_LinearVelocity * (float)delta;
		this.Position = position;

		float rotation = this.Rotation;
		rotation += m_AngularVelocity * m_SpinDirection * (float)delta;
		if (m_SpinDirection > 0.0f && rotation > (float)Math.PI * 2.0f)
		{
			rotation -= (float)Math.PI * 2.0f;
		}
		else if (m_SpinDirection < 0.0f && rotation < -((float)Math.PI * 2.0f))
		{
			rotation += (float)Math.PI * 2.0f;
		}

		this.Rotation = rotation;

		// If the asteroid goes off-screen deactivate it
		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
		if (position.X < -(m_SpriteSize.X * 0.5f) || position.Y < -m_SpriteSize.Y * 0.5f || position.Y > visibleAreaSize.Y + (m_SpriteSize.Y * 0.5f))
			Disable();
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, Size size)
	{
		ResetFade();

		Enable();

		this.Position = position;
		m_LinearVelocity = linearVelocity;
		m_AngularVelocity = (float)GD.RandRange((double)Constants.kAsteroidMinAngularVelocity, (double)Constants.kAsteroidMaxAngularVelocity);
		m_SpinDirection = GD.RandRange(0, 1) == 1 ? 1.0f : -1.0f;
		m_Size = size;

		int sizeValue = (int)m_Size;
		SetHealth(Constants.kAsteroidHealth[sizeValue]);
		SetAttackDamage(Constants.kAsteroidAttackDamage[sizeValue]);

		// Determine the atlas frame to use base on the size of the asteroid
		int keyIndex = GD.RandRange(0, Constants.kNumAsteroidVariations[sizeValue] - 1);
		Sprite2D sprite = this.GetChild<Sprite2D>(0, true);

		switch (m_Size)
		{
			case Size.Big:
				{
					var path = "res://Textures/" + Constants.kBigAsteroidAtlasKeys[keyIndex] + ".png";
					sprite.Texture = ResourceLoader.Load<Texture2D>(path);
				}
				break;

			case Size.Medium:
				{
					var path = "res://Textures/" + Constants.kMediumAsteroidAtlasKeys[keyIndex] + ".png";
					sprite.Texture = ResourceLoader.Load<Texture2D>(path);
				}
				break;

			case Size.Small:
				{
					var path = "res://Textures/" + Constants.kSmallAsteroidAtlasKeys[keyIndex] + ".png";
					sprite.Texture = ResourceLoader.Load<Texture2D>(path);
				}
				break;

			case Size.Tiny:
				{
					var path = "res://Textures/" + Constants.kTinyAsteroidAtlasKeys[keyIndex] + ".png";
					sprite.Texture = ResourceLoader.Load<Texture2D>(path);
				}
				break;

			default:
				{
					Disable();
					return;
				}
		}

		// Lastly update the sprite size and collider radius (as it might have changed)
		m_SpriteSize = new Vector2(sprite.Texture.GetWidth(), sprite.Texture.GetHeight());

		Area2D area2D = this.GetChild<Area2D>(1, true);
		CollisionShape2D collisionShape = area2D.GetChild<CollisionShape2D>(0, true);
		CircleShape2D circleShape2D = new CircleShape2D();
		circleShape2D.Radius = m_SpriteSize.X * 0.5f;
		collisionShape.SetDeferred("shape", circleShape2D);
	}

	public void Explode(bool onlySmallDebris = false)
	{
		if (IsEnabled() && (m_Size == Size.Big || m_Size == Size.Medium))
		{
			Vector2 position = this.Position;
			Vector2 linearVelocity;
			Asteroid.Size size = Asteroid.Size.Unknown;

			int numDirections = m_Size == Asteroid.Size.Big ? 6 : 3;
			float directionsIncrement = (MathF.PI * 2.0f) / numDirections;
			float offset = (float)GD.RandRange(0.0f, MathF.PI * 0.5f);
			float radians = 0.0f;
			float speed = 0.0f;

			// Spawn little asteroids as a result of the explosion
			for (int i = 0; i < numDirections; ++i)
			{
				if (onlySmallDebris)
					size = GD.RandRange(0, 1) == 1 ? Asteroid.Size.Small : Asteroid.Size.Tiny;
				else
				{
					if (m_Size == Asteroid.Size.Big)
					   size = GD.RandRange(0, 4) > 2 ? Asteroid.Size.Medium : Asteroid.Size.Small;
					else
					   size = GD.RandRange(0, 1) == 1 ? Asteroid.Size.Small : Asteroid.Size.Tiny;
				}

				radians = (directionsIncrement * i) + offset;
				speed = (float)GD.RandRange(Constants.kAsteroidExplosionMinSpeed, Constants.kAsteroidExplosionMaxSpeed);
				linearVelocity.X = MathF.Cos(radians) * speed;
				linearVelocity.Y = MathF.Sin(radians) * speed;

				Asteroid asteroid = Spawner.sharedInstance.SpawnAsteroid(position, linearVelocity, size);
				if (asteroid != null && size != Asteroid.Size.Medium)
					asteroid.FadeOut(Constants.kAsteroidExplosionFadeOutTime, Constants.kAsteroidExplosionFadeDelay);
			}

			// Spawn an explosion
			float scaleF = m_Size == Asteroid.Size.Big ? 1.0f : 0.6f;
			Vector2 scale = new Vector2(scaleF, scaleF);
			Spawner.sharedInstance.SpawnExplosion(position, scale);

			// Lastely deactivate the asteroid
			Disable();	
		}
	}
		
	private void _OnAreaEntered(Area2D area)
	{
		if (IsEnabled() && !IsFading())
		{
			BaseNode otherNode = area.GetParent<BaseNode>();
			if (otherNode.Type == Constants.kLaserType)
			{
				Laser laser = (Laser)otherNode;
				Spawner.sharedInstance.SpawnLaserImpact(laser.GetFront(), laser.GetLaserColor());

				if (ApplyDamage(laser.GetAttackDamage()))
				{
					Explode();

					if (laser.GetLaserColor() == Laser.Color.Blue)
					{
						Statistic asteroidDestroyedStat = GetNetwork.GetStatisticsManager.GetStatisticByName(Constants.kBrainCloudStatAsteroidDestroyed);
						if (asteroidDestroyedStat != null)
							asteroidDestroyedStat.ApplyIncrement();
					}
				}
				otherNode.Disable();
			}
		}
	}
}



