// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class UniversalLoginDialog : Dialog
{
	[Export] private TextureButton _LoginButton;
	[Export] private LineEdit _UsernameLineEdit;
	[Export] private LineEdit _PasswordLineEdit;

	private Network.AuthenticationRequestCompleted m_AuthenticationRequestCompleted;
	private Network.AuthenticationRequestFailed m_AuthenticationRequestFailed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_LoginButton.Connect("pressed", new Callable(this, "OnLoginButtonClicked"));

		base._Ready();
	}

	public void Set(Network.AuthenticationRequestCompleted authenticationRequestCompleted, Network.AuthenticationRequestFailed authenticationRequestFailed)
	{
	   m_AuthenticationRequestCompleted = authenticationRequestCompleted;
	   m_AuthenticationRequestFailed = authenticationRequestFailed;
	}

	public void OnLoginButtonClicked()
	{
		Hide();
		DialogManager.sharedInstance.ShowConnectingDialog();
		GetNetwork.AuthenticateUniversal(_UsernameLineEdit.Text, _PasswordLineEdit.Text, m_AuthenticationRequestCompleted, m_AuthenticationRequestFailed);
	}

	protected override void OnClose()
	{
		// Dialog closed without logging in, show the main menu dialog
		DialogManager.sharedInstance.ShowMainMenuDialog();
	}
}
