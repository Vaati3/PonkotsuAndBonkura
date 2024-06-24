using Godot;
using System;

public partial class Lobby : Panel
{
	GameManager gameManager;
	Map map;
	Timer timer;
	GridContainer grid;
	string selectedMap = null;
	int selectedMapNumber = -1;
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");

		//delay to wait for serverplayer to update on other
        timer = new Timer
        {
            Autostart = true,
            WaitTime = 0.1,
            OneShot = true
        };
		timer.Timeout += JoinUpdate;
		AddChild(timer);
		grid = GetNode<GridContainer>("GridContainer");
		CreateMapButtons("res://Map/Maps/");

		map = GD.Load<PackedScene>("res://Map/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.Visible = false;
		map.UnloadMap += ReloadLobby;
		map.gameMenu.GetMap += GetMap;
	}

	private void CreateMapButtons(string path)
	{
		DirAccess dir = DirAccess.Open(path);

		if (dir == null)
			return;

		dir.ListDirBegin();
		string fileName = dir.GetNext();
		int n = 0;
		while (fileName != "")
		{
			if (!dir.CurrentIsDir())
			{
				MapButton mapButton = new MapButton(fileName, n, LevelPressed);
				grid.AddChild(mapButton);
			} else {
				GD.Print("folder " + fileName);
				n--;
				//CreateMapButtons(fileName);//recursive folder open
			}
			fileName = dir.GetNext();
			n++;
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void UpdateSelectedLevel(string name, int number)
	{
		if (selectedMapNumber >= 0)
		{
			if (grid.GetChild(selectedMapNumber) is MapButton oldButton)
				oldButton.Disabled = false;
		}
		if (gameManager.isAlone || gameManager.otherPlayer != null || gameManager.player.id != 1) {
			GetNode<Button>("Ready").Disabled = false;
		}
		if (grid.GetChild(number) is MapButton button)
			button.Disabled = true;
		selectedMap = name;
		selectedMapNumber = number;
	}

	private void LevelPressed(MapButton mapButton)
	{
		Rpc(nameof(UpdateSelectedLevel), mapButton.mapName, mapButton.mapNumber);
	}

	private void JoinUpdate()
	{
		Rpc(nameof(UpdateMenu));
		gameManager.UpdateServer += UpdateServer;
		timer.QueueFree();//use for start timer, free to be removed
	}

	private void UpdateServer()
	{
		if (selectedMapNumber >= 0)
			Rpc(nameof(UpdateSelectedLevel), selectedMap, selectedMapNumber);
	}

    private void UpdatePlayerInfo(Player player)
	{
		int playerNumber = player.id == 1 ? 1 : 2;
		GetNode<Label>("VBox/Player" + playerNumber + "/Name").Text = player.name;
		GetNode<Label>("VBox/Player" + playerNumber + "/Character").Text = player.characterType.ToString();
		GetNode<Label>("VBox/Player" + playerNumber + "/Ready").Text = player.isReady ? "Ready" : "Not Ready";
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	public void UpdateMenu()
	{
		UpdatePlayerInfo(gameManager.player);
		if (gameManager.otherPlayer != null)
			UpdatePlayerInfo(gameManager.otherPlayer);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void StartMap(string mapName, int mapNumber)
	{
		map.StartMap(mapName, mapNumber);
		Visible = false;
		map.Visible = true;
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void UpdateReady(bool isReady)
	{
		gameManager.otherPlayer.isReady = isReady;
		UpdatePlayerInfo(gameManager.otherPlayer);
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
		GetNode<Button>("Ready").Text = "Ready";
		gameManager.player.isReady = false;
		UpdatePlayerInfo(gameManager.player);
		if (gameManager.otherPlayer != null)
		{
			gameManager.otherPlayer.isReady = false;
			UpdatePlayerInfo(gameManager.otherPlayer);
		}
		if (grid.GetChild(selectedMapNumber) is MapButton button)
				button.Disabled = false;
		selectedMap = null;
		selectedMapNumber = -1;
	}

	public string GetMap(int number)
	{
		if (number >= grid.GetChildCount())
			return null;
		if (grid.GetChild(number) is MapButton button)
			return button.mapName;
		return null;
	}

	public void _on_switch_characters_pressed()
	{
		Rpc(nameof(SwitchCharacter));
	}


	public void ConfirmLeave()
	{
		GetNode<MainMenu>("/root/MainMenu").Visible = true;
		GetNode<MultiplayerController>("/root/MultiplayerController").Quit();
		HideGame();
	}

	public void HideGame()
	{
		map.BacktoLobby();
		Visible = false;
	}

	public void PlayerDisconnected()
	{
		GetNode<Label>("VBox/Player2/Name").Text = "";
		GetNode<Label>("VBox/Player2/Character").Text = "";
		GetNode<Label>("VBox/Player2/Ready").Text = "";
		Button button = GetNode<Button>("Ready");
		button.Text = "Ready";
		button.Disabled = true;
		gameManager.player.isReady = false;
		UpdatePlayerInfo(gameManager.player);
	}

	public void _on_ready_pressed()
	{
		gameManager.player.isReady = !gameManager.player.isReady;
		
		GetNode<Button>("Ready").Text = gameManager.player.isReady ? "Not\nReady" : "Ready";
		UpdatePlayerInfo(gameManager.player);
		Rpc(nameof(UpdateReady), gameManager.player.isReady);
		if (gameManager.isAlone || (gameManager.otherPlayer != null && gameManager.otherPlayer.isReady))
		{
			Rpc(nameof(StartMap), selectedMap, selectedMapNumber);
		}
	}

	public void _on_leave_pressed()
	{
		AddChild(Popup.Open("Are you sure you want to leave ?", ConfirmLeave));
		//to be completed button is not visible
	}
}
