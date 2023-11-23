// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using Godot.Collections;
using System;

public partial class BaseNode : Node2D
{
	private int m_Health = 1;
	private int m_AttackDamage = 1;
	private double m_FadeDelay;
	private double m_FadeTimer;
	private double m_FadeDuration;
	private string m_Type;

	public string Type
	{
		get { return m_Type; }
	}

	public Network GetNetwork
	{
		get { return GetNode<Network>("/root/Network"); }
	}

	protected void SetType(string type)
	{
		m_Type = type;
	}

	public void Enable()
	{
		this.Visible = true;
		this.SetDeferred("process_mode", (int)ProcessModeEnum.Inherit);
	}

	public void Disable()
	{
		this.Visible = false;
		this.SetDeferred("process_mode", (int)ProcessModeEnum.Disabled);
	}

	public bool IsEnabled()
	{
		return this.Visible;
	}

	protected void SetAlpha(float alpha)
	{
		Color color = this.Modulate;
		color.A = alpha;
		this.Modulate = color;
	}

	public bool ApplyDamage(int damage)
	{
		m_Health -= damage;

		if (m_Health <= 0)
		{
			m_Health = 0;
			return true;
		}

		return false;
	}

	public void SetHealth(int health)
	{
		m_Health = health;
	}

	public int GetHealth()
	{
		return m_Health;
	}

	public void SetAttackDamage(int attackDamange)
	{
		m_AttackDamage = attackDamange;
	}

	public int GetAttackDamage()
	{
		return m_AttackDamage;
	}

	public bool IsFading()
	{
		return m_FadeTimer > 0.0;
	}

	public void FadeOut(double fadeTime, double fadeDelay = 0.0)
	{
		m_FadeDelay = fadeDelay;
		m_FadeTimer = 0.0;
		m_FadeDuration = fadeTime;
	}

	protected void ResetFade()
	{
		m_FadeDelay = 0.0;
		m_FadeTimer = 0.0;
		m_FadeDuration = 0.0;
		SetAlpha(1.0f);
	}

	protected void HandleFade(double delta)
	{
		if (m_FadeDelay > 0.0)
		{
			m_FadeDelay -= delta;
			if (m_FadeDelay <= 0.0)
				m_FadeDelay = 0.0;
			else
				return;
		}

		if (m_FadeDuration > 0.0)
		{
			m_FadeTimer += delta;
			if (m_FadeTimer >= m_FadeDuration)
			{
				ResetFade();
				Disable();
				return;
			}

			SetAlpha(1.0f - (float)(m_FadeTimer / m_FadeDuration));
		}
	}
}
