// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System;

public class Statistic
{
    private string m_Name;
    private string m_Description;
    private int m_Value;
    private int m_Increment;

    public Statistic(string name, string description, int value)
    {
        m_Name = name;
        m_Description = description;
        m_Value = value;
        m_Increment = 0;
    }

    public void ApplyIncrement(int amount = 1)
    {
        m_Increment += amount;
        m_Value += amount;
    }

    public string Name
    {
        get { return m_Name; }
    }

    public string Description
    {
        get { return m_Description; }
    }

    public int Value
    {
        get { return m_Value; }
    }

    public int Increment
    {
        get { return m_Increment; }
    }
}