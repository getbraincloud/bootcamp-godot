// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System;

public class UserProgress
{
    string m_EntityID;
    string m_EntityType;
    bool m_LevelOneCompleted;
    bool m_LevelTwoCompleted;
    bool m_LevelThreeCompleted;
    bool m_LevelBossCompleted;

    public UserProgress()
    {
        m_EntityID = "";
        m_EntityType = "";
        m_LevelOneCompleted = false;
        m_LevelTwoCompleted = false;
        m_LevelThreeCompleted = false;
        m_LevelBossCompleted = false;
    }

    public UserProgress(string entityID, string entityType)
    {
        m_EntityID = entityID;
        m_EntityType = entityType;
        m_LevelOneCompleted = false;
        m_LevelTwoCompleted = false;
        m_LevelThreeCompleted = false;
        m_LevelBossCompleted = false;
    }

    public string EntityID
    {
        get { return m_EntityID; }
    }

    public string EntityType
    {
        get { return m_EntityType; }
    }

    public string JsonData
    {
        get
        {
            string jsonData = "{\"levelOneCompleted\":\"" + m_LevelOneCompleted.ToString() +
              "\",\"levelTwoCompleted\":\"" + m_LevelTwoCompleted.ToString() +
              "\",\"levelThreeCompleted\":\"" + m_LevelThreeCompleted.ToString() +
              "\",\"levelBossCompleted\":\"" + m_LevelBossCompleted.ToString() + "\"}";
            return jsonData;
        }
    }

    public bool LevelOneCompleted
    {
        get { return m_LevelOneCompleted; }
        set { m_LevelOneCompleted = value; }
    }

    public bool LevelTwoCompleted
    {
        get { return m_LevelTwoCompleted; }
        set { m_LevelTwoCompleted = value; }
    }

    public bool LevelThreeCompleted
    {
        get { return m_LevelThreeCompleted; }
        set { m_LevelThreeCompleted = value; }
    }

    public bool LevelBossCompleted
    {
        get { return m_LevelBossCompleted; }
        set { m_LevelBossCompleted = value; }
    }
}