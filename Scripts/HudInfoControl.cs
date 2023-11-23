// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class HudInfoControl : Control
{
	[Export] private Label _Text = null;

	public delegate void HudInfoControlHoldCompleted(HudInfoControl hudInfoControl);
	
	private Slider m_Slider = new Slider();
	private double m_DisplayMin;
	private HudInfoControlHoldCompleted m_HudInfoControlHoldCompleted;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		m_Slider.Update(delta);
		this.Position = m_Slider.Current;

		if (m_DisplayMin > 0.0)
		{
			m_DisplayMin -= delta;
			if (m_DisplayMin < 0.0)
			{
				m_DisplayMin = 0.0;

				if (m_HudInfoControlHoldCompleted != null)
					m_HudInfoControlHoldCompleted(this);
			}
		}
	}

	public void Init(LeaderboardEntry leaderboardEntry, Vector2 position, HudInfoControlHoldCompleted hudInfoControlHoldCompleted = null)
	{
		Init("High Score #" + leaderboardEntry.Rank.ToString() + ": " + TimeSpan.FromSeconds(leaderboardEntry.Time).ToString(@"mm\:ss"), position, hudInfoControlHoldCompleted);
	}

	public void Init(string overrideText, Vector2 position, HudInfoControlHoldCompleted hudInfoControlHoldCompleted = null)
	{
		Init(overrideText, position, 0.0, hudInfoControlHoldCompleted);
	}

	public void Init(string overrideText, Vector2 position, double displayMin, HudInfoControlHoldCompleted hudInfoControlHoldCompleted = null)
	{
		this.Visible = false;

		_Text.Text = overrideText;
		this.Position = position;

		m_DisplayMin = displayMin;
		m_HudInfoControlHoldCompleted = hudInfoControlHoldCompleted;
	}

	public void SetText(string text)
	{
		_Text.Text = text;
	}

	public void MoveTo(Vector2 target)
	{
		this.Visible = true;

		Vector2 start = this.Position;
		float distance = start.DistanceTo(target);
		float time = distance / Constants.kHudHighScoreMovementSpeed;
		m_Slider.StartSlide(start, target, (double)time);
	}

	public bool IsMoving()
	{
		return m_Slider.IsSliding();
	}

	public bool CanPush()
	{
		return m_DisplayMin == 0.0;
	}
}
