// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


public partial class Network : Node
{
	public delegate void AuthenticationRequestCompleted();
	public delegate void AuthenticationRequestFailed();
	public delegate void BrainCloudLogOutCompleted();
	public delegate void BrainCloudLogOutFailed();

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

	public bool IsAuthenticated()
	{
		return m_BrainCloud.Client.Authenticated;
	}

	public bool HasAuthenticatedPreviously()
	{
		return m_BrainCloud.GetStoredProfileId() != "" && m_BrainCloud.GetStoredAnonymousId() != "";
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
			OutputLog("Log out error: user is not authenticated");

			if(brainCloudLogOutFailed != null)
				brainCloudLogOutFailed();

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
			OutputLog("Log out error");

			if(brainCloudLogOutFailed != null)
				brainCloudLogOutFailed();
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
			OutputLog("Reconnect authentication error");

			if (authenticationRequestFailed != null)
				authenticationRequestFailed();
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
			OutputLog("Anonymous authentication error");

			if (authenticationRequestFailed != null)
				authenticationRequestFailed();		
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
			OutputLog("Universal authentication error");

			if (authenticationRequestFailed != null)
				authenticationRequestFailed();		
		};

		// Make the BrainCloud request
		m_BrainCloud.AuthenticateUniversal(userID, password, true, successCallback, failureCallback);
	}

	private void HandleAuthenticationSuccess(string responseData, AuthenticationRequestCompleted authenticationRequestCompleted)
	{
		OutputLog("Authentication successful: " + responseData);

		if (authenticationRequestCompleted != null)
			authenticationRequestCompleted();
	}

	private void OutputLog(string output)
	{
		if(m_EnabledLogging)
			GD.Print(output);
	}
}
