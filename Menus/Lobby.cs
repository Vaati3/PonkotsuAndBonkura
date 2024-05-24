using Godot;
using System;

public partial class Lobby : Panel
{
	GameManager gameManager;

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
	}

	private void JoinUpdate()
	{
		Rpc(nameof(UpdateMenu));
		timer.QueueFree();
	}

    public override void _Process(double delta)
    {
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
		Map map = GD.Load<PackedScene>("res://Map/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.StartMap(mapName);
		Visible = false;
	} 

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void SwitchCharacter()
	{
		gameManager.player.characterType = gameManager.player.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		if (gameManager.otherPlayer != null)
			gameManager.otherPlayer.characterType = gameManager.otherPlayer.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		UpdateMenu();
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
