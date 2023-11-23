// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;
using System.Collections.Generic;

public partial class DialogManager : Control
{
	public static DialogManager sharedInstance;

	private readonly PackedScene m_ConnectingDialogPrefab = ResourceLoader.Load("res://Prefabs/ConnectingDialog.tscn") as PackedScene;
	private readonly PackedScene m_EmailLoginDialogPrefab = ResourceLoader.Load("res://Prefabs/EmailLoginDialog.tscn") as PackedScene;
	private readonly PackedScene m_EmailLoginAnonymousDialogPrefab = ResourceLoader.Load("res://Prefabs/EmailLoginAnonymousDialog.tscn") as PackedScene;
	private readonly PackedScene m_EmailLoginTwitchDialogPrefab = ResourceLoader.Load("res://Prefabs/EmailLoginTwitchDialog.tscn") as PackedScene;
	private readonly PackedScene m_UnviersalLoginDialogPrefab = ResourceLoader.Load("res://Prefabs/UniversalLoginDialog.tscn") as PackedScene;
	private readonly PackedScene m_MainMenuDialogPrefab = ResourceLoader.Load("res://Prefabs/MainMenuDialog.tscn") as PackedScene;
	private readonly PackedScene m_BrainCloudDialogPrefab = ResourceLoader.Load("res://Prefabs/BrainCloudDialog.tscn") as PackedScene;
	private readonly PackedScene m_InGameBrainCloudDialogPrefab = ResourceLoader.Load("res://Prefabs/InGameBrainCloudDialog.tscn") as PackedScene;
	private readonly PackedScene m_LevelSelectDialogPrefab = ResourceLoader.Load("res://Prefabs/LevelSelectDialog.tscn") as PackedScene;
	private readonly PackedScene m_LeaderobardsDialogPrefab = ResourceLoader.Load("res://Prefabs/LeaderboardsDialog.tscn") as PackedScene;
	private readonly PackedScene m_StatisticsDialogPrefab = ResourceLoader.Load("res://Prefabs/StatisticsDialog.tscn") as PackedScene;
	private readonly PackedScene m_AchieveentsDialogPrefab = ResourceLoader.Load("res://Prefabs/AchievementsDialog.tscn") as PackedScene;
	private readonly PackedScene m_PlayAgainDialogPrefab = ResourceLoader.Load("res://Prefabs/PlayAgainDialog.tscn") as PackedScene;
	private readonly PackedScene m_PauseDialogPrefab = ResourceLoader.Load("res://Prefabs/PauseDialog.tscn") as PackedScene;
	private readonly PackedScene m_ChangeUsernameDialogPrefab = ResourceLoader.Load("res://Prefabs/ChangeUsernameDialog.tscn") as PackedScene;
	private readonly PackedScene m_PostScoreDialogPrefab = ResourceLoader.Load("res://Prefabs/PostScoreDialog.tscn") as PackedScene;
	private readonly PackedScene m_AttachEmailDialogPrefab = ResourceLoader.Load("res://Prefabs/AttachEmailDialog.tscn") as PackedScene;
	private readonly PackedScene m_ErrorDialogPrefab = ResourceLoader.Load("res://Prefabs/ErrorDialog.tscn") as PackedScene;

	private Stack<Dialog> m_ActiveDialogs = new Stack<Dialog>();
	private ConnectingDialog m_ConnectingDialog;
	private EmailLoginDialog m_EmailLoginDialog;
	private UniversalLoginDialog m_UniversalLoginDialog;
	private MainMenuDialog m_MainMenuDialog;
	private BrainCloudDialog m_BrainCloudDialog;
	private InGameBrainCloudDialog m_InGameBrainCloudDialog;
	private LevelSelectDialog m_LevelSelectDialog;
	private LeaderboardsDialog m_LeaderboardsDialog;
	private StatisticsDialog m_StatisticsDialog;
	private AchievementsDialog m_AchievementsDialog;
	private PlayAgainDialog m_PlayAgainDialog;
	private PauseDialog m_PauseDialog;
	private ChangeUsernameDialog m_ChangeUsernameDialog;
	private PostScoreDialog m_PostScoreDialog;
	private AttachEmailDialog m_AttachEmailDialog;
	private ErrorDialog m_ErrorDialog;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sharedInstance = this;

		m_ConnectingDialog = InstantiateDialog<ConnectingDialog>(m_ConnectingDialogPrefab);

		m_EmailLoginDialog = InstantiateDialog<EmailLoginDialog>(m_EmailLoginDialogPrefab);
		//m_EmailLoginDialog = InstantiateDialog<EmailLoginDialog>(m_EmailLoginAnonymousDialogPrefab);
		//m_EmailLoginDialog = InstantiateDialog<EmailLoginDialog>(m_EmailLoginTwitchDialogPrefab);
		
		m_UniversalLoginDialog = InstantiateDialog<UniversalLoginDialog>(m_UnviersalLoginDialogPrefab);
		m_MainMenuDialog = InstantiateDialog<MainMenuDialog>(m_MainMenuDialogPrefab);
		m_BrainCloudDialog = InstantiateDialog<BrainCloudDialog>(m_BrainCloudDialogPrefab);
		m_InGameBrainCloudDialog = InstantiateDialog<InGameBrainCloudDialog>(m_InGameBrainCloudDialogPrefab);
		m_LevelSelectDialog = InstantiateDialog<LevelSelectDialog>(m_LevelSelectDialogPrefab);

		m_LeaderboardsDialog = InstantiateDialog<LeaderboardsDialog>(m_LeaderobardsDialogPrefab);
		m_StatisticsDialog = InstantiateDialog<StatisticsDialog>(m_StatisticsDialogPrefab);
		m_AchievementsDialog = InstantiateDialog<AchievementsDialog>(m_AchieveentsDialogPrefab);

		m_PlayAgainDialog = InstantiateDialog<PlayAgainDialog>(m_PlayAgainDialogPrefab);
		m_PauseDialog = InstantiateDialog<PauseDialog>(m_PauseDialogPrefab);
		m_ChangeUsernameDialog = InstantiateDialog<ChangeUsernameDialog>(m_ChangeUsernameDialogPrefab);
		m_PostScoreDialog = InstantiateDialog<PostScoreDialog>(m_PostScoreDialogPrefab);
		m_AttachEmailDialog = InstantiateDialog<AttachEmailDialog>(m_AttachEmailDialogPrefab);
		m_ErrorDialog = InstantiateDialog<ErrorDialog>(m_ErrorDialogPrefab);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Escape"))
		{
			OnEscape();
		}
	}

	public void ShowConnectingDialog()
	{
		if (!m_ConnectingDialog.IsShowing())
			Show(m_ConnectingDialog);
	}

	public void HideConnectingDialog()
	{
		if (m_ConnectingDialog.IsShowing())
			m_ConnectingDialog.Hide();
	}

	public void ShowMainMenuDialog()
	{
		if (!m_MainMenuDialog.IsShowing())
			Show(m_MainMenuDialog);
	}

	public void ShowPauseDialog()
	{
		if (!m_PauseDialog.IsShowing())
			Show(m_PauseDialog);
	}

	public void ShowPlayAgainDialog()
	{
		if (!m_PlayAgainDialog.IsShowing())
			Show(m_PlayAgainDialog);
	}

	public void ShowLeaderboardsDialog()
	{
		if (!m_LeaderboardsDialog.IsShowing())
			Show(m_LeaderboardsDialog);
	}

	public void ShowStatisticsDialog()
	{
		if (!m_StatisticsDialog.IsShowing())
			Show(m_StatisticsDialog);
	}

	public void ShowAchievementsDialog()
	{
		if (!m_AchievementsDialog.IsShowing())
			Show(m_AchievementsDialog);
	}

	public void ShowUniversalLoginDialog(Network.AuthenticationRequestCompleted authenticationRequestCompleted = null, Network.AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		if (!m_UniversalLoginDialog.IsShowing())
		{
			m_UniversalLoginDialog.Set(authenticationRequestCompleted, authenticationRequestFailed);
			Show(m_UniversalLoginDialog);
		}
	}

	public void ShowEmailLoginDialog(Network.AuthenticationRequestCompleted authenticationRequestCompleted = null, Network.AuthenticationRequestFailed authenticationRequestFailed = null)
	{
		if (!m_EmailLoginDialog.IsShowing())
		{
			m_EmailLoginDialog.Set(authenticationRequestCompleted, authenticationRequestFailed);
			Show(m_EmailLoginDialog);
		}
	}

	public void ShowPostScoreDialog(double time)
	{
		if (!m_PostScoreDialog.IsShowing())
		{
			m_PostScoreDialog.Set(time);
			Show(m_PostScoreDialog);
		}
	}

	public void ShowChangeUsernameDialog()
	{
		if (!m_ChangeUsernameDialog.IsShowing())
			Show(m_ChangeUsernameDialog);
	}

	public void ShowBrainCloudDialog(bool inGameDialog = false)
	{
		if (inGameDialog)
		{
			if (!m_InGameBrainCloudDialog.IsShowing())
				Show(m_InGameBrainCloudDialog);
		}
		else
		{
			if (!m_BrainCloudDialog.IsShowing())
				Show(m_BrainCloudDialog);
		}
	}

	public void ShowAttachEmailDialog()
	{
		if (!m_AttachEmailDialog.IsShowing())
		{
			Show(m_AttachEmailDialog);
		}
	}

	public void ShowLevelSelectDialog()
	{
		if (!m_LevelSelectDialog.IsShowing())
			Show(m_LevelSelectDialog);
	}

	public void ShowErrorDialog(string message)
	{
		if (!m_ErrorDialog.IsShowing())
		{
			m_ErrorDialog.Set(message);
			Show(m_ErrorDialog);
		}
	}

	public bool AreAnyDialogsShowing()
	{
		return m_ActiveDialogs.Count > 0;
	}

	public int DialogStackCount()
	{
		return m_ActiveDialogs.Count;
	}

	public void OnEscape()
	{
		if (!AreAnyDialogsShowing())
		{
			if (Game.sharedInstance.IsGameOver() || Game.sharedInstance.IsGameWon())
				ShowPlayAgainDialog();
			else
				ShowPauseDialog();
		}
		else
		{
			if(m_ActiveDialogs.Peek() != m_MainMenuDialog)
				m_ActiveDialogs.Peek().Hide();
		}
	}

	private void Show(Dialog dialog)
	{
		if (m_ActiveDialogs.Count == 0)
			Game.sharedInstance.TimeScale = 0.0;
		else
			m_ActiveDialogs.Peek().Hide(false);

		m_ActiveDialogs.Push(dialog);

		dialog.Show();
	}

	private void OnDialogShown(Dialog dialog)
	{
	}

	private void OnDialogHidden(Dialog dialog)
	{
		m_ActiveDialogs.Pop();

		if (m_ActiveDialogs.Count == 0)
			Game.sharedInstance.TimeScale = 1.0;
		else
			m_ActiveDialogs.Peek().Show(false);
	}

	private Type InstantiateDialog<Type>(PackedScene prefab) where Type : Dialog
	{
		Type dialog = prefab.Instantiate<Type>();
		dialog.Hide(false);
		dialog.DialogShown = OnDialogShown;
		dialog.DialogHidden = OnDialogHidden;
		this.AddChild(dialog);
		return dialog;
	}
}
