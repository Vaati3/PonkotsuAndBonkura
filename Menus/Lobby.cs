using Godot;
using System;

public partial class Lobby : Panel
{
	GameManager gameManager;
	Map map;
	Timer timer;
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		if (gameManager.player.id == 1)
			GetNode<Button>("StartMap").Visible = true;

		//delay to wait for serverplayer to update on other
        timer = new Timer
        {
            Autostart = true,
            WaitTime = 0.1,
            OneShot = true
        };
		timer.Timeout += JoinUpdate;
		AddChild(timer);

		map = GD.Load<PackedScene>("res://Map/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.Visible = false;
		map.UnloadMap += ReloadLobby;
	}

	private void JoinUpdate()
	{
		Rpc(nameof(UpdateMenu));
		timer.QueueFree();
	}

    private void UpdatePlayerInfo(Player player)
	{
		int playerNumber = player.id == 1 ? 1 : 2;
		GetNode<Label>("VBox/Player" + playerNumber + "/Name").Text = player.name;
		GetNode<Label>("VBox/Player" + playerNumber + "/Character").Text = player.characterType.ToString();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	public void UpdateMenu()
	{
		UpdatePlayerInfo(gameManager.player);
		if (gameManager.otherPlayer != null)
			UpdatePlayerInfo(gameManager.otherPlayer);
		GetNode<Button>("StartMap").Disabled = gameManager.player.id != 1 || gameManager.otherPlayer == null;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void StartMap(string mapName)
	{
		map.StartMap(mapName);
		Visible = false;
		map.Visible = true;
	} 

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void SwitchCharacter()
	{
		gameManager.player.characterType = gameManager.player.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		if (gameManager.otherPlayer != null)
			gameManager.otherPlayer.characterType = gameManager.otherPlayer.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		UpdateMenu();
	}

	public void ReloadLobby()
	{
		map.Visible = false;
		Visible = true;
	}
	public void _on_switch_characters_pressed()
	{
		Rpc(nameof(SwitchCharacter));
	}
	public void _on_start_map_pressed()
	{
		Rpc(nameof(StartMap), "testarea");
	}
}
