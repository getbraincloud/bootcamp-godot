// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public class LevelData
{
	public class EnemyVariant
	{
		string m_Variant;
		int m_Health;

		public EnemyVariant(string variant, int health)
		{
			m_Variant = variant;
			m_Health = health;
		}

		public string Variant
		{
			get { return m_Variant; }
		}

		public int Health
		{
			get { return m_Health; }
		}
	}

	private List<EnemyVariant> m_EnemyVariants;
	private List<string> m_StartingAsteroids;
	private List<string> m_StartingEnemies;
	private string m_EntityType;
	private string m_EntityID;
	private string m_Description;
	private int m_Index;
	private double m_Duration;
	private int m_AsteroidMinSpawnCount;
	private int m_AsteroidMaxSpawnCount;
	private double m_AsteroidMinSpawnTime;
	private double m_AsteroidMaxSpawnTime;
	private float m_AsteroidMinSpeed;
	private float m_AsteroidMaxSpeed;
	private int m_EnemyMinSpawnCount;
	private int m_EnemyMaxSpawnCount;
	private double m_EnemyMinSpawnTime;
	private double m_EnemyMaxSpawnTime;
	private float m_EnemyMinSpeed;
	private float m_EnemyMaxSpeed;

	public LevelData()
	{
		m_EntityType = "Default";
		m_EntityID = "-1";
		m_Index = -1;
		m_EnemyVariants = new List<EnemyVariant>();
		m_StartingAsteroids = new List<string>();
		m_StartingEnemies = new List<string>();

		m_Duration = -1.0;
		m_Description = "Survive as long as possible";
		m_AsteroidMinSpawnCount = Constants.kAsteroidMinSpawnCount;
		m_AsteroidMaxSpawnCount = Constants.kAsteroidMaxSpawnCount;
		m_AsteroidMinSpawnTime = Constants.kAsteroidMinSpawnTime;
		m_AsteroidMaxSpawnTime = Constants.kAsteroidMaxSpawnTime;
		m_AsteroidMinSpeed = Constants.kAsteroidMinSpeed;
		m_AsteroidMaxSpeed = Constants.kAsteroidMaxSpeed;

		m_EnemyMinSpawnCount = Constants.kEnemyMinSpawnCount;
		m_EnemyMaxSpawnCount = Constants.kEnemyMaxSpawnCount;
		m_EnemyMinSpawnTime = Constants.kEnemyMinSpawnTime;
		m_EnemyMaxSpawnTime = Constants.kEnemyMaxSpawnTime;
		m_EnemyMinSpeed = Constants.kEnemyMinSpeed;
		m_EnemyMaxSpeed = Constants.kEnemyMaxSpeed;

		m_EnemyVariants.Add(new EnemyVariant("Enemy-1", 2));
		m_EnemyVariants.Add(new EnemyVariant("Enemy-2", 3));
		m_EnemyVariants.Add(new EnemyVariant("Enemy-3", 3));
		m_EnemyVariants.Add(new EnemyVariant("Enemy-4", 4));
		m_EnemyVariants.Add(new EnemyVariant("Enemy-5", 4));

		m_StartingAsteroids.Add("Big");
		m_StartingAsteroids.Add("Big");
		m_StartingAsteroids.Add("Medium");
	}

	public int GetHealthForEnemyVariant(string enemyVariant)
	{
		foreach (EnemyVariant et in m_EnemyVariants)
			if (et.Variant == enemyVariant)
				return et.Health;

		return 1;
	}

	public string EntityType
	{
		get { return m_EntityType; }
	}

	public string EntityID
	{
		get { return m_EntityID; }
	}

	public string Description
	{
		get { return m_Description; }
	}

	public int Index
	{
		get { return m_Index; }
	}

	public double Duration
	{
		get { return m_Duration; }
	}

	public int AsteroidMinSpawnCount
	{
		get { return m_AsteroidMinSpawnCount; }
	}

	public int AsteroidMaxSpawnCount
	{
		get { return m_AsteroidMaxSpawnCount; }
	}

	public double AsteroidMinSpawnTime
	{
		get { return m_AsteroidMinSpawnTime; }
	}

	public double AsteroidMaxSpawnTime
	{
		get { return m_AsteroidMaxSpawnTime; }
	}

	public float AsteroidMinSpeed
	{
		get { return m_AsteroidMinSpeed; }
	}

	public float AsteroidMaxSpeed
	{
		get { return m_AsteroidMaxSpeed; }
	}

	public int EnemyMinSpawnCount
	{
		get { return m_EnemyMinSpawnCount; }
	}

	public int EnemyMaxSpawnCount
	{
		get { return m_EnemyMaxSpawnCount; }
	}

	public double EnemyMinSpawnTime
	{
		get { return m_EnemyMinSpawnTime; }
	}

	public double EnemyMaxSpawnTime
	{
		get { return m_EnemyMaxSpawnTime; }
	}

	public float EnemyMinSpeed
	{
		get { return m_EnemyMinSpeed; }
	}

	public float EnemyMaxSpeed
	{
		get { return m_EnemyMaxSpeed; }
	}

	public List<EnemyVariant> EnemyVariants
	{
		get { return m_EnemyVariants; }
	}

	public List<string> StartingAsteroids
	{
		get { return m_StartingAsteroids; }
	}

	public List<string> StartingEnemies
	{
		get { return m_StartingEnemies; }
	}
}
