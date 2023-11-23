using Godot;
using System;

public partial class LeaderboardsDialog : Dialog
{
	[Export] private Godot.Collections.Array<Container> _Containers = new Godot.Collections.Array<Container>();
	[Export] private TextureButton _SegmentedControlLeft;
	[Export] private TextureButton _SegmentedControlMiddle;
	[Export] private TextureButton _SegmentedControlRight;

	private readonly PackedScene m_LeaderboardControlPrefab = ResourceLoader.Load("res://Prefabs/LeaderboardControl.tscn") as PackedScene;

	private Godot.Collections.Array<LeaderboardControl> _LeaderboardsControl = new Godot.Collections.Array<LeaderboardControl>();

	private enum SegmentControlState
	{
		Main = 0,
		Daily,
		Country
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_SegmentedControlLeft.Connect("pressed", new Callable(this, "OnMainScoresClicked"));
		_SegmentedControlMiddle.Connect("pressed", new Callable(this, "OnDailyScoresClicked"));
		_SegmentedControlRight.Connect("pressed", new Callable(this, "OnCountryScoresClicked"));

		for (int i = 0; i < _Containers.Count; i++)
		{
			LeaderboardControl leaderboardControl = m_LeaderboardControlPrefab.Instantiate<LeaderboardControl>();
			leaderboardControl.Position = Vector2.Zero;
			_Containers[i].AddChild(leaderboardControl);
			_LeaderboardsControl.Add(leaderboardControl);
		}

		base._Ready();
	}

	protected override void OnShow()
	{
		SetSegmentControlState(SegmentControlState.Main);
		SetLeaderboardData(Constants.kBrainCloudMainLeaderboardID);
	}

	public void OnMainScoresClicked()
	{
		SetSegmentControlState(SegmentControlState.Main);
		SetLeaderboardData(Constants.kBrainCloudMainLeaderboardID);
	}

	public void OnDailyScoresClicked()
	{
		SetSegmentControlState(SegmentControlState.Daily);
		SetLeaderboardData(Constants.kBrainCloudDailyLeaderboardID);
	}

	public void OnCountryScoresClicked()
	{
		SetSegmentControlState(SegmentControlState.Country);
		SetLeaderboardData(Constants.kBrainCloudCountryLeaderboardID);
	}

	private void SetSegmentControlState(SegmentControlState segmentControlState)
	{
		string leftTexture = GetTextureFile(SegmentControlState.Main, segmentControlState == SegmentControlState.Main);
		string middleTexture = GetTextureFile(SegmentControlState.Daily, segmentControlState == SegmentControlState.Daily);
		string rightTexture = GetTextureFile(SegmentControlState.Country, segmentControlState == SegmentControlState.Country);

		_SegmentedControlLeft.TextureNormal = ResourceLoader.Load<Texture2D>(leftTexture);
		_SegmentedControlMiddle.TextureNormal = ResourceLoader.Load<Texture2D>(middleTexture);
		_SegmentedControlRight.TextureNormal = ResourceLoader.Load<Texture2D>(rightTexture);
	}

	private void SetLeaderboardData(string leaderboardId)
	{
		ResetLeaderboardData();

		Leaderboard leaderboard = GetNetwork.GetLeaderboardsManager.GetLeaderboardByName(leaderboardId);

		if (leaderboard != null)
		{
			for (int i = 0; i < leaderboard.GetCount(); i++)
			{
				LeaderboardEntry leaderboardEntry = leaderboard.GetLeaderboardEntryAtIndex(i);
				if (leaderboardEntry != null && i < _LeaderboardsControl.Count)
					_LeaderboardsControl[i].Set(leaderboardEntry);
			}
		}
	}

	private void ResetLeaderboardData()
	{
		foreach (LeaderboardControl leaderboardControl in _LeaderboardsControl)
			leaderboardControl.Reset();
	}

	private string GetTextureFile(SegmentControlState segmentControlState, bool isSelected)
	{
		string texture = "res://Textures/SegmentControl";

		switch (segmentControlState)
		{
			case SegmentControlState.Main:
				texture += "Left-";
				break;
			case SegmentControlState.Daily:
				texture += "Middle-";
				break;
			case SegmentControlState.Country:
				texture += "Right-";
				break;
		}

		texture += isSelected ? "Selected.png" : "Unselected.png";
		return texture;
	}
}
