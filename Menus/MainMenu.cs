using Godot;
using GodotSteam;
using System;

public partial class MainMenu : Panel
{
	MultiplayerController controller;
	GameManager manager;
	SoundManager soundManager;
	Control menu;
	Control playMenu;
	Control joinMenu;
	Control optionMenu;

	public override void _Ready()
	{
		controller = GetNode<MultiplayerController>("/root/MultiplayerController");
		manager = GetNode<GameManager>("/root/GameManager");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		menu = GetNode<Control>("Menu");
		playMenu = GetNode<Control>("Play");
		joinMenu = GetNode<Control>("JoinMenu");
		optionMenu = GetNode<Control>("OptionMenu");
		optionMenu.GetNode<Button>("Back").Pressed += _on_option_back_pressed;
		controller.OpenLobby += OpenLobby;
		controller.BackToMainMenu += BackToMainMenu;
		soundManager.PlayMusic("music");
	}

	public void OpenLobby()
	{
		controller.lobby = GD.Load<PackedScene>("res://Menus/Lobby.tscn").Instantiate<Lobby>();
		controller.lobby.local = controller.lobbyId == 0;
		GetTree().Root.AddChild(controller.lobby);
		BackToMainMenu();
		Visible = false;
	}

	public void BackToMainMenu()
	{
		playMenu.Visible = false;
		joinMenu.Visible = false;
		menu.Visible = true;
	}

	//main menu
	public void _on_play_pressed()
	{
		soundManager.PlaySFX("button");
		menu.Visible = false;
		playMenu.Visible = true;
	}
	public void _on_play_solo_pressed()
	{
		soundManager.PlaySFX("button");
		manager.isAlone = true;
		OpenLobby();
	}
	public void _on_options_pressed()
	{
		soundManager.PlaySFX("button");
		optionMenu.Visible = true;
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("button");
		GetTree().Quit();
	}

	//play menus
	public void _on_host_pressed()
	{
		soundManager.PlaySFX("button");
		if (!controller.Host())
		{
			GD.Print("host failed");
			BackToMainMenu();
		}
	}

	public void _on_host_steam_pressed()
	{
		soundManager.PlaySFX("button");
		controller.HostSteam();
	}

	public void _on_join_pressed()
	{
		soundManager.PlaySFX("button");
		playMenu.Visible = false;
		joinMenu.Visible = true;
	}
	public void _on_back_pressed()
	{
		soundManager.PlaySFX("button");
		menu.Visible = playMenu.Visible;
		playMenu.Visible = joinMenu.Visible;
		joinMenu.Visible = false;
	}
	public void _on_confirm_join_pressed()
	{
		soundManager.PlaySFX("button");
		string address = GetNode<TextEdit>("JoinMenu/TextEdit").Text;
		if (!controller.Join(address))
		{
			GD.Print("join Failed");
		}
		BackToMainMenu();

	}

	//options
	public void _on_option_back_pressed()
	{
		soundManager.PlaySFX("button");
		optionMenu.Visible = false;
		manager.Save();
	}
}
