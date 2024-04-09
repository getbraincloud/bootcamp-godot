// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public partial class HUD : Node2D
{
	[Export] private Label _appVersion = null;
	[Export] private Label _brainCloudVersion = null;
	[Export] private Label _timerLabel = null;
	[Export] private Label _mainInformation = null;
	[Export] private Label _secondaryInformation = null;
	[Export] private Control _infoControlMask = null;

	private readonly PackedScene m_InfoControlPrefab = ResourceLoader.Load("res://Prefabs/HudInfoControl.tscn") as PackedScene;

	private List<HudInfoControl> m_InfoControls = new List<HudInfoControl>();
	private Fader m_InformationFader = new Fader();
	private bool m_PlayerHasAllTimeHighScore = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mainInformation.Visible = false;
		_secondaryInformation.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (m_InfoControls.Count > 0 && m_InfoControls[0].Position.Y == GetHighScoreLeavingPosition().Y)
		{
			m_InfoControls[0].QueueFree();
			m_InfoControls.RemoveAt(0);
			TryPushingNextHighScore();
		}

		if (_mainInformation.Visible && m_InformationFader.IsFading())
		{
			m_InformationFader.Update(delta);

			Color color = new Color(1.0f, 1.0f, 1.0f, m_InformationFader.Alpha);
			_mainInformation.Modulate = color;
			_secondaryInformation.Modulate = color;
		}
	}

	public void SetAppVersion(string version)
	{
		_appVersion.Text = "App version: " + version;
	}

	public void SetBrainCloudVersion(string version)
	{
		_brainCloudVersion.Text = "BC Client version: " + version;
	}

	public void SetElapsedTime(double elapsedTime)
	{
		_timerLabel.Text = "Time: " + TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
	}

	public void ShowGameOver()
	{
		ShowInformation("GAME OVER", "");
	}

	public void HideGameOver()
	{
		HideInformation(false);
	}

	public void ShowGameWon()
	{
		ShowInformation("YOU\'VE SURVIVED...", "JUST BARELY.");
	}

	public void HideGameWon()
	{
		HideInformation(false);
	}

	public void ShowLevel(int number, string information = "")
	{
		if(number == -1)
		   ShowInformation("SURVIVE", "As long as possible");
		else
		   ShowInformation("LEVEL " + number.ToString(), information);
	}

	public void HideLevel()
	{
		HideInformation();
	}

	public void Reset()
	{
		_timerLabel.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		_timerLabel.Text = "Time: 00:00";
		HideGameOver();

		for(int i = 0; i < m_InfoControls.Count; i++)
			m_InfoControls[0].QueueFree();

		m_InfoControls.Clear();
	}

	public void PushLeaderboardEntry(LeaderboardEntry leaderboardEntry)
	{
		HudInfoControl temp = m_InfoControlPrefab.Instantiate<HudInfoControl>();
		_infoControlMask.AddChild(temp);
		temp.Init(leaderboardEntry, GetHighScoreEnteringPosition());

		PushInfoControl(temp);
	}

	public void PushAchievement(Achievement achievement)
	{
		HudInfoControl temp = m_InfoControlPrefab.Instantiate<HudInfoControl>();
		_infoControlMask.AddChild(temp);
		temp.Init(achievement, GetHighScoreEnteringPosition(), OnInfoControlHoldCompleted);

		PushInfoControl(temp);
	}

	public void PushLevelGoal(string levelGoal)
	{
		HudInfoControl temp = m_InfoControlPrefab.Instantiate<HudInfoControl>();
		_infoControlMask.AddChild(temp);
		temp.Init(levelGoal, GetHighScoreEnteringPosition());

		PushInfoControl(temp);
	}

	public void SetPlayerHasAllTimeHighScore()
	{
		if (!m_PlayerHasAllTimeHighScore)
		{
		   m_PlayerHasAllTimeHighScore = true;
		   _timerLabel.Modulate = new Color(207.0f / 255.0f, 198.0f / 255.0f, 0.0f, 1.0f);
		   PushAllTimeHighScore();
		}
	}

	private void ShowInformation(string main, string secondary, bool fadeIn = true)
	{
		if (!_mainInformation.Visible)
		{
			_mainInformation.Visible = true;
			_mainInformation.Text = main;

			_secondaryInformation.Visible = true;
			_secondaryInformation.Text = secondary;

			if (fadeIn)
			{
				Color color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				_mainInformation.Modulate = color;
				_secondaryInformation.Modulate = color;

				m_InformationFader.StartFade(FadeType.In, 0.5f);
			}
			else
			{
				Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				_mainInformation.Modulate = color;
				_secondaryInformation.Modulate = color;
			}
		}
	}

	private void HideInformation(bool fadeOut = true)
	{
		if (fadeOut)
			m_InformationFader.StartFade(FadeType.Out, 0.5f, OnInformationFadeOutComplete);
		else
			OnInformationFadeOutComplete(m_InformationFader);
	}

	private void TryPushingNextHighScore()
	{
		if (m_InfoControls.Count >= 2)
		{
			if (!m_InfoControls[0].IsMoving() && m_InfoControls[0].CanPush())
			{
				m_InfoControls[0].MoveTo(GetHighScoreLeavingPosition());
				m_InfoControls[1].MoveTo(GetHighScorePosition());
			}
		}
	}

	private void PushAllTimeHighScore()
	{
		HudInfoControl temp = m_InfoControlPrefab.Instantiate<HudInfoControl>();
		_infoControlMask.AddChild(temp);
		temp.Init("NEW #1 HIGH SCORE!", GetHighScoreEnteringPosition());

		PushInfoControl(temp);
	}

	private void PushInfoControl(HudInfoControl infoControl)
	{
		m_InfoControls.Add(infoControl);

		if (m_InfoControls.Count == 1)  //If it's the first highscore push it
			m_InfoControls[0].MoveTo(GetHighScorePosition());
		else
			TryPushingNextHighScore();
	}

	private Vector2 GetHighScorePosition()
	{
		return Vector2.Zero;
	}

	private Vector2 GetHighScoreEnteringPosition()
	{
		float x = GetHighScorePosition().Y;
		float y = GetHighScorePosition().Y + Constants.kHudHeight + Constants.kHudSmallBuffer;
		return new Vector2(x, y);
	}

	private Vector2 GetHighScoreLeavingPosition()
	{
		float x = GetHighScorePosition().X;
		float y = GetHighScorePosition().Y - Constants.kHudHeight - Constants.kHudSmallBuffer;
		return new Vector2(x, y);
	}

	private void OnInformationFadeOutComplete(Fader fader)
	{
		if (fader == m_InformationFader)
		{
			_mainInformation.Visible = false;
			_secondaryInformation.Visible = false;
		}
	}

	private void OnInfoControlHoldCompleted(HudInfoControl hudInfoControl)
	{
		TryPushingNextHighScore();
	}
}
