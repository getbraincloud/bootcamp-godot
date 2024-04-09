// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	public static Spawner sharedInstance;

	private readonly PackedScene m_AsteroidPrefab = ResourceLoader.Load("res://Prefabs/Asteroid.tscn") as PackedScene;
	private readonly PackedScene m_MissilePrefab = ResourceLoader.Load("res://Prefabs/Missile.tscn") as PackedScene;
	private readonly PackedScene m_LaserPrefab = ResourceLoader.Load("res://Prefabs/Laser.tscn") as PackedScene;
	private readonly PackedScene m_LaserImpactPrefab = ResourceLoader.Load("res://Prefabs/LaserImpact.tscn") as PackedScene;
	private readonly PackedScene m_ExplosionPrefab = ResourceLoader.Load("res://Prefabs/Explosion.tscn") as PackedScene;
	private readonly PackedScene m_EnemyPrefab = ResourceLoader.Load("res://Prefabs/Enemy.tscn") as PackedScene;
	private readonly PackedScene m_PickupPrefab = ResourceLoader.Load("res://Prefabs/Pickup.tscn") as PackedScene;
	private readonly PackedScene m_ShipWingPrefab = ResourceLoader.Load("res://Prefabs/ShipWing.tscn") as PackedScene;
	private readonly PackedScene m_BossPrefab = ResourceLoader.Load("res://Prefabs/Boss.tscn") as PackedScene;

	private ObjectPool<Asteroid> m_AsteroidPool;
	private ObjectPool<Missile> m_MissilePool;
	private ObjectPool<Laser> m_LaserPool;
	private ObjectPool<LaserImpact> m_LaserImpactPool;
	private ObjectPool<Explosion> m_ExplosionPool;
	private ObjectPool<Enemy> m_EnemyPool;
	private ObjectPool<Pickup> m_PickupPool;
	private ObjectPool<ShipWing> m_ShipWingPool;
	private Boss m_Boss;

	private List<LevelData> m_LevelData = new List<LevelData>();
	private LevelData m_DefaultLevel = new LevelData();
	private double m_AsteroidSpawnTimer = 0.0;
	private double m_EnemySpawnTimer = 0.0;
	private int m_LevelIndex = -1;
	private bool m_CanSpawn = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sharedInstance = this;

		m_AsteroidPool = new ObjectPool<Asteroid>(m_AsteroidPrefab, this, Constants.kAsteroidPoolSize);
		m_MissilePool = new ObjectPool<Missile>(m_MissilePrefab, this, Constants.kMissilePoolSize);
		m_LaserPool = new ObjectPool<Laser>(m_LaserPrefab, this, Constants.kLaserPoolSize);
		m_LaserImpactPool = new ObjectPool<LaserImpact>(m_LaserImpactPrefab, this, Constants.kLaserImpactPoolSize);
		m_ExplosionPool = new ObjectPool<Explosion>(m_ExplosionPrefab, this, Constants.kExplosionPoolSize);
		m_EnemyPool = new ObjectPool<Enemy>(m_EnemyPrefab, this, Constants.kEnemyPoolSize);
		m_PickupPool = new ObjectPool<Pickup>(m_PickupPrefab, this, Constants.kPickupPoolSize);
		m_ShipWingPool = new ObjectPool<ShipWing>(m_ShipWingPrefab, this, Constants.kShipWingPoolSize);

		m_Boss = m_BossPrefab.Instantiate<Boss>();
		m_Boss.Disable();
		this.AddChild(m_Boss);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeScale = Game.sharedInstance.TimeScale;
		if (!m_CanSpawn || timeScale == 0.0f)
			return;

		if (HandleSpawnTimer(delta, ref m_AsteroidSpawnTimer))
		{
			// Reset the asteroid spawn timer
			double minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
			double maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
			m_AsteroidSpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);

			Asteroid.Size size = Asteroid.Size.Unknown;
			int minSpawnCount = CurrentLevelData.AsteroidMinSpawnCount;
			int maxSpawnCount = CurrentLevelData.AsteroidMaxSpawnCount;
			int spawnCount = GD.RandRange(minSpawnCount, maxSpawnCount);

			for (int i = 0; i < spawnCount; i++)
			{
				size = GD.RandRange(0, 4) >= 2 ? Asteroid.Size.Big : Asteroid.Size.Medium; //More likely to spawn a big asteroid
				SpawnAsteroid(size);
			}
		}

		if (HandleSpawnTimer(delta, ref m_EnemySpawnTimer))
		{
			if (CurrentLevelData.EnemyVariants.Count > 0)
			{
				// Reset the enemy spawn timer
				double minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
				double maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
				m_EnemySpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);

				int minSpawnCount = CurrentLevelData.EnemyMinSpawnCount;
				int maxSpawnCount = CurrentLevelData.EnemyMaxSpawnCount;
				int spawnCount = maxSpawnCount > 1 ? GD.RandRange(minSpawnCount, maxSpawnCount) : 1;

				int index = 0;
				int health = 0;
				string variant;

				for (int i = 0; i < spawnCount; i++)
				{
					index = GD.RandRange(0, CurrentLevelData.EnemyVariants.Count -1);
					health = CurrentLevelData.EnemyVariants[index].Health;
					variant = CurrentLevelData.EnemyVariants[index].Variant;

					SpawnEnemy(variant, health);
				}
			}
		}
	}

	public void SetLevelData(ref List<LevelData> levelData)
	{
		m_LevelData = levelData;
		m_LevelData.Sort((x, y) => x.Index.CompareTo(y.Index));
	}

	public void SetBossExplodedDelegate(Boss.BossHasExploded explodedDelegate)
	{
		m_Boss.SetDelegate(explodedDelegate);
	}

	public void StartSpawning(int levelIndex)
	{
		if (levelIndex >= 0 && levelIndex < m_LevelData.Count || levelIndex == -1)
		{
			m_LevelIndex = levelIndex;
			m_CanSpawn = true;

			double minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
			double maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
			m_AsteroidSpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);

			minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
			maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
			m_EnemySpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);

			foreach (string type in CurrentLevelData.StartingAsteroids)
			{
				if (type == "Big")
					SpawnAsteroid(Asteroid.Size.Big);
				else if (type == "Medium")
					SpawnAsteroid(Asteroid.Size.Medium);
			}

			foreach (string variant in CurrentLevelData.StartingEnemies)
			  SpawnEnemy(variant, CurrentLevelData.GetHealthForEnemyVariant(variant));
		}
	}

	public void StopSpawning()
	{
		m_CanSpawn = false;
	}

	public void Reset()
	{
		m_CanSpawn = false;

		m_Boss.Disable();
		m_AsteroidPool.DisableAll();
		m_MissilePool.DisableAll();
		m_LaserPool.DisableAll();
		m_LaserImpactPool.DisableAll();
	  	m_ExplosionPool.DisableAll();
	  	m_EnemyPool.DisableAll();
		m_PickupPool.DisableAll();
		m_ShipWingPool.DisableAll();

		double minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
		double maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
		m_AsteroidSpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);

		minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
		maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
		m_EnemySpawnTimer = GD.RandRange(minSpawnTime, maxSpawnTime);
	}

	public void ExplodeAllActive()
	{
		// Explode the enemies and the asteroids
		List<Asteroid> activeAsteroids = m_AsteroidPool.GetActiveNodes();
		foreach (Asteroid asteroid in activeAsteroids)
			asteroid.Explode(true);

		List<Enemy> activeEnemies = m_EnemyPool.GetActiveNodes();
		foreach (Enemy enemy in activeEnemies)
			enemy.Explode();

		// Deactivate the pickups, lasers and missiles
		m_PickupPool.DisableAll();
		m_LaserPool.DisableAll();
		m_MissilePool.DisableAll();
	}

	public Boss SpawnBoss()
	{
		if (m_Boss != null)
			m_Boss.Spawn();
		return m_Boss;
	}

	public Asteroid SpawnAsteroid(Asteroid.Size size)
	{
		Vector2I windowSize = DisplayServer.WindowGetSize();
		float radians = (float)GD.RandRange(Constants.kAsteroidMinSpawnRadians, Constants.kAsteroidMaxSpawnRadians);
		float minSpeed = CurrentLevelData.AsteroidMinSpeed;
		float maxSpeed = CurrentLevelData.AsteroidMaxSpeed;
		float speed = (float)GD.RandRange(minSpeed, maxSpeed);
		Vector2 linearVelocity = new Vector2(Mathf.Cos(radians) * speed, Mathf.Sin(radians) * speed);
		Vector2 position = new Vector2((float)windowSize.X + Constants.kOffScreenSpawnBuffer, (float)GD.RandRange(0, windowSize.Y));
		return SpawnAsteroid(position, linearVelocity, size);
	}

	public Asteroid SpawnAsteroid(Vector2 position, Vector2 linearVelocity, Asteroid.Size size)
	{
		Asteroid asteroid = m_AsteroidPool.GetNodeFromPool();
		if (asteroid != null)
			asteroid.Spawn(position, linearVelocity, size);
		return asteroid;
	}

	private void SpawnEnemy(string variant, int health)
	{
		if (variant == "Enemy-1")
			SpawnEnemy(Enemy.Variant.One, health);
		else if (variant == "Enemy-2")
			SpawnEnemy(Enemy.Variant.Two, health);
		else if (variant == "Enemy-3")
			SpawnEnemy(Enemy.Variant.Three, health);
		else if (variant == "Enemy-4")
			SpawnEnemy(Enemy.Variant.Four, health);
		else if (variant == "Enemy-5")
			SpawnEnemy(Enemy.Variant.Five, health);
		else if (variant == "Boss")
			SpawnBoss();
	}

	public Enemy SpawnEnemy(Enemy.Variant variant, int health)
	{
		float radians = (float)GD.RandRange(Constants.kEnemyMinSpawnAngle, Constants.kEnemyMaxSpawnAngle);
		float minSpeed = CurrentLevelData.EnemyMinSpeed;
		float maxSpeed = CurrentLevelData.EnemyMaxSpeed;
		float speed = (float)GD.RandRange(minSpeed, maxSpeed);
		Vector2I windowSize = DisplayServer.WindowGetSize();
		Vector2 linearVelocity = new Vector2(MathF.Cos(radians) * speed, MathF.Sin(radians) * speed);
		Vector2 position = new Vector2((float)windowSize.X + Constants.kOffScreenSpawnBuffer, (float)GD.RandRange(0.0f, (float)windowSize.Y));
		return SpawnEnemy(position, linearVelocity, variant, health);
	}

	public Enemy SpawnEnemy(Vector2 position, Vector2 linearVelocity, Enemy.Variant variant, int health)
	{
		Enemy enemy = m_EnemyPool.GetNodeFromPool();
		if (enemy != null)
			enemy.Spawn(position, linearVelocity, variant, health);
		return enemy;
	}

	public Explosion SpawnExplosion(Vector2 position, Vector2 scale, float delay = 0.0f)
	{
		Explosion explosion = m_ExplosionPool.GetNodeFromPool();
		if (explosion != null)
			explosion.Spawn(position, scale, delay);
		return explosion;
	}

	public Missile SpawnMissile(Vector2 position, Vector2 linearVelocity, Missile.Size size)
	{
		Missile missile = m_MissilePool.GetNodeFromPool();
		if (missile != null)
			missile.Spawn(position, linearVelocity, size);
		return missile;
	}

	public Laser SpawnBlueLaser(Vector2 position, Vector2 linearVelocity)
	{
		return SpawnLaser(position, linearVelocity, Laser.Color.Blue);
	}

	public Laser SpawnRedLaser(Vector2 position, Vector2 linearVelocity)
	{
		return SpawnLaser(position, linearVelocity, Laser.Color.Red);
	}

	private Laser SpawnLaser(Vector2 position, Vector2 linearVelocity, Laser.Color color)
	{
		Laser laser = m_LaserPool.GetNodeFromPool();
		if (laser != null)
			laser.Spawn(position, linearVelocity, color);
		return laser;
	}

	public LaserImpact SpawnLaserImpact(Vector2 position, Laser.Color color)
	{
		LaserImpact laserImpact = m_LaserImpactPool.GetNodeFromPool();
		if (laserImpact != null)
			laserImpact.Spawn(position, color);
		return laserImpact;
	}

	public Pickup SpawnPickup(Vector2 position, Vector2 linearVelocity, Pickup.Variant variant)
	{
   		Pickup pickup = m_PickupPool.GetNodeFromPool();
		if (pickup != null)
			pickup.Spawn(position, linearVelocity, variant);
		return pickup;
	}

	public ShipWing SpawnBossWing(Vector2 initialPosition, float initialRadians, float offsetRadians, string texture)
	{
		float radians = initialRadians + offsetRadians; 
		Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));
		Vector2 position = initialPosition + direction * Constants.kBossWingExplosionOffset;
		Vector2 linearVelocity = direction * Constants.kBossWingExplosionSpeed;

		return SpawnWing(position, linearVelocity, initialRadians, texture);
	}

	public ShipWing SpawnWing(Vector2 initialPosition, float initialRadians, string texture, bool isLeft)
	{
		float speed = (float)GD.RandRange(Constants.kShipWingExplosionMinSpeed, Constants.kShipWingExplosionMaxSpeed);
		float halfPI = MathF.PI * 0.5f;
		float radians = initialRadians + (isLeft ? halfPI : -halfPI); // Perpendicular to the current direction
		Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));

		Vector2 position = initialPosition + direction * Constants.kShipWingExplosionOffset;
		Vector2 linearVelocity = direction * speed;

		return SpawnWing(position, linearVelocity, initialRadians, texture);
	}

	private ShipWing SpawnWing(Vector2 initialPosition, Vector2 linearVelocity, float initialRadians, string texture)
	{
   		ShipWing shipWing = m_ShipWingPool.GetNodeFromPool();
		if (shipWing != null)
			shipWing.Spawn(initialPosition, linearVelocity, initialRadians, texture);
		return shipWing;
	}

	private LevelData CurrentLevelData
	{
		get
		{   if (m_LevelIndex == -1)
				return m_DefaultLevel;
			return m_LevelData[m_LevelIndex];
		}
	}

	private bool HandleSpawnTimer(double delta, ref double timer)
	{
		if (timer > 0.0)
		{
			timer -= delta;
			if (timer <= 0.0)
				return true;
		}
		return false;
	}
}
