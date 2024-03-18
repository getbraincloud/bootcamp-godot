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

	private delegate void ErrorDelegate<T1>(T1 a);

	private BrainCloudWrapper m_BrainCloud;
	private bool m_EnabledLogging = true;
	private string m_Username;
	private LeaderboardsManager m_LeaderboardsManager = new LeaderboardsManager();

	public bool EnableLogging
	{
		get { return m_EnabledLogging; }
		set { m_EnabledLogging = value; }
	}

	public LeaderboardsManager GetLeaderboardsManager
	{
		get { return m_LeaderboardsManager; }
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
			HandleAuthenticationSuccess(responseData, authenticationRequestCompleted);
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
			HandleAuthenticationSuccess(responseData, authenticationRequestCompleted);
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
			HandleAuthenticationSuccess(responseData, authenticationRequestCompleted);
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
			HandleAuthenticationSuccess(responseData, authenticationRequestCompleted);
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

	private void HandleAuthenticationSuccess(string responseData, AuthenticationRequestCompleted authenticationRequestCompleted)
	{
		OutputLog("Authentication successful: " + responseData);

		// Read the player name from the response data
		Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
		Dictionary<string, object> data = response["data"] as Dictionary<string, object>;
		m_Username = data["playerName"] as string;
		
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
