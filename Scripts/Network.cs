// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;


public partial class Network : Node
{
	private bool m_EnabledLogging = true;

	public bool EnableLogging
	{
		get { return m_EnabledLogging; }
		set { m_EnabledLogging = value; }
	}

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

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
