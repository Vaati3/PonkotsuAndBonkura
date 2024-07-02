using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	GameManager manager;
	Map map;

	Control levelCompleted;
	Control pause;

	public string currentMap {get; private set;}
	public int curentMapNumber {get; private set;}

	public delegate MapButton GetMapCallback(int number);
	public GetMapCallback GetMap;

	MapButton nextMap;
	public override void _Ready()
	{
		manager = GetNode<GameManager>("/root/GameManager");
		if (GetParent() is Map map)
			this.map = map;

		levelCompleted = GetNode<Control>("LevelCompleted");
		pause = GetNode<Control>("Pause");
	}

	public void Init(string curentMap, int number)
	{
		this.currentMap = curentMap;
		curentMapNumber = number; 
	}

	public void MapCompleted()
	{
		if (Visible)
			return;
		Rpc(nameof(ShowLevelCompletedMenu));
	}

	public void Pause()
	{
		if (Visible)
			return;
		Rpc(nameof(ShowPauseMenu));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void ShowPauseMenu()
	{
		pause.Visible = true;
		Visible = true;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void ShowLevelCompletedMenu()
	{
		levelCompleted.Visible = true;
		Visible = true;

		nextMap = GetMap(curentMapNumber + 1);
		if (manager.player.progression <= curentMapNumber)
		{
			manager.player.progression = curentMapNumber + 1;
			manager.Save();
			nextMap.Visible = true;
		}
		levelCompleted.GetNode<Button>("VBoxContainer/NextLevel").Visible = nextMap != null;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Resume()
	{
		pause.Visible = false;
		Visible = false;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void BacktoLobby()
	{
		levelCompleted.Visible = false;
		pause.Visible = false;
		Visible = false;
		map.BacktoLobby();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadMap(string mapName, int mapNumber)
	{
		map.ClearMap();
		map.StartMap(mapName, mapNumber);
		levelCompleted.Visible = false;
		Visible = false;
	}

	public void _on_back_button_pressed()
	{
		Rpc(nameof(BacktoLobby));
	}

	public void _on_resume_pressed()
	{
		Rpc(nameof(Resume));
	}

	public void _on_reset_pressed()
	{
		Rpc(nameof(LoadMap), currentMap, curentMapNumber);
	}

	public void _on_next_level_pressed()
	{
		Rpc(nameof(LoadMap), nextMap.mapName, curentMapNumber + 1);
	}
}
