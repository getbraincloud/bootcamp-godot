// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


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
	private int m_LeaderboardEntryIndex = -1;
	private UserProgress m_UserProgress = new UserProgress();

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

	public String AppVersion
	{
		get { return ProjectSettings.GetSetting("application/config/version").ToString(); }
	}

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
 
		_HUD.SetAppVersion(AppVersion);
		_HUD.SetBrainCloudVersion(GetNetwork.BrainCloudClientVersion);

		_Ship.Disable();
		_Ship.SetDelegate(OnShipHasExploded);

		Spawner.sharedInstance.SetBossExplodedDelegate(OnBossHasExploded);

		HandleAuthentication();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
 		if (m_GameState == GameState.GameOver || m_GameState == GameState.Victory)
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
						if(GetNetwork.IsAuthenticated())
						{
							if (GetNetwork.IsUsernameSaved())
								GetNetwork.PostScoreToLeaderboards(m_ElapsedTime, OnPostScoreRequestCompleted);
							else
								DialogManager.sharedInstance.ShowPostScoreDialog(m_ElapsedTime, OnPostScoreRequestCompleted);
						}
						else
						{
							DialogManager.sharedInstance.ShowPlayAgainDialog();
						}
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
				CheckEndlessModeTimeAchievements();

				if (m_LeaderboardEntryIndex != -1)
				{
					Leaderboard leaderboard = GetNetwork.GetLeaderboardsManager.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);
					if (leaderboard != null)
					{
						LeaderboardEntry leaderboardEntry = leaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex);
						if (leaderboardEntry != null && m_ElapsedTime > leaderboardEntry.Time)
							DisplayNextHighScore();
					}
				}
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

	public void HandleAuthentication()
	{
		m_GameState = GameState.Authenticating;

		if (GetNetwork.HasAuthenticatedPreviously())
		{
			DialogManager.sharedInstance.ShowConnectingDialog();
			GetNetwork.Reconnect(OnAuthenticationRequestCompleted, OnAuthenticationRequestFailed);
		}
		else
		{
			DialogManager.sharedInstance.ShowEmailLoginDialog(OnAuthenticationRequestCompleted, OnAuthenticationRequestFailed);
		}
	}

	public void OnAuthenticationRequestCompleted(ref AuthenticationData authenticationData)
	{		
		if (m_GameState == GameState.Authenticating)
		{
			m_GameState = GameState.LoadingData;

			DialogManager.sharedInstance.HideConnectingDialog();
			DialogManager.sharedInstance.ShowMainMenuDialog();

			//Set the LevelData
			if (authenticationData.LevelDataList != null)
			{
				List<LevelData> levelData = authenticationData.LevelDataList;
				Spawner.sharedInstance.SetLevelData(ref levelData);

				m_LevelCount = levelData.Count;

				foreach (LevelData ld in levelData)
				{
					m_LevelDurations.Add(ld.Duration);
					m_LevelDescriptions.Add(ld.Description);
				}
			}
			else
				GetNetwork.RequestLevelData(OnLevelDataRequestCompleted);

			// Set the Main Leaderboard
			if (authenticationData.MainLeaderboard == null)
				GetNetwork.RequestLeaderboard(Constants.kBrainCloudMainLeaderboardID);

			// Set the Daily Leaderboard
			if (authenticationData.DailyLeaderboard == null)
				GetNetwork.RequestLeaderboard(Constants.kBrainCloudDailyLeaderboardID);

			// Set the Country Leaderboard
			if (authenticationData.CountryLeaderboard == null)
				GetNetwork.RequestCountryLeaderboard();

			//Set the Statistics
			if (authenticationData.StatisticsList == null)
				GetNetwork.RequestUserStatistics();

			//Set the Achievements
			if (authenticationData.AchievementsList == null)
				GetNetwork.RequestAchievements();

			//Set the UserProgress
			if (authenticationData.UserProgressData != null)
				m_UserProgress = authenticationData.UserProgressData;
			else
				GetNetwork.RequestUserProgressData(OnUserProgressDataRequestCompleted);
		}
	}

	public void OnAuthenticationRequestFailed(string errorMessage)
	{
		if (m_GameState == GameState.Authenticating)
		{
			m_GameState = GameState.Gameplay;

			DialogManager.sharedInstance.HideConnectingDialog();
			DialogManager.sharedInstance.ShowErrorDialog(errorMessage);

			GetNetwork.ResetStoredProfileId();
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

	public UserProgress GetUserProgress()
	{
		return m_UserProgress;
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
			Leaderboard mainLeaderboard = GetNetwork.GetLeaderboardsManager.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);

			if (mainLeaderboard != null && mainLeaderboard.GetCount() > 0)
			{
				m_LeaderboardEntryIndex = mainLeaderboard.GetCount() - 1;
				_HUD.PushLeaderboardEntry(mainLeaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex));
			}

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

		Achievement beatLevel1 = GetNetwork.GetAchievementsManager.GetAchievementByID(Constants.kBrainCloudAchievementBeatLevel1);
		if (beatLevel1 != null && !beatLevel1.IsAwarded && m_LevelIndex == 0)
		{
			beatLevel1.Award();
			_HUD.PushAchievement(beatLevel1);
		}

		bool needsSync = m_UserProgress.SetLevelCompleted(m_LevelIndex);

		if (needsSync)
			GetNetwork.UpdateUserProgressData(m_UserProgress.EntityID, m_UserProgress.EntityType, m_UserProgress.JsonData);

		if (m_LevelIndex + 1 < m_LevelCount)
		{
			_Ship.HealUp();
			PrepareLevel(m_LevelIndex + 1);
		}
	}

	private void DisplayNextHighScore()
	{
		Leaderboard leaderboard = GetNetwork.GetLeaderboardsManager.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);

		if (m_LeaderboardEntryIndex == -1 || leaderboard == null || leaderboard.GetCount() == 0)
			return;

		if (m_LeaderboardEntryIndex > 0)
		{
			m_LeaderboardEntryIndex--;
			_HUD.PushLeaderboardEntry(leaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex));
		}
		else if (m_LeaderboardEntryIndex == 0)
		{
			_HUD.SetPlayerHasAllTimeHighScore();
		}
	}

	private void CheckEndlessModeTimeAchievements()
	{
		Achievement survive30 = GetNetwork.GetAchievementsManager.GetAchievementByID(Constants.kBrainCloudAchievementSurvive30);
		if (survive30 != null && !survive30.IsAwarded && m_ElapsedTime >= 30.0f)
		{
			survive30.Award();
			_HUD.PushAchievement(survive30);
		}

		Achievement survive60 = GetNetwork.GetAchievementsManager.GetAchievementByID(Constants.kBrainCloudAchievementSurvive60);
		if (survive60 != null && !survive60.IsAwarded && m_ElapsedTime >= 60.0f)
		{
			survive60.Award();
			_HUD.PushAchievement(survive60);
		}
	}

	private void OnShipHasExploded()
	{
		m_GameState = GameState.GameOver;
		m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
		_HUD.ShowGameOver();

		// Update the statistics
		Statistic gamesPlayed = GetNetwork.GetStatisticsManager.GetStatisticByName(Constants.kBrainCloudStatGamesPlayed);
		if (gamesPlayed != null)
			gamesPlayed.ApplyIncrement();

		// Send all the statistics increments to brainCloud now that the game has ended
		Dictionary<string, object> dictionary = GetNetwork.GetStatisticsManager.GetIncrementsDictionary();
		if (dictionary != null)
			GetNetwork.IncrementUserStatistics(dictionary);
	}

	public void OnBossHasExploded()
	{	
		m_GameState = GameState.Victory;
		m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
		_HUD.ShowGameWon();

		Spawner.sharedInstance.StopSpawning();
		Spawner.sharedInstance.ExplodeAllActive();

		// Update the statistics
		Statistic gamesPlayed = GetNetwork.GetStatisticsManager.GetStatisticByName(Constants.kBrainCloudStatGamesPlayed);
		if (gamesPlayed != null)
			gamesPlayed.ApplyIncrement();

		// Send all the statistics increments to brainCloud now that the game has ended
		Dictionary<string, object> dictionary = GetNetwork.GetStatisticsManager.GetIncrementsDictionary();
		if(dictionary != null)
			GetNetwork.IncrementUserStatistics(dictionary);

		// The user beat the boss level, update the user's progress
		bool needsSync = m_UserProgress.SetLevelCompleted(m_LevelIndex);
		if (needsSync)
			GetNetwork.UpdateUserProgressData(m_UserProgress.EntityID, m_UserProgress.EntityType, m_UserProgress.JsonData);
	}

	private void OnLeaderboardRequestCompleted(ref Leaderboard leaderboard)
	{
		if (IsGameOver())
		{
			if (m_Mode == Mode.Endless && leaderboard.Name == Constants.kBrainCloudMainLeaderboardID)
			{
				DialogManager.sharedInstance.ShowPlayAgainDialog();
				DialogManager.sharedInstance.ShowLeaderboardsDialog();
			}
		}
	}

	private void OnPostScoreRequestCompleted()
	{
		GetNetwork.GetLeaderboardsManager.SetUserTime(m_ElapsedTime);
		GetNetwork.RequestLeaderboard(Constants.kBrainCloudMainLeaderboardID, OnLeaderboardRequestCompleted);
		GetNetwork.RequestLeaderboard(Constants.kBrainCloudDailyLeaderboardID);
		GetNetwork.RequestCountryLeaderboard();
	}

	private void OnLevelDataRequestCompleted(ref List<LevelData> levelData)
	{
		Spawner.sharedInstance.SetLevelData(ref levelData);

		m_LevelCount = levelData.Count;

		foreach (LevelData ld in levelData)
		{
			m_LevelDurations.Add(ld.Duration);
			m_LevelDescriptions.Add(ld.Description);
		}

		DialogManager.sharedInstance.HideConnectingDialog();
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}

	private void OnUserProgressDataRequestCompleted(UserProgress userProgress)
	{
		if (userProgress != null)
		{
			m_UserProgress = userProgress;
		}
		else
		{
			// User entity for User progress data doesn't exist, create one
			GetNetwork.CreateUserProgressData();
		}
	}
}
