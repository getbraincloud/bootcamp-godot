// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


public partial class Network : Node
{
	private BrainCloudWrapper m_BrainCloud;
	private bool m_EnabledLogging = true;

	public bool EnableLogging
	{
		get { return m_EnabledLogging; }
		set { m_EnabledLogging = value; }
	}

	public string BrainCloudClientVersion
	{
		get { return m_BrainCloud.Client.BrainCloudClientVersion; }
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
		return false;
	}

	private void OutputLog(string output)
	{
		if(m_EnabledLogging)
			GD.Print(output);
	}
}
