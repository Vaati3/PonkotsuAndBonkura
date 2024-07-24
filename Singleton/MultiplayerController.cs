using Godot;
using GodotSteam;
using System;
using System.Linq;

public partial class MultiplayerController : Node
{
	ENetMultiplayerPeer peer;
	GameManager gameManager;
	public Lobby lobby {get; set;}
	int port = 9001;

	//steam
	SteamMultiplayerPeer steamPeer;
	ulong lobbyId = 0;

	[Signal] public delegate void OpenLobbyEventHandler();
	[Signal] public delegate void BackToMainMenuEventHandler();

	public override void _Ready()
	{
		Multiplayer.PeerConnected += PlayerConnected;
		Multiplayer.PeerDisconnected += PlayerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		Multiplayer.ServerDisconnected += ServerDisconnected;

		Steam.LobbyCreated += LobbyCreated;
		Steam.LobbyJoined += LobbyJoined;

		peer = new ENetMultiplayerPeer();
		steamPeer = new SteamMultiplayerPeer();
		gameManager = GetNode<GameManager>("/root/GameManager");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void UpdatePlayers(String playerString)
	{
		Player player = Player.FromString(playerString);
		gameManager.PlayerJoined(player, Multiplayer.IsServer());
		if (Multiplayer.IsServer())
			RpcId(player.id, nameof(UpdatePlayers), gameManager.player.ToString());
	}

	public bool Host()
	{
		if (peer.CreateServer(port, 2) != Error.Ok)
			return false;
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		gameManager.player.id = 1;
		EmitSignal(nameof(OpenLobby));
		return true;
	}

	public void HostSteam()
	{
		Steam.CreateLobby(Steam.LobbyType.FriendsOnly, 2);
	}

	public void LobbyCreated(long connect, ulong lobbyId)
	{
		if (connect != 1)
		{
			GD.Print("Lobby failed with error " + connect);
			EmitSignal(nameof(BackToMainMenu));
			return;
		}
		this.lobbyId = lobbyId;
		Steam.SetLobbyData(lobbyId, "lobby", (string)Steam.GetPersonaName());
		steamPeer.CreateHost(0, new Godot.Collections.Array());
		Multiplayer.MultiplayerPeer = steamPeer;
		gameManager.player.id = 1;
		EmitSignal(nameof(OpenLobby));
		GD.Print("Lobby successfully created with id " + lobbyId);
	}

	public bool Join(string address)
	{
		if (peer.CreateClient(address, port) != Error.Ok)
			return false;
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		gameManager.player.id = Multiplayer.GetUniqueId();
		return true;
	}

	public void JoinSteam(ulong lobbyId)
	{
		Steam.JoinLobby(lobbyId);
	}

	public void LobbyJoined(ulong lobbyId, long permissions, bool locked, long response)
	{
		if (response != 1)
		{
			GD.Print("Lobby join failed with error " + response);
			EmitSignal(nameof(BackToMainMenu));
			return;
		}
		this.lobbyId = lobbyId;
		ulong id = Steam.GetLobbyOwner(lobbyId);
		if (id != Steam.GetSteamID())
		{
			steamPeer.CreateClient(id, 0, new Godot.Collections.Array());
			Multiplayer.MultiplayerPeer = steamPeer;
			gameManager.player.id = (long)id;
		}
	}

	public void Quit()
	{
		if (gameManager.player.id == 1)
			CloseServer();
		else
			KickPlayer("");
	}

	public void CloseServer()//can't rehost after but work if port is changed 
	{
		if (lobbyId != 0)
		{
			steamPeer.Close();
			lobbyId = 0;
		} else {
			if (gameManager.otherPlayer != null)
				peer.DisconnectPeer((int)gameManager.otherPlayer.id);
			peer.Close();
		}
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		gameManager.Clear();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void KickPlayer(string reason)
	{
		if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer)
			return;
		lobby.CloseLobby();
		GetNode<MainMenu>("/root/MainMenu").Visible = true;
		peer.Close();
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		gameManager.Clear();
		AddChild(Popup.Open(reason));
	}

	public string GetIP()
	{
		return IP.ResolveHostname(OS.GetEnvironment("COMPUTERNAME"), Godot.IP.Type.Ipv4);
	}

	//multiplayer signals
	public void PlayerConnected(long id)
	{
		GD.Print("Player " + id + " connected");
	}

	public void PlayerDisconnected(long id)
	{
		GD.Print("Player " + id + " disconnected");
		if (id == 1)
		{
			KickPlayer("Host Closed Server");
		} else {
			lobby.PlayerDisconnected();
			gameManager.Clear();
		}
	}

	public void ConnectedToServer()
	{
		GD.Print("Player connected to server");
		RpcId(1, nameof(UpdatePlayers), gameManager.player.ToString());
		EmitSignal(nameof(OpenLobby));
	}

	public void ServerDisconnected()
	{
		GD.Print("Server disconnected");
	}
	
	public void ConnectionFailed()
	{
		GD.Print("Connection failed");
	}

	//check disconnection
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
		{
			Quit();
		}
    }

    public override void _Process(double delta)
    {
        Steam.RunCallbacks();
    }
}
