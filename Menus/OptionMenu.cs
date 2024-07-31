using Godot;
using System;

public partial class OptionMenu : Panel
{
	SoundManager soundManager;
	GameManager gameManager;
	Panel inputsPanel;
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		inputsPanel = GetNode<Panel>("InputsPanel");
	}

	public void _on_back_pressed()
	{
		soundManager.PlaySFX("button");
		Visible = false;
		gameManager.Save();
	}

	public void _on_inputs_button_pressed()
	{
		soundManager.PlaySFX("button");
		inputsPanel.Visible = true;
	}

	public void _on_back_inputs_pressed()
	{
		soundManager.PlaySFX("button");
		inputsPanel.Visible = false;
	}
}
