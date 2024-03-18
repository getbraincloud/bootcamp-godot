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

	private delegate void ErrorDelegate<T1>(T1 a);

	private BrainCloudWrapper m_BrainCloud;
	private bool m_EnabledLogging = true;

	public bool EnableLogging
	{
		get { return m_EnabledLogging; }
		set { m_EnabledLogging = value; }
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

	private void HandleAuthenticationSuccess(string responseData, AuthenticationRequestCompleted authenticationRequestCompleted)
	{
		OutputLog("Authentication successful: " + responseData);

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
