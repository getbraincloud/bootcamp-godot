// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


public partial class Network : Node
{
	public delegate void AuthenticationRequestCompleted();
	public delegate void AuthenticationRequestFailed();

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
	
	public bool IsAuthenticated()
	{
		return m_BrainCloud.Client.Authenticated;
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
