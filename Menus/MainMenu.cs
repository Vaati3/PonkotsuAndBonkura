using Godot;
using System;

public partial class MainMenu : Panel
{
	MultiplayerController controller;
	GameManager manager;
	SoundManager soundManager;
	Control menu;
	Control playMenu;
	Control joinMenu;
	Control namePopup;
	public override void _Ready()
	{
		controller = GetNode<MultiplayerController>("/root/MultiplayerController");
		manager = GetNode<GameManager>("/root/GameManager");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		menu = GetNode<Control>("Menu");
		playMenu = GetNode<Control>("Play");
		joinMenu = GetNode<Control>("JoinMenu");
		namePopup = GetNode<Control>("NamePopup");
		if (!manager.Load())
		{
			menu.Visible = false;
			namePopup.Visible = true;
		}
		
		controller.HostCreated += OpenLobby;
	}

	public void OpenLobby()
	{
		controller.lobby = GD.Load<PackedScene>("res://Menus/Lobby.tscn").Instantiate<Lobby>();
		GetTree().Root.AddChild(controller.lobby);
		Visible = false;
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
		GD.Print("Option button has been pressed");
	}
	public void _on_quit_pressed()
	{
		soundManager.PlaySFX("button");
		GetTree().Quit();
	}
	//play and option menu
	public void _on_host_pressed()
	{
		soundManager.PlaySFX("button");
		if (!controller.Host())
			GD.Print("host failed");
		playMenu.Visible = false;
		menu.Visible = true;
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
		string address = GetNode<TextEdit>("JoinMenu/TextEdit").Text;//"127.0.0.1"
		controller.Join(address);
		joinMenu.Visible = false;
		menu.Visible = true;
	}
	//name popup
	public void _on_confirm_name_pressed()
	{
		soundManager.PlaySFX("button");
		string name = namePopup.GetNode<TextEdit>("NameTextbox").Text;
		if (name != "")
		{
			manager.player.name = name;
			manager.Save();
			namePopup.Visible = false;
			menu.Visible = true;
		}
	}
}
