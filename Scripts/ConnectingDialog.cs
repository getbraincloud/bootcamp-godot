// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class ConnectingDialog : Dialog
{
	[Export] private Label _Dots;
	
	private Timer timer = new Timer();
	private int m_DotsCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = new Timer();
		timer.Connect("timeout", new Callable(this,"OnTimerCallback"));
		timer.WaitTime = 0.5;
		AddChild(timer);
		timer.Start();

		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		

	}

	public void OnTimerCallback()
	{
		timer.Start();

		m_DotsCount++;

		if (m_DotsCount >= 4)
			m_DotsCount = 0;

		string dotsText = "";
		for (int i = 0; i < m_DotsCount; i++)
			dotsText += ".";

		_Dots.Text = dotsText;
	}
}
