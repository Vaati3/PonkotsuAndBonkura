using Godot;
using System;

public partial class Lobby : Panel
{
	GameManager gameManager;
	Map map;
	Timer timer;
	GridContainer grid;
	LevelButton selectedLevel = null;
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		if ( gameManager.isAlone || gameManager.player.id == 1)
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
		grid = GetNode<GridContainer>("GridContainer");
		CreateLevelButtons("res://Map/Maps/");

		map = GD.Load<PackedScene>("res://Map/Map.tscn").Instantiate<Map>();
		GetTree().Root.AddChild(map);
		map.Visible = false;
		map.UnloadMap += ReloadLobby;
		map.gameMenu.GetMap += GetMap;
	}

	private void CreateLevelButtons(string path)
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
				LevelButton levelButton = new LevelButton(fileName, n, LevelPressed);
				grid.AddChild(levelButton);
			} else {
				GD.Print("folder " + fileName);
				n--;
				//CreateLevelButtons(fileName);//recursive folder open
			}
			fileName = dir.GetNext();
			n++;
		}
	}

	private void LevelPressed(LevelButton levelButton)
	{
		if (selectedLevel != null)
			selectedLevel.Disabled = false;
		selectedLevel = levelButton;
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
		GetNode<Button>("StartMap").Disabled = gameManager.isAlone && gameManager.otherPlayer != null;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	private void StartMap(string mapName, int mapNumber)
	{
		map.StartMap(mapName, mapNumber);
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
	public string GetMap(int number)
	{
		if (number >= grid.GetChildCount())
			return null;
		if (grid.GetChild(number) is LevelButton button)
			return button.levelName;
		return null;
	}

	public void _on_switch_characters_pressed()
	{
		Rpc(nameof(SwitchCharacter));
	}
	public void _on_start_map_pressed()
	{
		if (selectedLevel != null)
			Rpc(nameof(StartMap), selectedLevel.levelName, selectedLevel.levelNumber);
		//Rpc(nameof(StartMap), "level-1");
	}

	public void _on_leave_pressed()
	{
		gameManager.isAlone = false;
		//to be completed button is not visible
	}
}
