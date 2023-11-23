// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using Godot;
using System;

public enum DialogType
{
	Unknown = -1,
	Pause,
	Login,
	HighScores,
	PostScore
};

public enum DialogSize
{
	Unknown = -1,
	Big,
	Medium,
	Small,
	Narrow
};

public partial class Dialog : Control
{
	[Export] protected TextureButton _CloseButton;

	public delegate void DialogShownDelegate(Dialog dialog);
	public delegate void DialogHiddenDelegate(Dialog dialog);

	private DialogShownDelegate m_DialogShown;
	private DialogHiddenDelegate m_DialogHidden;

	public DialogShownDelegate DialogShown
	{
		get { return m_DialogShown; }
		set { m_DialogShown = value; }
	}

	public DialogHiddenDelegate DialogHidden
	{
		get { return m_DialogHidden; }
		set { m_DialogHidden = value; }
	}

	public Network GetNetwork
	{
		get { return GetNode<Network>("/root/Network"); }
	}

	public override void _Ready()
	{
		if(_CloseButton != null)
			_CloseButton.Connect("pressed", new Callable(this, "OnCloseButtonClicked"));
	}

	public void Show(bool triggerCallback = true)
	{
		this.Visible = true;
		this.ProcessMode = ProcessModeEnum.Inherit;

		OnShow();

		if (triggerCallback && m_DialogShown != null)
			m_DialogShown(this);
	}

	public void Hide(bool triggerCallback = true)
	{
		this.Visible = false;
		this.ProcessMode = ProcessModeEnum.Disabled;

		OnHide();

		if(triggerCallback && m_DialogHidden != null)
			m_DialogHidden(this);
	}

	public bool IsShowing()
	{
		return this.Visible;
	}

	public void OnCloseButtonClicked()
	{
		Hide();
		OnClose();
	}

	protected virtual void OnShow() {}
	protected virtual void OnHide() {}
	protected virtual void OnClose() {}
}
