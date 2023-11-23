// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;

public class Achievement
{
    private string m_ID;
    private string m_Title;
    private string m_Description;
    private bool m_IsAwarded;

    public Achievement(string id, string title, string description, string status)
    {
        m_ID = id;
        m_Title = title;
        m_Description = description;
        m_IsAwarded = status == "AWARDED";
    }

    public void Award()
    {
        m_IsAwarded = true;
        Game.sharedInstance.GetNetwork.AwardAchievement(this);
    }

    public string GetStatusString()
    {
        if (m_IsAwarded)
            return "Earned";
        return "";
    }

    public string ID
    {
        get { return m_ID; }
    }

    public string Title
    {
        get { return m_Title; }
    }

    public string Description
    {
        get { return m_Description; }
    }

    public bool IsAwarded
    {
        get { return m_IsAwarded; }
    }
}
