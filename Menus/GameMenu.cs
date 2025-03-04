using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	GameManager manager;
	SoundManager soundManager;
	Map map;

	Control levelCompleted;
	Control pause;
	OptionMenu optionMenu;

	public string currentMap {get; private set;}
	public int curentMapNumber {get; private set;}

	public delegate MapButton GetMapEventHandler(int number);
	public GetMapEventHandler GetMap;

	MapButton nextMap;
	public override void _Ready()
	{
		manager = GetNode<GameManager>("/root/GameManager");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		optionMenu = GetNode<OptionMenu>("Pause/OptionMenu");
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
		soundManager.PlaySFX("button", true);
		Rpc(nameof(BacktoLobby));
	}

	public void _on_resume_pressed()
	{
		soundManager.PlaySFX("button", true);
		Rpc(nameof(Resume));
	}

	public void _on_reset_pressed()
	{
		soundManager.PlaySFX("button", true);
		Rpc(nameof(LoadMap), currentMap, curentMapNumber);
	}

	public void _on_next_level_pressed()
	{
		soundManager.PlaySFX("button", true);
		Rpc(nameof(LoadMap), nextMap.mapName, curentMapNumber + 1);
	}

	public void _on_options_pressed()
	{
		soundManager.PlaySFX("button", true);
		optionMenu.Visible = true;
	}
}
