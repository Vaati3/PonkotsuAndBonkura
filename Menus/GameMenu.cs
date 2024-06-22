using Godot;
using System;

public partial class GameMenu : CanvasLayer
{
	Map map;

	Control levelCompleted;
	Control pause;
	public override void _Ready()
	{
		if (GetParent() is Map map)
			this.map = map;

		levelCompleted = GetNode<Control>("LevelCompleted");
		pause = GetNode<Control>("Pause");
	}

	public void MapCompleted()
	{
		Rpc(nameof(ShowLevelCompletedMenu));
	}

	public void Pause()
	{
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
	public void ResetMap()
	{
		map.ClearMap();
		map.StartMap(map.currentMap);
		Resume();
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
		Rpc(nameof(ResetMap));
	}
}
