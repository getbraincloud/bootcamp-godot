// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class StatisticsManager : Node
{
	private List<Statistic> m_Statistics;


	public Statistic GetStatisticByName(string name)
	{
		if (m_Statistics != null)
			for (int i = 0; i < m_Statistics.Count; i++)
				if (m_Statistics[i].Name == name)
					return m_Statistics[i];
		return null;
	}

	public Statistic GetStatisticAtIndex(int index)
	{
		if (m_Statistics != null)
			if (index >= 0 && index < GetCount())
				return m_Statistics[index];
		return null;
	}

	public int GetCount()
	{
		return m_Statistics.Count;
	}

	public void SetStatistics(ref List<Statistic> statistics)
	{
		m_Statistics = statistics;
	}
}
