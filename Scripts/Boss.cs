// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Boss : BaseNode
{
	public delegate void BossHasExploded();

	private Vector2 m_SpriteSize;
	private double m_SmallMissileDelay;
	private double m_BigMissileDelay;
	private double m_MovementDelay;
	private Slider m_Slider = new Slider();
	private BossHasExploded m_BossHasExploded;


	private enum State
	{
		Unknown = -1,
		Spawning,
		Attacking,
		Dead
	}

	private enum MovementDirection
	{
		Unknown = -1,
		Up,
		Down
	}

	private State m_State;
	private MovementDirection m_LastMovementDirection;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetType(Constants.kBossType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;

		if (m_Slider.IsSliding())
		{
			m_Slider.Update(delta * timeScale);
			this.Position = m_Slider.Current;
		}

		if (m_State == State.Attacking)
		{
			if (m_MovementDelay > 0.0)
			{
				m_MovementDelay -= delta * timeScale;

				if (m_MovementDelay < 0.0)
				{
					SetupNextMovement();
				}
			}

			// Clamp the position on screen
			Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
			Vector2 position = this.Position;
			position.X = Mathf.Clamp(position.X, Constants.kBossMinX, visibleAreaSize.X - Constants.kHudHeight - m_SpriteSize.X * 0.5f);
			position.Y = Mathf.Clamp(position.Y, m_SpriteSize.Y * 0.5f, visibleAreaSize.Y - Constants.kHudHeight - m_SpriteSize.Y * 0.5f);
			this.Position = position;


			if (m_SmallMissileDelay > 0.0)
			{
				m_SmallMissileDelay -= delta * timeScale;

				if (m_SmallMissileDelay <= 0.0)
				{
					m_SmallMissileDelay = 0.0;
					FireMissile(Missile.Size.Small);
				}
			}

			if (m_BigMissileDelay > 0.0)
			{
				m_BigMissileDelay -= delta * timeScale;

				if (m_BigMissileDelay <= 0.0)
				{
					m_BigMissileDelay = 0.0;
					FireMissile(Missile.Size.Big);
				}
			}

		}
	}

	public void SetDelegate(BossHasExploded bossHasExploded)
	{
		m_BossHasExploded = bossHasExploded;
	}

	public void Spawn()
	{
		Enable();

		m_State = State.Spawning;
		m_LastMovementDirection = MovementDirection.Unknown;

		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
		Vector2 start = new Vector2(Constants.kBossOffScreenSpawnX, visibleAreaSize.Y * 0.5f);
		Vector2 target = new Vector2(Constants.kBossSpawnX, visibleAreaSize.Y * 0.5f);
		this.Position = start;
		m_Slider.StartSlide(start, target, 1.0f, OnSlideCompleted);

		ResetMissileDelay(Missile.Size.Small);
		ResetMissileDelay(Missile.Size.Big);

		SetHealth(Constants.kBossHealth);
	}

	private void FireMissile(Missile.Size size)
	{
		float radians = this.Rotation;
		float halfPI = MathF.PI * 0.5f;
		float sixRadians = 0.1047198f; //6 degrees
		Vector2 position = this.Position;
		Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));

		if (size == Missile.Size.Small)
		{
			Vector2 linearVelocity = direction * Constants.kMissileSmallSpeed;
			Vector2 direction2 = new Vector2(MathF.Cos(radians + sixRadians), MathF.Sin(radians + sixRadians));
			Vector2 linearVelocity2 = direction2 * Constants.kMissileSmallSpeed;
			Vector2 direction3 = new Vector2(MathF.Cos(radians - sixRadians), MathF.Sin(radians - sixRadians));
			Vector2 linearVelocity3 = direction3 * Constants.kMissileSmallSpeed;

			Vector2 edge1 = new Vector2(MathF.Cos(radians + halfPI), MathF.Sin(radians + halfPI)) * Constants.kBossGunOffset2;
			Vector2 position1 = position + direction * Constants.kBossGunOffset1 + edge1;
			Spawner.sharedInstance.SpawnMissile(position1, linearVelocity, Missile.Size.Small);
			//Spawner.sharedInstance.SpawnMissile(position1, linearVelocity2, Missile.Size.Small);
			//Spawner.sharedInstance.SpawnMissile(position1, linearVelocity3, Missile.Size.Small);

			Vector2 edge2 = new Vector2(MathF.Cos(radians - halfPI), MathF.Sin(radians - halfPI)) * Constants.kBossGunOffset2;
			Vector2 position2 = position + direction * Constants.kBossGunOffset1 + edge2;
			Spawner.sharedInstance.SpawnMissile(position2, linearVelocity, Missile.Size.Small);
			//Spawner.sharedInstance.SpawnMissile(position2, linearVelocity2, Missile.Size.Small);
			//Spawner.sharedInstance.SpawnMissile(position2, linearVelocity3, Missile.Size.Small);
		}
		else
		{
			Vector2 linearVelocity = direction * Constants.kMissileBigSpeed;
			Vector2 missilePosition = position + direction * Constants.kBossGunOffset1;
			Spawner.sharedInstance.SpawnMissile(missilePosition, linearVelocity, Missile.Size.Big);
		}

		ResetMissileDelay(size);
	}

	private void ResetMissileDelay(Missile.Size size)
	{
		if (size == Missile.Size.Small)
			m_SmallMissileDelay = GD.RandRange(Constants.kBossSmallMissileMinDelay, Constants.kBossSmallMissileMaxDelay);
		else
			m_BigMissileDelay = GD.RandRange(Constants.kEnemyLaserMinDelay, Constants.kEnemyLaserMaxDelay);
	}

	private void SetupNextMovement()
	{
		if (m_LastMovementDirection == MovementDirection.Down)
			SetupUpMovement();
		else if (m_LastMovementDirection == MovementDirection.Up)
			SetupDownMovement();
		else
		{
			if (GD.RandRange(0, 3) <= 1)
				SetupUpMovement();
			else
				SetupDownMovement();
		}
	}

	private void SetupUpMovement()
	{
		m_LastMovementDirection = MovementDirection.Up;

		float radians = (float)GD.RandRange(0.0, MathF.PI);
		float distance = (float)GD.RandRange(Constants.kBossMovementMinRange, Constants.kBossMovementMaxRange);
		float time = distance / Constants.kBossSpeed;
		Vector2 start = this.Position;
		Vector2 target = start + new Vector2(Mathf.Cos(radians) * distance, Mathf.Sin(radians) * distance);

		m_Slider.StartSlide(start, target, time, OnSlideCompleted);
	}

	private void SetupDownMovement()
	{
		m_LastMovementDirection = MovementDirection.Down;

		float radians = -(float)GD.RandRange(0.0, MathF.PI);
		float distance = (float)GD.RandRange(Constants.kBossMovementMinRange, Constants.kBossMovementMaxRange);
		float time = distance / Constants.kBossSpeed;
		Vector2 start = this.Position;
		Vector2 target = start + new Vector2(MathF.Cos(radians) * distance, MathF.Sin(radians) * distance);

		m_Slider.StartSlide(start, target, time, OnSlideCompleted);
	}

	private void _OnAreaEntered(Area2D area)
	{
		if (IsEnabled())
		{
			BaseNode otherNode = area.GetParent<BaseNode>();
			if (otherNode.Type == Constants.kAsteroidType)
			{
				Asteroid asteroid = (Asteroid)otherNode;
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

	public void Explode()
	{
		if (IsEnabled())
		{
			m_State = State.Dead;

			Spawner.sharedInstance.SpawnExplosion(this.Position, new Vector2(4.0f, 4.0f));

			// Front wings
			float leftFrontOffset = MathF.PI * 0.75f;
			ShipWing leftFrontWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, leftFrontOffset, Constants.kBossFrontLeftWingAtlasKey);
			if (leftFrontWing != null)
				leftFrontWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			float rightFrontOffset = -MathF.PI * 0.75f;
			ShipWing rightFrontWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, rightFrontOffset, Constants.kBossFrontRightWingAtlasKey);
			if (rightFrontWing != null)
				rightFrontWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			// Middle wings
			float leftMiddleOffset = MathF.PI * 0.5f;
			ShipWing leftMiddleWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, leftMiddleOffset, Constants.kBossMiddleLeftWingAtlasKey);
			if (leftMiddleWing != null)
				leftMiddleWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			float rightMiddleOffset = -MathF.PI * 0.5f;
			ShipWing rightMiddleWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, rightMiddleOffset, Constants.kBossMiddleRightWingAtlasKey);
			if (rightMiddleWing != null)
				rightMiddleWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			// Back wings
			float leftBackOffset = MathF.PI * 0.25f;
			ShipWing leftBackWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, leftBackOffset, Constants.kBossBackLeftWingAtlasKey);
			if (leftBackWing != null)
				leftBackWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			float rightBackOffset = -MathF.PI * 0.25f;
			ShipWing rightBackWing = Spawner.sharedInstance.SpawnBossWing(this.Position, this.Rotation, rightBackOffset, Constants.kBossBackRightWingAtlasKey);
			if (rightBackWing != null)
				rightBackWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			Disable();

			if (m_BossHasExploded != null)
				m_BossHasExploded();
		}
	}

	private void OnSlideCompleted(Slider slider)
	{
		if (m_State == State.Spawning)
			m_State = State.Attacking;
		
		m_MovementDelay = GD.RandRange(Constants.kBossMovementMinDelay, Constants.kBossMovementMaxDelay);
	}
}
