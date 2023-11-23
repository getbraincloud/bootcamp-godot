// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class AchievementsManager : Node
{
	private List<Achievement> m_Achievements;

	public Achievement GetAchievementByID(string id)
	{
		if (m_Achievements != null)
			for (int i = 0; i < m_Achievements.Count; i++)
				if (m_Achievements[i].ID == id)
					return m_Achievements[i];
		return null;
	}

	public Achievement GetAchievementAtIndex(int index)
	{
		if (m_Achievements != null)
			if (index >= 0 && index < GetCount())
				return m_Achievements[index];
		return null;
	}

	public int GetCount()
	{
		return m_Achievements.Count;
	}

	public void SetAchievements(ref List<Achievement> achievements)
	{
		m_Achievements = achievements;
	}
}
