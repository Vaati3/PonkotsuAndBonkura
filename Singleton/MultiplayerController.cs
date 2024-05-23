using Godot;
using System;

public partial class MultiplayerController : Node
{
	ENetMultiplayerPeer peer;
	GameManager gameManager;
	int port = 9001;

	[Signal]
	public delegate void HostCreatedEventHandler();

	public override void _Ready()
	{
		Multiplayer.PeerConnected += PlayerConnected;
		Multiplayer.PeerDisconnected += PlayerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;

		peer = new ENetMultiplayerPeer();
		gameManager = GetNode<GameManager>("/root/GameManager");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void UpdatePlayers(String playerString)
	{
		Player player = Player.FromString(playerString);
		GD.Print("Player " + player.id + " updated");	
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

//multiplayer signals
	public void PlayerConnected(long id)
	{
		GD.Print("Player " + id + " connected");
	}
	public void PlayerDisconnected(long id)
	{
		GD.Print("Player " + id + " disconnected");
	}
	public void ConnectedToServer()
	{
		GD.Print("Player connected to server");
		RpcId(1, nameof(UpdatePlayers), gameManager.player.ToString());
	}
	public void ConnectionFailed()
	{
		GD.Print("Connection failed");
	}
}
