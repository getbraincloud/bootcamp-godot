// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public enum FadeType
{
    None = -1,
    Out,
    In
};

public class Fader
{
    public delegate void OnFadeHasCompleted(Fader fader);

    private OnFadeHasCompleted m_OnFadeHasCompleted;
    private float m_Alpha;
    private double m_Elapsed = 0.0f;
    private double m_Duration = 0.0f;
    private double m_Delay = 0.0f;
    private FadeType m_Type = FadeType.None;

    // Update is called once per frame
    public void Update(double delta)
    {
        if (m_Type != FadeType.None)
        {
            if (m_Delay > 0.0)
            {
                m_Delay -= delta;
                if (m_Delay <= 0.0)
                    m_Delay = 0.0;
                else
                    return;
            }

            if (m_Duration > 0.0)
            {
                m_Elapsed += delta;
                if (m_Elapsed >= m_Duration)
                    m_Elapsed = m_Duration;

                m_Alpha = (float)((m_Type == FadeType.Out) ? (1.0 - (m_Elapsed / m_Duration)) : (m_Elapsed / m_Duration));

                if (m_Elapsed == m_Duration && m_OnFadeHasCompleted != null)
                    m_OnFadeHasCompleted(this);
            }
        }
    }

    public void StartFade(FadeType fadeType, double duration, OnFadeHasCompleted onFadeHasCompleted = null)
    {
        StartFade(fadeType, duration, 0.0, onFadeHasCompleted);
    }

    public void StartFade(FadeType fadeType, double duration, double delay, OnFadeHasCompleted onFadeHasCompleted = null)
    {
        m_OnFadeHasCompleted = onFadeHasCompleted;
        m_Type = fadeType;
        m_Alpha = m_Type == FadeType.Out ? 1.0f : 0.0f;
        m_Duration = duration;
        m_Elapsed = 0.0;
        m_Delay = delay;
    }

    public void Reset(float alpha)
    {
        m_Type = FadeType.None;
        m_Alpha = alpha;
        m_Duration = 0.0;
        m_Elapsed = 0.0;
        m_Delay = 0.0;
    }

    public bool IsFading()
    {
        return m_Duration > 0.0 && m_Type != FadeType.None;
    }

    public bool IsFadingIn()
    {
        return IsFading() && m_Type == FadeType.In;
    }

    public bool IsFadingOut()
    {
        return IsFading() && m_Type == FadeType.Out;
    }

    public bool IsDelayed()
    {
        return m_Delay > 0.0 && IsFading();
    }

    public float Alpha
    {
        get { return m_Alpha; }
    }
}
