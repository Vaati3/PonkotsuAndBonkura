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
	public ulong lobbyId { get; private set;} = 0;
	Godot.Collections.Array optionArray;

	[Signal] public delegate void OpenLobbyEventHandler();
	[Signal] public delegate void BackToMainMenuEventHandler();

	public override void _Ready()
	{
		peer = new ENetMultiplayerPeer();
		steamPeer = new SteamMultiplayerPeer();
		optionArray = new Godot.Collections.Array();
		gameManager = GetNode<GameManager>("/root/GameManager");

		Multiplayer.PeerConnected += PlayerConnected;
		Multiplayer.PeerDisconnected += PlayerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		Multiplayer.ServerDisconnected += ServerDisconnected;

		Steam.LobbyCreated += LobbyCreated;
		Steam.LobbyJoined += LobbyJoined;
		Steam.JoinRequested += JoinRequested;
		Steam.LobbyInvite += InviteRequested;
		Steam.LobbyChatUpdate += SteamLobbyUpdate;
		steamPeer.PeerConnected += SteamPeerConnected;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void UpdatePlayers(String playerString)
	{
		Player player = Player.FromString(playerString);
		gameManager.PlayerJoined(player, Multiplayer.IsServer());
		if (Multiplayer.IsServer())
			RpcId(player.id, nameof(UpdatePlayers), gameManager.player.ToString());
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

	//ENet
	public string GetIP()
	{
		return IP.ResolveHostname(OS.GetEnvironment("COMPUTERNAME"), Godot.IP.Type.Ipv4);
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

	public bool Join(string address)
	{
		if (peer.CreateClient(address, port) != Error.Ok)
			return false;
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		gameManager.player.id = Multiplayer.GetUniqueId();
		return true;
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
		if (reason != "")
			AddChild(Popup.Open(reason));
	}

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

	//Steam
	public void HostSteam()
	{
		Steam.CreateLobby(Steam.LobbyType.Public, 2);
	}

	public void LobbyCreated(long connect, ulong lobbyId)
	{
		if (connect != 1)
		{
			GD.Print("Lobby failed with error " + connect);
			EmitSignal(nameof(BackToMainMenu));
			return;
		}
		Error error = steamPeer.CreateHost(0, optionArray);
		if (error != Error.Ok)
		{
			GD.Print("Create Host failed with error " + error);
			return;
		}
		GD.Print("Lobby successfully created with id " + lobbyId);
		Steam.SetLobbyJoinable(lobbyId, true);
		Steam.SetLobbyData(lobbyId, "name", gameManager.player.name);
		Steam.SetLobbyData(lobbyId, "mode", "CoOP");
		Multiplayer.MultiplayerPeer = steamPeer;
		gameManager.player.id = 1;
		this.lobbyId = lobbyId;
		EmitSignal(nameof(OpenLobby));
	}

	public async void LobbyJoined(ulong lobbyId, long permissions, bool locked, long response)
	{
		if (response != 1)
		{
			GD.Print("Lobby join failed with error " + response);
			EmitSignal(nameof(BackToMainMenu));
			return;
		}
		if (locked)
		{
			GD.Print("Lobby is locked");
			EmitSignal(nameof(BackToMainMenu));
			return;
		}
		ulong owner = Steam.GetLobbyOwner(lobbyId);
		if (owner == Steam.GetSteamID())
			return;
		this.lobbyId = lobbyId;
		Error createrr = steamPeer.CreateClient(owner, 0, optionArray);
		GD.Print("create client " + createrr);
		Multiplayer.MultiplayerPeer = steamPeer;
		gameManager.player.id = (long)Steam.GetSteamID();
		await ToSignal(GetTree().CreateTimer(5), SceneTreeTimer.SignalName.Timeout);
		Error err = RpcId(1, nameof(UpdatePlayers), gameManager.player.ToString());
		GD.Print("test err " + err);
		EmitSignal(nameof(OpenLobby));
	}

	public void JoinRequested(ulong lobbyId, ulong steamId)
	{
		GD.Print("joining " + Steam.GetFriendPersonaName(steamId) + " game");
		Steam.JoinLobby(lobbyId);
	}

	public void InviteRequested(ulong inviter, ulong lobbyId, ulong game)
	{
		GD.Print("joining " + Steam.GetFriendPersonaName(inviter) + " game");
		Steam.JoinLobby(lobbyId);
	}

	public void SteamPeerConnected(long id)
	{
		GD.Print("steam peer connected id " + id + " name " + Steam.GetFriendPersonaName((ulong)id));
	}

	public void SteamLobbyUpdate(ulong lobbyId, long changedId, long makingChangeId, long chatState)
	{
		// GD.Print("log");
		// for (int i = 0; i < Steam.GetNumLobbyMembers(lobbyId); i++)
		// {
		// 	ulong id = Steam.GetLobbyMemberByIndex(lobbyId, i);
		// 	GD.Print("user id " + id + " name " + Steam.GetFriendPersonaName(id));
		// }
		// =if (chatState == (int)Steam.ChatMemberStateChange.Entered)
	}

	//check disconnection and steam callbacks
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