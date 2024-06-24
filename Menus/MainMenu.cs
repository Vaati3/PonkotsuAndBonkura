using Godot;
using System;

public partial class MainMenu : Panel
{
	MultiplayerController controller;
	GameManager manager;

	Control menu;
	Control playMenu;
	Control joinMenu;
	Control namePopup;
	public override void _Ready()
	{
		controller = GetNode<MultiplayerController>("/root/MultiplayerController");
		manager = GetNode<GameManager>("/root/GameManager");
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
		Lobby lobby = GD.Load<PackedScene>("res://Menus/Lobby.tscn").Instantiate<Lobby>();
		GetTree().Root.AddChild(lobby);
		Visible = false;
	}

	//main menu
	public void _on_play_pressed()
	{
		menu.Visible = false;
		playMenu.Visible = true;
	}
	public void _on_play_solo_pressed()
	{
		manager.isAlone = true;
		OpenLobby();
	}
	public void _on_options_pressed()
	{
		GD.Print("Option button has been pressed");
	}
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	//play and option menu
	public void _on_host_pressed()
	{
		if (!controller.Host())
			GD.Print("host failed");
		playMenu.Visible = false;
		menu.Visible = true;
	}
	public void _on_join_pressed()
	{
		playMenu.Visible = false;
		joinMenu.Visible = true;
	}
	public void _on_back_pressed()
	{
		menu.Visible = playMenu.Visible;
		playMenu.Visible = joinMenu.Visible;
		joinMenu.Visible = false;
	}
	public void _on_confirm_join_pressed()
	{
		//string address = GetNode<TextEdit>("JoinMenu/TextEdit").Text;
		controller.Join("127.0.0.1");
		joinMenu.Visible = false;
		menu.Visible = true;
	}
	//name popup
	public void _on_confirm_name_pressed()
	{
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
