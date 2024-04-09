// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;


public partial class Network : Node
{
	public delegate void AuthenticationRequestCompleted();
	public delegate void AuthenticationRequestFailed(string errorMessage);
	public delegate void BrainCloudLogOutCompleted();
	public delegate void BrainCloudLogOutFailed(string errorMessage);
	public delegate void UpdateUsernameRequestCompleted();
	public delegate void UpdateUsernameRequestFailed(string errorMessage);
	public delegate void LeaderboardRequestCompleted(ref Leaderboard leaderboard);
	public delegate void LeaderboardRequestFailed(string errorMessage);
	public delegate void PostScoreRequestCompleted();
	public delegate void PostScoreRequestFailed(string errorMessage);
	public delegate void LevelDataRequestCompleted(ref List<LevelData> levels);
	public delegate void LevelDataRequestFailed(string errorMessage);
	public delegate void UserStatisticsRequestCompleted(ref List<Statistic> statistics);
	public delegate void UserStatisticsRequestFailed(string errorMessage);
	public delegate void IncrementUserStatisticsCompleted(ref List<Statistic> statistics);
	public delegate void IncrementUserStatisticsFailed(string errorMessage);
	public delegate void AchievementsRequestCompleted(ref List<Achievement> achievements);
	public delegate void AchievementsRequestFailed(string errorMessage);
	public delegate void AchievementAwardedCompleted();
	public delegate void AchievementAwardedFailed(string errorMessage);
	public delegate void UserProgressDataRequestCompleted(UserProgress userProgress);
	public delegate void UserProgressDataRequestFailed(string errorMessage);
	public delegate void CreateUserProgressDataCompleted();
	public delegate void CreateUserProgressDataFailed(string errorMessage);
	public delegate void UpdateUserProgressDataCompleted();
	public delegate void UpdateUserProgressDataFailed(string errorMessage);
	public delegate void GetIdentitiesRequestCompleted();
	public delegate void GetIdentitiesRequestFailed(string errorMessage);
	public delegate void AttachEmailIdentityCompleted();
	public delegate void AttachEmailIdentityFailed(string errorMessage);

	private delegate void ErrorDelegate<T1>(T1 a);

	private BrainCloudWrapper m_BrainCloud;
	private bool m_EnabledLogging = true;
	private string m_Username;
	private LeaderboardsManager m_LeaderboardsManager = new LeaderboardsManager();
	private StatisticsManager m_StatisticsManager = new StatisticsManager();
	private AchievementsManager m_AchievementsManager = new AchievementsManager();
	private List<string> m_IdentityTypesList = new List<string>();


	public bool EnableLogging
	{
		get { return m_EnabledLogging; }
		set { m_EnabledLogging = value; }
	}

	public string BrainCloudClientVersion
	{
		get { return m_BrainCloud.Client.BrainCloudClientVersion; }
	}

	public LeaderboardsManager GetLeaderboardsManager
	{
		get { return m_LeaderboardsManager; }
	}

	public StatisticsManager GetStatisticsManager
	{
		get { return m_StatisticsManager; }
	}

	public AchievementsManager GetAchievementsManager
	{
		get { return m_AchievementsManager; }
	}

	public List<string> IdentityTypesList
	{
		get { return m_IdentityTypesList; }
	}

	public override void _Ready()
	{
		// Create and initialize the BrainCloud wrapper
		m_BrainCloud = new BrainCloudWrapper();
		m_BrainCloud.Init(Constants.kBrainCloudServer, Constants.kBrainCloudAppSecret, Constants.kBrainCloudAppID, Constants.kBrainCloudAppVersion);

		// Log the BrainCloud client version
		OutputLog("BrainCloud client version: " + m_BrainCloud.Client.BrainCloudClientVersion);
	}

	public override void _Process(double delta)
	{
		// Make sure you invoke this method in Update, or else you won't get any callbacks
		m_BrainCloud.RunCallbacks();
	}

	public override void _Notification(int notification)
	{
		if (notification == NotificationWMCloseRequest) 
		{
 			EndSession();
			GetTree().Quit(); // default behavior
		}
	}

	public bool HasAuthenticatedPreviously()
	{
		return m_BrainCloud.GetStoredProfileId() != "" && m_BrainCloud.GetStoredAnonymousId() != "";
	}

	public bool IsAuthenticated()
	{
		return m_BrainCloud.Client.Authenticated;
	}

	public bool IsUsernameSaved()
	{
		return m_Username != "";
	}

	public string GetUsername()
	{
		return m_Username;
	}

	public void ResetStoredProfileId()
	{
		m_BrainCloud.ResetStoredProfileId();
	}

	public void EndSession()
	{
		m_BrainCloud.Logout(false);
	}

	public void LogOut(BrainCloudLogOutCompleted brainCloudLogOutCompleted = null, BrainCloudLogOutFailed brainCloudLogOutFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Log out error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(brainCloudLogOutFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Log out successful: " + responseData);

			if (brainCloudLogOutCompleted != null)
				brainCloudLogOutCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Log out error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(brainCloudLogOutFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.Logout(true, successCallback, failureCallback);
	}

	public void Reconnect(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
            HandleAuthenticationError(reasonCode, jsonError, authenticationRequestFailed);
		};

		// Make the BrainCloud request
		m_BrainCloud.Reconnect(successCallback, failureCallback);
	}

	public void AuthenticateAnonymous(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
            HandleAuthenticationError(reasonCode, jsonError, authenticationRequestFailed);
		};

		// Make the BrainCloud request
		m_BrainCloud.AuthenticateAnonymous(successCallback, failureCallback);
	}

	public void AuthenticateUniversal(string userID, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
            HandleAuthenticationError(reasonCode, jsonError, authenticationRequestFailed);
		};

		// Make the BrainCloud request
		m_BrainCloud.AuthenticateUniversal(userID, password, true, successCallback, failureCallback);
	}

	public void AuthenticateEmailPassword(string email, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
            HandleAuthenticationError(reasonCode, jsonError, authenticationRequestFailed);
		};

		// Make the BrainCloud request
		m_BrainCloud.AuthenticateEmailPassword(email, password, true, successCallback, failureCallback);
	}

	public void UpdateUsername(string username, UpdateUsernameRequestCompleted updateUsernameRequestCompleted = null, UpdateUsernameRequestFailed updateUsernameRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Update username error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(updateUsernameRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Update username successful: " + responseData);

			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			m_Username = data["playerName"] as string;

			if (updateUsernameRequestCompleted != null)
				updateUsernameRequestCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Update username error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(updateUsernameRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.PlayerStateService.UpdateName(username, successCallback, failureCallback);
	}

	public void RequestLeaderboard(string leaderboardId, LeaderboardRequestCompleted leaderboardRequestCompleted = null, LeaderboardRequestFailed leaderboardRequestFailed = null)
	{
		RequestLeaderboard(leaderboardId, Constants.kBrainCloudDefaultMinHighScoreIndex, Constants.kBrainCloudDefaultMaxHighScoreIndex, leaderboardRequestCompleted, leaderboardRequestFailed);
	}

	public void RequestLeaderboard(string leaderboardId, int startIndex, int endIndex, LeaderboardRequestCompleted leaderboardRequestCompleted = null, LeaderboardRequestFailed leaderboardRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Request Leaderboard error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(leaderboardRequestFailed));
			return;
		}
			
		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Request Leaderboard successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Leaderboard leaderboard = ParseLeaderboard(ref data);
			GetLeaderboardsManager.AddLeaderboard(ref leaderboard);

			if (leaderboardRequestCompleted != null)
				leaderboardRequestCompleted(ref leaderboard);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Request Leaderboard error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(leaderboardRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.LeaderboardService.GetGlobalLeaderboardPage(leaderboardId, BrainCloud.BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW, startIndex, endIndex, successCallback, failureCallback);
	}

	public void PostScoreToLeaderboard(string leaderboardID, double time, PostScoreRequestCompleted postScoreRequestCompleted = null, PostScoreRequestFailed postScoreRequestFailed = null)
	{
		PostScoreToLeaderboard(leaderboardID, time, GetUsername(), postScoreRequestCompleted, postScoreRequestFailed);
	}

	public void PostScoreToLeaderboard(string leaderboardID, double time, string nickname, PostScoreRequestCompleted postScoreRequestCompleted = null, PostScoreRequestFailed postScoreRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Post Score to Leaderboard error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(postScoreRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Post Score to Leaderboard successful: " + responseData);

			if (postScoreRequestCompleted != null)
				postScoreRequestCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Post Score to Leaderboard error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(postScoreRequestFailed));
		};

		// Make the BrainCloud request
		long score = (long)(time * 1000.0);   // Convert the time from seconds to milleseconds
		string jsonOtherData = "{\"nickname\":\"" + nickname + "\"}";
		m_BrainCloud.LeaderboardService.PostScoreToLeaderboard(leaderboardID, score, jsonOtherData, successCallback, failureCallback);
	}

	public void RequestLevelData(LevelDataRequestCompleted levelDataRequestCompleted = null, LevelDataRequestFailed levelDataRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Request Level Data error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(levelDataRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Request Level Data successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> entityData = response["data"] as Dictionary<string, object>;
			List<LevelData> levelData = ParseLevelData(ref entityData);

			if (levelDataRequestCompleted != null)
				levelDataRequestCompleted(ref levelData);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Request Level Data error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(levelDataRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.GlobalEntityService.GetListByIndexedId(Constants.kBrainCloudGlobalEntityIndexedID, 5, successCallback, failureCallback);
	}

	public void RequestUserStatistics(UserStatisticsRequestCompleted userStatisticsRequestCompleted = null, UserStatisticsRequestFailed userStatisticsRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Request User Statistics error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(userStatisticsRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog(" User Statistics successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Dictionary<string, object> statistics = data["statistics"] as Dictionary<string, object>;
			List<Statistic> statisticsList = ParseStatistics(ref statistics);

			GetStatisticsManager.SetStatistics(ref statisticsList);

			if (userStatisticsRequestCompleted != null)
				userStatisticsRequestCompleted(ref statisticsList);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Request User Statistics error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(userStatisticsRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.PlayerStatisticsService.ReadAllUserStats(successCallback, failureCallback);
	}

	public void IncrementUserStatistics(Dictionary<string, object> data, IncrementUserStatisticsCompleted incrementUserStatisticsCompleted = null, IncrementUserStatisticsFailed incrementUserStatisticsFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Increment User Statistics error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(incrementUserStatisticsFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Increment User Statistics successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Dictionary<string, object> statistics = data["statistics"] as Dictionary<string, object>;
			List<Statistic> statisticsList = ParseStatistics(ref statistics);

			GetStatisticsManager.SetStatistics(ref statisticsList);

			if (incrementUserStatisticsCompleted != null)
				incrementUserStatisticsCompleted(ref statisticsList);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Increment User Statistics error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(incrementUserStatisticsFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.PlayerStatisticsService.IncrementUserStats(data, successCallback, failureCallback);
	}

	public void RequestAchievements(AchievementsRequestCompleted achievementsRequestCompleted = null, AchievementsRequestFailed achievementsRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Request Achievements error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(achievementsRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Request Achievements successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Array achievementArray = data["achievements"] as Array;
			List<Achievement> achievementsList = ParseAchievements(ref achievementArray);

			GetAchievementsManager.SetAchievements(ref achievementsList);

			if (achievementsRequestCompleted != null)
				achievementsRequestCompleted(ref achievementsList);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Request Achievements error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(achievementsRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.GamificationService.ReadAchievements(true, successCallback, failureCallback);
	}

	public void AwardAchievement(Achievement achievement, AchievementAwardedCompleted achievementAwardedCompleted = null, AchievementAwardedFailed achievementAwardedFailed = null)
	{
		List<Achievement> achievements = new List<Achievement>() {achievement};
		AwardAchievements(ref achievements);
	}

	public void AwardAchievements(ref List<Achievement> achievements, AchievementAwardedCompleted achievementAwardedCompleted = null, AchievementAwardedFailed achievementAwardedFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Award Achievements error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(achievementAwardedFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Award Achievements successful: " + responseData);

			if (achievementAwardedCompleted != null)
				achievementAwardedCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Award Achievements error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(achievementAwardedFailed));
		};

		// Make the BrainCloud request
		List<string> achievementIds = new List<string>();
		foreach(Achievement achievement in achievements)
			achievementIds.Add(achievement.ID);

		m_BrainCloud.GamificationService.AwardAchievements(achievementIds, successCallback, failureCallback);
	}

	public void RequestUserProgressData(UserProgressDataRequestCompleted userProgressDataRequestCompleted = null, UserProgressDataRequestFailed userProgressDataRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Request User Progress error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(userProgressDataRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Request User Progress successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Array entitiesArray = data["entities"] as Array;

			UserProgress userProgress = null;

			if (entitiesArray.Length > 0)
			{
				Dictionary<string, object> entityData = entitiesArray.GetValue(0) as Dictionary<string, object>;
				userProgress = ParseUserProgress(ref entityData);
			}

			if (userProgressDataRequestCompleted != null)
				userProgressDataRequestCompleted(userProgress);
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Request User Progress error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(userProgressDataRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.EntityService.GetEntitiesByType(Constants.kBrainCloudUserProgressUserEntityType, successCallback, failureCallback);
	}

	public void CreateUserProgressData(CreateUserProgressDataCompleted createUserProgressDataCompleted = null, CreateUserProgressDataFailed createUserProgressDataFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Create User Progress error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(createUserProgressDataFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Create User Progress successful: " + responseData);

			if (createUserProgressDataCompleted != null)
				createUserProgressDataCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Create User Progress error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(createUserProgressDataFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.EntityService.CreateEntity(Constants.kBrainCloudUserProgressUserEntityType,
												Constants.kBrainCloudUserProgressUserEntityDefaultData,
												Constants.kBrainCloudUserProgressUserEntityDefaultAcl,
												successCallback, failureCallback);
	}

	public void UpdateUserProgressData(string entityID, string entityType, string jsonData, UpdateUserProgressDataCompleted updateUserProgressDataCompleted = null, UpdateUserProgressDataFailed updateUserProgressDataFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Update User Progress error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(updateUserProgressDataFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Update User Progress successful: " + responseData);

			if (updateUserProgressDataCompleted != null)
				updateUserProgressDataCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Update User Progress error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(updateUserProgressDataFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.EntityService.UpdateEntity(entityID, entityType, jsonData, Constants.kBrainCloudUserProgressUserEntityDefaultAcl, -1, successCallback, failureCallback);
	}

	public void GetIdentities(GetIdentitiesRequestCompleted getIdentitiesRequestCompleted = null, GetIdentitiesRequestFailed getIdentitiesRequestFailed = null)
	{
		// Check if the user is authenticated
		if (!IsAuthenticated())
		{
			string errorMessage = "Get Identities error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(getIdentitiesRequestFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Get Identities successful: " + responseData);

			// Parse the JSON data
			Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
			Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
			Dictionary<string, object> identities = data["identities"] as Dictionary<string, object>;
			ParseIdentities(ref identities);

			if (getIdentitiesRequestCompleted != null)
				getIdentitiesRequestCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Get Identities error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(getIdentitiesRequestFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.IdentityService.GetIdentities(successCallback, failureCallback);
	}

	public void AttachEmailIdentity(string email, string password, AttachEmailIdentityCompleted attachEmailIdentityCompleted = null, AttachEmailIdentityFailed attachEmailIdentityFailed = null)
	{
		if (!IsAuthenticated())
		{
			string errorMessage = "Attach Email Identity error: user is not authenticated";
			HandleRequestError(errorMessage, new ErrorDelegate<string>(attachEmailIdentityFailed));
			return;
		}

		// Success callback lambda
		BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
		{
			OutputLog("Attach Email Identity successful: " + responseData);

			// Ensure the Identity types list doesn't contain an email identity, if it doesn't add it
			if (!m_IdentityTypesList.Contains("Email"))
				m_IdentityTypesList.Add("Email");

			if (attachEmailIdentityCompleted != null)
				attachEmailIdentityCompleted();
		};

		// Failure callback lambda
		BrainCloud.FailureCallback failureCallback = (status, reasonCode, jsonError, cbObject) =>
		{
			string errorMessage = "Attach Email Identity error: " + ExtractStatusMessage(jsonError);
			HandleRequestError(errorMessage, new ErrorDelegate<string>(attachEmailIdentityFailed));
		};

		// Make the BrainCloud request
		m_BrainCloud.IdentityService.AttachEmailIdentity(email, password, successCallback, failureCallback);
	}

	private void HandleAuthenticationSuccess(string responseData, object cbObject, AuthenticationRequestCompleted authenticationRequestCompleted)
	{
		OutputLog("Authentication successful: " + responseData);

		// Read the player name from the response data
		Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
		Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
		m_Username = data["playerName"] as string;
		
		// Parse the Statistics data
		Dictionary<string, object>  statisticsData = data["statistics"] as Dictionary<string, object>;
		List<Statistic> statisticsList = ParseStatistics(ref statisticsData);
		GetStatisticsManager.SetStatistics(ref statisticsList);

		if (authenticationRequestCompleted != null)
			authenticationRequestCompleted();
	}

	private void HandleAuthenticationError(int reasonCode, string jsonError, AuthenticationRequestFailed authenticationRequestFailed)
    {
        if (reasonCode == BrainCloud.ReasonCodes.GAME_VERSION_NOT_SUPPORTED)
        {
            HandleAppVersionError(jsonError, authenticationRequestFailed);
        }
        else
        {
            string errorMessage = "Authentication error: " + ExtractStatusMessage(jsonError);
            HandleRequestError(errorMessage, new ErrorDelegate<string>(authenticationRequestFailed));
        }
    }

	private void HandleRequestError(string errorMessage, ErrorDelegate<string> errorDelegate)
	{
		OutputLog(errorMessage);

		if (errorDelegate != null)
			errorDelegate(errorMessage);
	}

	private void HandleAppVersionError(string errorJson, AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(errorJson);
		string upgradeAppIdMessage = response["upgradeAppId"] as string;
		string errorMessage = "Authentication error: " + upgradeAppIdMessage;
		HandleRequestError(upgradeAppIdMessage, new ErrorDelegate<string>(authenticationRequestFailed));
	}

	private List<Statistic> ParseStatistics(ref Dictionary<string, object> statisticsData)
	{
		List<Statistic> statisticsList = new List<Statistic>();
		int value = 0;
		string description;

		foreach (string key in statisticsData.Keys)
		{
			value = int.Parse(statisticsData[key].ToString());
			description = Constants.kBrainCloudStatDescriptions[key];
			statisticsList.Add(new Statistic(key, description, value));
		}

		return statisticsList;
	}

	private Leaderboard ParseLeaderboard(ref Dictionary<string, object> leaderboardData)
	{
		if (leaderboardData != null)
		{
			Array leaderboardArray = leaderboardData["leaderboard"] as Array;
			string leaderboardId = leaderboardData["leaderboardId"] as string;

			List<LeaderboardEntry> leaderboardEntriesList = new List<LeaderboardEntry>();
			int rank = 0;
			string nickname;
			int ms = 0;
			double time = 0.0;

			for (int i = 0; i < leaderboardArray.Length; i++)
			{
				Dictionary<string, object> leaderboardEntry = leaderboardArray.GetValue(i) as Dictionary<string, object>;
				Dictionary<string, object> data = leaderboardEntry["data"] as Dictionary<string, object>;

				rank = int.Parse(leaderboardEntry["rank"].ToString());
				nickname = data["nickname"] as string;
				ms = int.Parse(leaderboardEntry["score"].ToString());
				time = (double)ms / 1000.0;

				leaderboardEntriesList.Add(new LeaderboardEntry(nickname, rank, time));
			}

			Leaderboard leaderboard = new Leaderboard(leaderboardId, leaderboardEntriesList);
			return leaderboard;
		}

		return null;
	}

	private List<LevelData> ParseLevelData(ref Dictionary<string, object> entityData)
	{
		if (entityData != null)
		{
			Array entityArray = entityData["entityList"] as Array;

			List<LevelData> levelData = new List<LevelData>();
			string entityType;
			string entityID;
			int index = 0;

			for (int i = 0; i < entityArray.Length; i++)
			{
				Dictionary<string, object> entityRoot = entityArray.GetValue(i) as Dictionary<string, object>;
				Dictionary<string, object> data = entityRoot["data"] as Dictionary<string, object>;
				Dictionary<string, object> level = data["level"] as Dictionary<string, object>;

				entityType = entityRoot["entityType"] as string;
				entityID = entityRoot["entityId"] as string;
				index = int.Parse(data["levelIndex"].ToString());

				levelData.Add(new LevelData(entityType, entityID, index, ref level));
			}

			return levelData;
		}

		return null;
	}

	private List<Achievement> ParseAchievements(ref Array achievementsArray)
	{
		List<Achievement> achievementsList = new List<Achievement>();
		string id;
		string title;
		string description;
		string status;

		for (int i = 0; i < achievementsArray.Length; i++)
		{
			Dictionary<string, object> achievement = achievementsArray.GetValue(i) as Dictionary<string, object>;

			id = achievement["id"] as string;
			title = achievement["title"] as string;
			description = achievement["description"] as string;
			status = achievement["status"] as string;
			achievementsList.Add(new Achievement(id, title, description, status));
		}

		return achievementsList;
	}

	private UserProgress ParseUserProgress(ref Dictionary<string, object> userEntity)
	{
		// Parse the user progress entity
		Dictionary<string, object> data = userEntity["data"] as Dictionary<string, object>;
		string entityID = userEntity["entityId"] as string;
		string entityType = userEntity["entityType"] as string;
		UserProgress userProgress = new UserProgress(entityID, entityType);

		userProgress.LevelOneCompleted = bool.Parse(data["levelOneCompleted"].ToString());
		userProgress.LevelTwoCompleted = bool.Parse(data["levelTwoCompleted"].ToString());
		userProgress.LevelThreeCompleted = bool.Parse(data["levelThreeCompleted"].ToString());
		userProgress.LevelBossCompleted = bool.Parse(data["levelBossCompleted"].ToString());

		return userProgress;
	}

	private void ParseIdentities(ref Dictionary<string, object> identitiesData)
	{
		m_IdentityTypesList.Clear();

		foreach (string key in identitiesData.Keys)
			m_IdentityTypesList.Add(key);
	}

	private void OutputLog(string output)
	{
		if(m_EnabledLogging)
			GD.Print(output);
	}

	private string ExtractStatusMessage(string errorJson)
	{
		Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(errorJson);
		string statusMessage = response["status_message"] as string;
		return statusMessage;
	}
}
