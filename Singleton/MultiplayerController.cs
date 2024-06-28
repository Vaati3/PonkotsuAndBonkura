using Godot;
using System;
using System.Linq;

public partial class MultiplayerController : Node
{
	ENetMultiplayerPeer peer;
	GameManager gameManager;
	public Lobby lobby {get; set;}
	int port = 9001;


	[Signal] public delegate void HostCreatedEventHandler();

	public override void _Ready()
	{
		Multiplayer.PeerConnected += PlayerConnected;
		Multiplayer.PeerDisconnected += PlayerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		Multiplayer.ServerDisconnected += ServerDisconnected;

		peer = new ENetMultiplayerPeer();
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
		EmitSignal(nameof(HostCreated));
		return true;
	}

	public bool Join(string address)
	{
		peer.CreateClient(address, port);
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		gameManager.player.id = Multiplayer.GetUniqueId();
		return true;
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
		if (gameManager.otherPlayer != null)
			peer.DisconnectPeer((int)gameManager.otherPlayer.id);
		peer.Close();
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		gameManager.Clear();
		GetTree();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void KickPlayer(string reason)
	{
		if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer)
			return;
		lobby.HideGame();
		GetNode<MainMenu>("/root/MainMenu").Visible = true;
		peer.Close();
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		gameManager.Clear();
		AddChild(Popup.Open(reason));
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
		EmitSignal(nameof(HostCreated));
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
}
