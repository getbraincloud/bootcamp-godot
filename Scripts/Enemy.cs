// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : BaseNode
{
	private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
	private double m_LaserDelay;
	private double m_EnemyFourFiringDuration;   //Enemy.Type.Four only
	private double m_EnemyFourFiringCooldown;   //Enemy.Type.Four only
	private double m_EnemyFiveMissileDelay;     //Enemy.Type.Five only
	private double m_EnemyFiveMissileCooldown;  //Enemy.Type.Five only

	private bool m_LaserAlternate = true;

	public enum Variant
	{
		Unknown = -1,
		One,
		Two,
		Three,
		Four,
		Five
	};

	private Variant m_Variant;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetType(Constants.kEnemyType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;

		// Move the enemy
		Vector2 position = this.Position;
		position += m_LinearVelocity * (float)delta;
		this.Position = position;

		// Only fire a laser if the front of the enemy is on-screen
		if (position.X < visibleAreaSize.X)
		{
			bool canFire = true;

			// Logic below is used for enemy four and five's firing behaviours

			if (m_Variant == Variant.Four)
			{
				if (m_EnemyFourFiringCooldown > 0.0)
				{
					m_EnemyFourFiringCooldown -= delta;

					if (m_EnemyFourFiringCooldown < 0.0)
					{
						m_EnemyFourFiringCooldown = 0.0;
						m_EnemyFourFiringDuration = Constants.kEnemyFourFiringDuration;
					}
					else
						canFire = false;
				}
				else
				{

					if (m_EnemyFourFiringDuration > 0.0)
					{
						m_EnemyFourFiringDuration -= delta;

						if (m_EnemyFourFiringDuration < 0.0)
						{
							m_EnemyFourFiringDuration = 0.0;
							m_EnemyFourFiringCooldown = Constants.kEnemyFourFiringCooldown;
							canFire = false;
						}
					}
				}
			}
			else if (m_Variant == Variant.Five)
			{
				if (m_EnemyFiveMissileCooldown > 0.0)
				{
					m_EnemyFiveMissileCooldown -= delta;

					if (m_EnemyFiveMissileCooldown < 0.0)
					{
						m_EnemyFiveMissileDelay = GD.RandRange(Constants.kEnemyFiveMissileMinDelay, Constants.kEnemyFiveMissileMaxDelay);
						canFire = false;
					}
				}
				else
				{

					if (m_EnemyFiveMissileDelay > 0.0)
					{
						m_EnemyFiveMissileDelay -= delta;

						if (m_EnemyFiveMissileDelay < 0.0)
						{
							m_EnemyFiveMissileDelay = 0.0;
							m_EnemyFiveMissileCooldown = GD.RandRange(Constants.kEnemyFiveFiringMinCooldown, Constants.kEnemyFiveFiringMaxCooldown);
							FireMissle();
							ResetLaserDelay();
						}
						else
							canFire = false;
					}
				}
			}

			if (m_LaserDelay > 0.0 && canFire)
			{
				m_LaserDelay -= delta;

				if (m_LaserDelay <= 0.0)
				{
					m_LaserDelay = 0.0;
					FireLaser();
					ResetLaserDelay();
				}
			}
		}

		// If the enemy goes off-screen deactivate it
		if (position.X < -(m_SpriteSize.X * 0.5f) || position.Y < -m_SpriteSize.Y * 0.5f || position.Y > visibleAreaSize.Y + (m_SpriteSize.Y * 0.5f))
			Disable();
	}

	public void Spawn(Vector2 position, Vector2 linearVelocity, Variant variant, int health)
	{
		if (variant != Variant.Unknown)
		{
			Enable();

			m_Variant = variant;
			m_LinearVelocity = linearVelocity;

			this.Position = position;
			this.Rotation = MathF.Atan2(linearVelocity.Y, linearVelocity.X);

			SetHealth(health);
			ResetLaserDelay();

			int keyIndex = (int)m_Variant;
			Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
			var path = "res://Textures/" + Constants.kEnemyAtlasKeys[keyIndex] + ".png";
			sprite.Texture = ResourceLoader.Load<Texture2D>(path);

			if (m_Variant == Variant.Four)
			{
				m_EnemyFourFiringCooldown = Constants.kEnemyFourFiringCooldown * 2.0;
				m_EnemyFourFiringDuration = 0.0;
			}
			else if (m_Variant == Variant.Five)
			{
				m_EnemyFiveMissileCooldown = GD.RandRange(Constants.kEnemyFiveFiringMinCooldown, Constants.kEnemyFiveFiringMaxCooldown);
			}

			m_SpriteSize = new Vector2(sprite.Texture.GetWidth(), sprite.Texture.GetHeight());

			Area2D area2D = this.GetChild<Area2D>(1, true);
			CollisionPolygon2D collisionPolygon = area2D.GetChild<CollisionPolygon2D>(0, true);
			Vector2[] polygon = GetCollisionPolygon();
			collisionPolygon.SetDeferred("polygon", polygon);
		}
	}

	private void FireMissle()
	{
		if (m_Variant == Variant.Five)
		{
			float radians = this.Rotation;
			Vector2 position = this.Position;
			Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));
			Vector2 linearVelocity = direction * Constants.kMissileSmallSpeed;
			Vector2 missilePosition = position + direction * Constants.kEnemyGunOffset1;
			Spawner.sharedInstance.SpawnMissile(missilePosition, linearVelocity, Missile.Size.Small);
		}
	}

	private void FireLaser()
	{
		float radians = this.Rotation;
		float halfPI = MathF.PI * 0.5f;
		Vector2 position = this.Position;
		Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));
		Vector2 linearVelocity = direction * Constants.kLaserSpeed;

		if (m_Variant == Variant.One)
		{
			Vector2 laserPosition = position + direction * Constants.kEnemyGunOffset1;
			Spawner.sharedInstance.SpawnRedLaser(laserPosition, linearVelocity);
		}
		else if (m_Variant == Variant.Two)
		{
			Vector2 edge1 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kEnemyGunOffset2;
			Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
			Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);

			Vector2 edge2 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kEnemyGunOffset2;
			Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
			Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
		}
		else if (m_Variant == Variant.Three)
		{
			m_LaserAlternate = !m_LaserAlternate;
			if (m_LaserAlternate)
			{
				Vector2 edge1 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kEnemyGunOffset3;
				Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
				Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);
			}
			else
			{
				Vector2 edge2 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kEnemyGunOffset3;
				Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
				Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
			}
		}
		else if (m_Variant == Variant.Four)
		{
			m_LaserAlternate = !m_LaserAlternate;
			if (m_LaserAlternate)
			{
				Vector2 edge1 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kEnemyGunOffset4_1;
				Vector2 edge4 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kEnemyGunOffset4_2;
				Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
				Vector2 position4 = position + direction * Constants.kEnemyGunOffset1 + edge4;

				Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);
				Spawner.sharedInstance.SpawnRedLaser(position4, linearVelocity);
			}
			else
			{
				Vector2 edge2 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kEnemyGunOffset4_2;
				Vector2 edge3 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kEnemyGunOffset4_1;
				Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
				Vector2 position3 = position + direction * Constants.kEnemyGunOffset1 + edge3;

				Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
				Spawner.sharedInstance.SpawnRedLaser(position3, linearVelocity);

			}
		}
		else if (m_Variant == Variant.Five)
		{
			Vector2 edge1 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kEnemyGunOffset5;
			Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
			Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);

			Vector2 edge2 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kEnemyGunOffset5;
			Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
			Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
		}
	}

	private Vector2[] GetCollisionPolygon()
	{
		List<Vector2> polygon = new List<Vector2>();

		if (m_Variant == Variant.One) 
		{
			polygon.Add(new Vector2(32, 15));
			polygon.Add(new Vector2(-9, 38));
			polygon.Add(new Vector2(-32, 0));
			polygon.Add(new Vector2(-9, -38));
			polygon.Add(new Vector2(32, -15));
		}
		else if (m_Variant == Variant.Two) 
		{
			polygon.Add(new Vector2(30, -18));
			polygon.Add(new Vector2(30, 18));
			polygon.Add(new Vector2(-12, 35));
			polygon.Add(new Vector2(-30, 22));
			polygon.Add(new Vector2(-30, -22));
			polygon.Add(new Vector2(-12, -35));
		}
		else if (m_Variant == Variant.Three) 
		{
			polygon.Add(new Vector2(36, -32));
			polygon.Add(new Vector2(36, 32));
			polygon.Add(new Vector2(-2, 44));
			polygon.Add(new Vector2(-30, 22));
			polygon.Add(new Vector2(-30, -22));
			polygon.Add(new Vector2(-2, -44));
		}
		else if (m_Variant == Variant.Four) 
		{
			polygon.Add(new Vector2(34, -10));
			polygon.Add(new Vector2(34, 10));
			polygon.Add(new Vector2(12, 40));
			polygon.Add(new Vector2(-35, 40));
			polygon.Add(new Vector2(-35, -40));
			polygon.Add(new Vector2(12, -40));
		}
		else if (m_Variant == Variant.Five) 
		{
			polygon.Add(new Vector2(38, -11));
			polygon.Add(new Vector2(38, 11));
			polygon.Add(new Vector2(22, 30));
			polygon.Add(new Vector2(-45, 50));
			polygon.Add(new Vector2(-45, -50));
			polygon.Add(new Vector2(22, -30));
		}

		return polygon.ToArray();
	}

	private void ResetLaserDelay()
	{
		if (m_Variant == Variant.Three)
			m_LaserDelay = Constants.kEnemyThreeFireDelay;
		else if (m_Variant == Variant.Four)
			m_LaserDelay = Constants.kEnemyFourFireDelay;
		else if(m_Variant == Variant.Five)
			m_LaserDelay = GD.RandRange(Constants.kEnemyFiveLaserMinDelay, Constants.kEnemyFiveLaserMaxDelay);
		else
			m_LaserDelay = GD.RandRange(Constants.kEnemyLaserMinDelay, Constants.kEnemyLaserMaxDelay);
	}

	public void Explode()
	{
		if (IsEnabled())
		{
			Spawner.sharedInstance.SpawnExplosion(this.Position, Vector2.One);

			int keyIndex = (int)m_Variant;

			string leftTexture = Constants.kEnemyWingLeftAtlasKeys[keyIndex];
			ShipWing leftWing = Spawner.sharedInstance.SpawnWing(this.Position, this.Rotation, leftTexture, true);
			if (leftWing != null)
				leftWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			string rightTexture = Constants.kEnemyWingRightAtlasKeys[keyIndex];
			ShipWing rightWing = Spawner.sharedInstance.SpawnWing(this.Position, this.Rotation, rightTexture, false);
			if (rightWing != null)
				rightWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			if (Game.sharedInstance.GetShip().GetHealth() < Constants.kShipInitialShieldHealth && GD.RandRange(1, 5) == 1)
				Spawner.sharedInstance.SpawnPickup(this.Position, m_LinearVelocity, Pickup.Variant.Shield);

			Disable();
		}
	}
	
	private void _OnAreaEntered(Area2D area)
	{
		if (IsEnabled())
		{
			BaseNode otherNode = area.GetParent<BaseNode>();
			if (otherNode.Type == Constants.kAsteroidType)
			{
				Asteroid asteroid = (Asteroid)otherNode;
				if (ApplyDamage(asteroid.GetAttackDamage()))
					Explode();
				asteroid.Explode();
			}
			else if (otherNode.Type == Constants.kLaserType)
			{
				Laser laser = (Laser)otherNode;

				if (laser.GetLaserColor() == Laser.Color.Blue)
				{
					Spawner.sharedInstance.SpawnLaserImpact(laser.GetFront(), laser.GetLaserColor());

					if (ApplyDamage(laser.GetAttackDamage()))
					{
						Explode();

						Statistic enemiesKilledStat = GetNetwork.GetStatisticsManager.GetStatisticByName(Constants.kBrainCloudStatEnemiesKilled);
						if (enemiesKilledStat != null)
							enemiesKilledStat.ApplyIncrement();
					}

					laser.Disable();
				}
			}
		}
	}
}
