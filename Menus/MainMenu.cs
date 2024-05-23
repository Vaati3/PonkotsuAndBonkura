using Godot;
using System;

public partial class MainMenu : Panel
{
	MultiplayerController controller;
	GameManager manager;

	Control menu;
	Control joinMenu;
	Control namePopup;
	public override void _Ready()
	{
		controller = GetNode<MultiplayerController>("/root/MultiplayerController");
		manager = GetNode<GameManager>("/root/GameManager");
		menu = GetNode<Control>("Menu");
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
		GD.Print("IT JUST WORKS");
	}

	//main menu
	public void _on_host_pressed()
	{
		if (!controller.Host())
			GD.Print("host failed");
	}
	public void _on_join_pressed()
	{
		menu.Visible = false;
		joinMenu.Visible = true;
	}
	public void _on_options_pressed()
	{
		GD.Print("Option button has been pressed");
	}
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	//option menu 
	public void _on_back_pressed()
	{
		joinMenu.Visible = false;
		menu.Visible = true;
	}
	public void _on_confirm_join_pressed()
	{
		controller.Join("127.0.0.1");
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
