// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class Ship : BaseNode
{
	public delegate void ShipHasExploded();

	private readonly PackedScene m_ShieldPrefab = ResourceLoader.Load("res://Prefabs/Shield.tscn") as PackedScene;

	private Shield m_Shield;

	private Vector2 m_Acceleration;
	private Vector2 m_LinearVelocity;
	private Vector2 m_SpriteSize;
	private double m_InvincibilityTimer;
	private ShipHasExploded m_ShipHasExploded;
	private Slider m_SpawnSlider;
	private bool m_IsSpawning = false;
	private bool m_RightKeyPressed = false;
	private bool m_LeftKeyPressed = false;
	private bool m_UpKeyPressed = false;
	private bool m_DownKeyPressed = false;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m_SpawnSlider = new Slider();

		m_Shield = m_ShieldPrefab.Instantiate<Shield>();
		m_Shield.Name = "Shield";
		this.AddChild(m_Shield);

		Sprite2D sprite = this.GetChild<Sprite2D>(0, true);
		m_SpriteSize = new Vector2(sprite.Texture.GetWidth(), sprite.Texture.GetHeight());

		SetHealth(Constants.kShipInitialHealth);
		SetType(Constants.kShipType);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;
		if(timeScale == 0.0f)
			return;
		
		if (m_IsSpawning)
		{
			if (m_SpawnSlider.IsSliding())
			{
				m_SpawnSlider.Update(delta * timeScale);
				this.Position = m_SpawnSlider.Current;
			}
			return;
		}

		if (m_InvincibilityTimer > 0.0)
		{
			m_InvincibilityTimer -= delta * timeScale;
			if (m_InvincibilityTimer <= 0.0)
			{
				m_InvincibilityTimer = 0.0;
				SetAlpha(1.0f);
			}
			else
				SetAlpha(Constants.kShipInvincibleAlpha);
		}
		
		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
		Vector2 drag = Vector2.Zero;
		drag.X = m_Acceleration.X != 0.0f ? (m_Acceleration.X - (m_Acceleration.X * Constants.kShipDrag)) / m_Acceleration.X : -Constants.kShipDrag;
		drag.Y = m_Acceleration.Y != 0.0f ? (m_Acceleration.Y - (m_Acceleration.Y * Constants.kShipDrag)) / m_Acceleration.Y : -Constants.kShipDrag;
		m_LinearVelocity += m_Acceleration + m_LinearVelocity * drag * (float)Math.Pow(delta * timeScale, 0.5);

		// Cap the speed
		if (Math.Abs(m_LinearVelocity.Length()) > Constants.kShipMaxSpeed)
			m_LinearVelocity = m_LinearVelocity.Normalized() * Constants.kShipMaxSpeed;

		// Move the ship
		Vector2 position = new Vector2(this.Position.X, this.Position.Y);
		position += m_LinearVelocity * (float)(delta * timeScale);

		// Clamp the position on screen
		position.X = Math.Clamp(position.X, Constants.kShipMinX, visibleAreaSize.X - Constants.kShipMaxOffsetX);
		position.Y = Math.Clamp(position.Y, 0.0f, visibleAreaSize.Y);
		this.Position = position;
		
		// Tilt the ship when moving along the y-axis
		this.Rotation = Constants.kShipTurnTilt * (m_LinearVelocity.Y / Constants.kShipMaxSpeed);
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Fire"))
		{
			OnFire();
		}

		if (@event.IsActionPressed("MovementUp"))
		{
			OnMovementUpStart();
		}
		else if (@event.IsActionReleased("MovementUp"))
		{
			OnMovementUpStop();
		}
		
		if (@event.IsActionPressed("MovementDown"))
		{
			OnMovementDownStart();
		}
		else if (@event.IsActionReleased("MovementDown"))
		{
			OnMovementDownStop();
		}
		
		if (@event.IsActionPressed("MovementLeft"))
		{
			OnMovementLeftStart();
		}
		else if (@event.IsActionReleased("MovementLeft"))
		{
			OnMovementLeftStop();
		}
		
		if (@event.IsActionPressed("MovementRight"))
		{
			OnMovementRightStart();
		}
		else if (@event.IsActionReleased("MovementRight"))
		{
			OnMovementRightStop();
		}
	}
	
	public void Spawn()
	{
		m_IsSpawning = true;

		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
		Vector2 start = new Vector2(Constants.kShipOffScreenSpawnX, visibleAreaSize.Y * 0.5f);
		Vector2 target = new Vector2(Constants.kShipSpawnX, visibleAreaSize.Y * 0.5f);
		this.Position = start;
		m_SpawnSlider.StartSlide(start, target, 1.0f, OnSpawnCompleted);
	}
	
	public void HealUp()
	{
		SetHealth(Constants.kShipInitialHealth);

		m_Shield.Enable();
		m_Shield.SetHealth(Constants.kShipInitialShieldHealth);
	}
	
	public void Reset()
	{
		Disable();
		Vector2 visibleAreaSize = Game.sharedInstance.VisibleAreaSize;
		this.Position = new Vector2(Constants.kShipOffScreenSpawnX, visibleAreaSize.Y * 0.5f);
		this.Rotation = 0.0f;

		m_InvincibilityTimer = 0.0;
		m_IsSpawning = false;

		m_LinearVelocity = Vector2.Zero;
		m_Acceleration = Vector2.Zero;

		HealUp();
	}
	
	public void SetDelegate(ShipHasExploded shipHasExploded)
	{
		m_ShipHasExploded = shipHasExploded;
	}
	
	public new bool ApplyDamage(int damage)
	{
		if (IsInvincible())
			return false;

		// Set the player's invincibility timer, since they took damage
		m_InvincibilityTimer = Constants.kShipInvincibilityDuration;

		// If the player still has the shield, then take damage from it first
		if (HasShield())
		{
			if (m_Shield.GetHealth() < damage)
			{
				int diff = damage - m_Shield.GetHealth();
				m_Shield.ApplyDamage(damage);
				m_Shield.Disable();
				return base.ApplyDamage(diff);
			}
			else
			{
				if (m_Shield.ApplyDamage(damage))
					m_Shield.Disable();
				return false;
			}
		}

		return base.ApplyDamage(damage);
	}
	
	public bool HasShield()
	{
		return m_Shield.IsEnabled();
	}

	public Shield GetShield()
	{
		return m_Shield;
	}

	public bool IsInvincible()
	{
		return m_InvincibilityTimer > 0.0;
	}

	public void OnFire()
	{
		if (!m_IsSpawning && GetHealth() > 0)
		{
			FireLaser(true);
			FireLaser(false);
		}
	}
	
	public void OnMovementUpStart()
	{		
		if (!m_IsSpawning)
		{
			m_UpKeyPressed = true;
			m_Acceleration.Y -= Constants.kShipAcceleration;
			m_LinearVelocity.Y = m_Acceleration.Y;
		}
	}

	public void OnMovementDownStart()
	{
		if (!m_IsSpawning)
		{
			m_DownKeyPressed = true;
			m_Acceleration.Y += Constants.kShipAcceleration;
			m_LinearVelocity.Y = m_Acceleration.Y;
		}
	}

	public void OnMovementLeftStart()
	{
		if (!m_IsSpawning)
		{
			m_LeftKeyPressed = true;
			m_Acceleration.X -= Constants.kShipAcceleration;
			m_LinearVelocity.X = m_Acceleration.X;
		}
	}

	public void OnMovementRightStart()
	{
		if (!m_IsSpawning)
		{
			m_RightKeyPressed = true;
			m_Acceleration.X += Constants.kShipAcceleration;
			m_LinearVelocity.X = m_Acceleration.X;
		}
	}

	public void OnMovementUpStop()
	{
		if (!m_IsSpawning && m_UpKeyPressed)
		{
			m_UpKeyPressed = false;
			m_Acceleration.Y += Constants.kShipAcceleration;
		}
	}

	public void OnMovementDownStop()
	{
		if (!m_IsSpawning && m_DownKeyPressed)
		{
			m_DownKeyPressed = false;
			m_Acceleration.Y -= Constants.kShipAcceleration;
		}
	}

	public void OnMovementLeftStop()
	{
		if (!m_IsSpawning && m_LeftKeyPressed)
		{
			m_LeftKeyPressed = false;
			m_Acceleration.X += Constants.kShipAcceleration;
		}
	}

	public void OnMovementRightStop()
	{
		if (!m_IsSpawning && m_RightKeyPressed)
		{
			m_RightKeyPressed = false;
			m_Acceleration.X -= Constants.kShipAcceleration;
		}
	}
	
	private void FireLaser(bool left)
	{
		float radians = this.Rotation;
		float magnitude = (m_SpriteSize.Y * 0.5f) - Constants.kShipGunOffset;
		float radiansTilt = left ? -Constants.kShipGunAngleTilt : Constants.kShipGunAngleTilt;
		float edgeRadians = radians + (left ? ((float)Math.PI * 0.5f) : -((float)Math.PI * 0.5f));
		Vector2 direction = new Vector2((float)Math.Cos(radians + radiansTilt), (float)Math.Sin(radians + radiansTilt));
		Vector2 edge = new Vector2((float)Math.Cos(edgeRadians), (float)Math.Sin(edgeRadians)) * magnitude;
		Vector2 position = this.Position;
		Vector2 laserPosition = position + edge;
		Vector2 linearVelocity = direction * Constants.kLaserSpeed;

		Spawner.sharedInstance.SpawnBlueLaser(laserPosition, linearVelocity);
	}
	
	private void HandlePickup(Pickup pickup)
	{
		if (pickup.GetVariant() == Pickup.Variant.Shield)
		{
			if (!m_Shield.IsEnabled())
				m_Shield.Enable();

			if (m_Shield.GetHealth() < Constants.kShipInitialShieldHealth)
				m_Shield.SetHealth(m_Shield.GetHealth() + 1);
		}
	}	

	private void Explode()
	{
		if (IsEnabled())
		{
			Spawner.sharedInstance.SpawnExplosion(this.Position, Vector2.One);

			ShipWing leftWing = Spawner.sharedInstance.SpawnWing(this.Position, this.Rotation, "Ship-LeftWing", true);
			if (leftWing != null) 
				leftWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);
			
			
			ShipWing rightWing = Spawner.sharedInstance.SpawnWing(this.Position, this.Rotation, "Ship-RightWing", false);
			if (rightWing != null) 
				rightWing.FadeOut(Constants.kShipWingExplosionFadeOutTime);

			Disable();

			if (m_ShipHasExploded != null)
				m_ShipHasExploded();
		}
	}

	private void OnSpawnCompleted(Slider slider)
	{
		m_IsSpawning = false;

		m_LinearVelocity = Vector2.Zero;
		m_Acceleration = Vector2.Zero;
	}
	
	private void _OnAreaEntered(Area2D area)
	{
		if(IsEnabled())
		{
			BaseNode otherNode = area.GetParent<BaseNode>();

			if (otherNode.Type == Constants.kAsteroidType)
			{
				Asteroid asteroid = (Asteroid)otherNode;
				int asteroidAttackDamage = asteroid.GetAttackDamage();
				asteroid.Explode();

				if (ApplyDamage(asteroidAttackDamage))
					Explode();
			}
			else if (otherNode.Type == Constants.kEnemyType)
			{
				Enemy enemy = (Enemy)otherNode;
				int enemyAttackDamage = enemy.GetAttackDamage();
				enemy.Explode();

				if (ApplyDamage(enemyAttackDamage))
					Explode();
			}
			else if (otherNode.Type == Constants.kBossType)
			{
				Boss boss = (Boss)otherNode;
				if (ApplyDamage(boss.GetAttackDamage()))
					Explode();
			}
			else if (otherNode.Type == Constants.kLaserType)
			{
				Laser laser = (Laser)otherNode;

				if (laser.GetLaserColor() == Laser.Color.Red)
				{
					Spawner.sharedInstance.SpawnLaserImpact(laser.GetFront(), laser.GetLaserColor());

					if (ApplyDamage(laser.GetAttackDamage()))
						Explode();

					otherNode.Disable();
				}
			}
			else if (otherNode.Type == Constants.kMissileType)
			{
				Missile missile = (Missile)otherNode;

				if(missile.GetMissleSize() == Missile.Size.Small)
					Spawner.sharedInstance.SpawnExplosion(missile.GetMiddle(), new Vector2(0.2f, 0.2f));
				else
					Spawner.sharedInstance.SpawnExplosion(missile.GetMiddle(), new Vector2(0.45f, 0.45f));

				if (ApplyDamage(missile.GetAttackDamage()))
					Explode();

				missile.Disable();
			}
			else if (otherNode.Type == Constants.kPickupType)
			{
				Pickup pickup = (Pickup)otherNode;

				HandlePickup(pickup);
				pickup.Disable();
			}	
		}
	}
}
