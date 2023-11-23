// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public partial class AttachEmailDialog : Dialog
{
	[Export] private TextureButton _AttachButton;
	[Export] private LineEdit _EmailLineEdit;
	[Export] private LineEdit _PasswordLineEdit;

	private Network.AttachEmailIdentityCompleted m_AttachEmailIdentityCompleted;
	private Network.AttachEmailIdentityFailed m_AttachEmailIdentityFailed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AttachButton.Connect("pressed", new Callable(this, "OnAttachButtonClicked"));

		base._Ready();
	}

	public void Set(Network.AttachEmailIdentityCompleted attachEmailIdentityCompleted, Network.AttachEmailIdentityFailed attachEmailIdentityFailed)
	{
	   m_AttachEmailIdentityCompleted = attachEmailIdentityCompleted;
	   m_AttachEmailIdentityFailed = attachEmailIdentityFailed;
	}

	public void OnAttachButtonClicked()
	{
		Hide();
		DialogManager.sharedInstance.ShowConnectingDialog();
		GetNetwork.AttachEmailIdentity(_EmailLineEdit.Text, _PasswordLineEdit.Text, m_AttachEmailIdentityCompleted, m_AttachEmailIdentityFailed);
	}
}
