// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class Game : Node2D
{
	[Export] private Ship _Ship = null;
	[Export] private HUD _HUD = null;
	[Export] private Control _ClippingRect = null;

	public static Game sharedInstance;

	private List<double> m_LevelDurations = new List<double>();
	private List<string> m_LevelDescriptions = new List<string>();
	private int m_LevelCount = 0;
	private double m_ElapsedTime = 0.0;
	private double m_TimeScale = 1.0;
	private double m_LevelIndicatorDisplayTime;
	private double m_EndOfGameDisplayTime;
	private int m_LevelIndex = -1;

	private enum GameState
	{
		Authenticating,
		LoadingData,
		Gameplay,
		LevelTransition,
		GameOver,
		Victory
	}
	
	private GameState m_GameState = GameState.Authenticating;

	public enum Mode
	{
		Unknown = -1,
		Endless,
		Horde
	}

	private Mode m_Mode = Mode.Unknown;

	public double TimeScale
	{
		get { return m_TimeScale; }
		set { m_TimeScale = value; }
	}

	public Vector2 VisibleAreaSize
	{
		get { return _ClippingRect.Size; }
	}

	public Network GetNetwork
	{
		get { return GetNode<Network>("/root/Network"); }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sharedInstance = this;

		GD.Randomize();
 
		_Ship.Disable();
		_Ship.SetDelegate(OnShipHasExploded);

		Spawner.sharedInstance.SetBossExplodedDelegate(OnBossHasExploded);

		m_GameState = GameState.LoadingData;
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
 		if (m_GameState == GameState.GameOver)
		{
			if (m_EndOfGameDisplayTime > 0.0)
			{
				m_EndOfGameDisplayTime -= delta * m_TimeScale;
				if (m_EndOfGameDisplayTime < 0.0)
				{
					if(m_GameState == GameState.GameOver)
						_HUD.HideGameOver();
					if (m_GameState == GameState.Victory)
						_HUD.HideGameWon();

					if (m_Mode == Mode.Endless)
					{
						// TODO: Implement Leaderboards - replace showing the play again dialog
						//       with the Post score dialog
						DialogManager.sharedInstance.ShowPlayAgainDialog();
					}
					else if (m_Mode == Mode.Horde)
					{
						DialogManager.sharedInstance.ShowPlayAgainDialog();
					}
				}
			}
		}
		else if (m_GameState == GameState.Gameplay)
		{
			m_ElapsedTime += delta * m_TimeScale;
			_HUD.SetElapsedTime(m_ElapsedTime);

			if (m_Mode == Mode.Endless)
			{
				// TODO: Implement Leaderboards
			}
			else if (m_Mode == Mode.Horde)
			{
				if (m_ElapsedTime > m_LevelDurations[m_LevelIndex] && m_LevelDurations[m_LevelIndex] != -1.0)
				{
					m_ElapsedTime = m_LevelDurations[m_LevelIndex];
					NextLevel();
				}
			}
		}
		else if (m_GameState == GameState.LevelTransition)
		{
			if (m_LevelIndicatorDisplayTime > 0.0)
			{
				m_LevelIndicatorDisplayTime -= delta * m_TimeScale;

				if (m_LevelIndicatorDisplayTime < 0.0)
					StartLevel();
			}
		}
	}

	public bool IsGameOver()
	{
		return m_GameState == GameState.GameOver;
	}

	public bool IsGameWon()
	{
		return m_GameState == GameState.Victory;
	}

	public void StartEndlessMode()
	{
		m_Mode = Mode.Endless;
		PrepareLevel(-1); // -1 is the default level used for endless mode
	}

	public void StartHordeMode(int levelIndex = 0)
	{
		m_Mode = Mode.Horde;
		PrepareLevel(levelIndex);
	}

	public void Reset(bool startNewGame)
	{
		Spawner.sharedInstance.Reset();
		_Ship.Reset();
		_HUD.Reset();

		if (startNewGame)
		{
			if (m_Mode == Mode.Endless)
				StartEndlessMode();
			else if (m_Mode == Mode.Horde)
				StartHordeMode();
		}
	}

	public Ship GetShip()
	{
		return _Ship;
	}

	public void OnCheatAddTime()
	{
		if (m_GameState == GameState.Gameplay)
			m_ElapsedTime += 15.0f;
	}

	public void OnCheatNextLevel()
	{
		if (m_GameState == GameState.Gameplay)
			NextLevel();
	}

	public void OnCheatHealUp()
	{
		if (m_GameState == GameState.Gameplay)
			_Ship.HealUp();
	}

	private void PrepareLevel(int levelIndex)
	{
		m_GameState = GameState.LevelTransition;
		m_LevelIndicatorDisplayTime = Constants.kLevelDisplayDuration;
		m_LevelIndex = levelIndex;
		m_ElapsedTime = 0.0f;

		if (m_Mode == Mode.Endless)
		{
			// TODO: Implement Leaderboards

			_HUD.ShowLevel(-1, "");
		}
		else if (m_Mode == Mode.Horde)
		{
			_HUD.ShowLevel(m_LevelIndex + 1, m_LevelDescriptions[m_LevelIndex]);
			_HUD.PushLevelGoal(m_LevelDescriptions[m_LevelIndex]);
		}
	}

	private void StartLevel()
	{
		m_GameState = GameState.Gameplay;

		_HUD.HideLevel();

		Spawner.sharedInstance.StartSpawning(m_LevelIndex);

		if (!_Ship.IsEnabled())
		{
			_Ship.Enable();
			_Ship.Spawn();
		}

		return;
	}

	private void NextLevel()
	{
		Spawner.sharedInstance.StopSpawning();
		Spawner.sharedInstance.ExplodeAllActive();

		if (m_LevelIndex + 1 < m_LevelCount)
		{
			_Ship.HealUp();
			PrepareLevel(m_LevelIndex + 1);
		}
	}

	private void OnShipHasExploded()
	{
		m_GameState = GameState.GameOver;
		m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
		_HUD.ShowGameOver();
	}

	public void OnBossHasExploded()
	{	
		m_GameState = GameState.Victory;
		m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
		_HUD.ShowGameWon();

		Spawner.sharedInstance.StopSpawning();
		Spawner.sharedInstance.ExplodeAllActive();
	}
}
