extends Node

var address:String = "127.0.0.1"#temporary
@export var port:int = 9001

var peer:ENetMultiplayerPeer

var menuRef:MainMenu

func _ready():
	multiplayer.peer_connected.connect(PlayerConnected)
	multiplayer.peer_disconnected.connect(PlayerDisconnected)
	multiplayer.connected_to_server.connect(ConnectedToServer)
	multiplayer.connection_failed.connect(ConnectionFailed)
	
	peer = ENetMultiplayerPeer.new()

func SetMenu(menu:MainMenu):
	menuRef = menu

func PlayerConnected(id:int):
	print("Player : " + str(id) + " connected")

func PlayerDisconnected(id:int):
	print("Player : " + str(id) + " disconnected")
	
func ConnectedToServer():
	print("Player connected to server")
	UpdatePlayers.rpc_id(1, GameManager.player)
	menuRef.OpenLobby()

func ConnectionFailed():
	print("Connection failed")

@rpc("any_peer")
func UpdatePlayers(player:Dictionary):
	if player.id != GameManager.player.id:
		GameManager.otherPlayer = player
	if multiplayer.is_server():
		GameManager.otherPlayer.isPonkotsu = not GameManager.player.isPonkotsu
		UpdatePlayers.rpc_id(player.id, GameManager.player)
	else:
		GameManager.player.isPonkotsu = not player.isPonkotsu
		print(GameManager.player)
		print(GameManager.otherPlayer)

func Host():
	if peer.create_server(port, 2) != OK:
		print("Host Failed")
		return
	peer.get_host().compress(ENetConnection.COMPRESS_RANGE_CODER)
	multiplayer.set_multiplayer_peer(peer)
	GameManager.player.id = multiplayer.get_unique_id()
	menuRef.OpenLobby()
	print("hosting")

func Join(hostAddress:String):
	peer.create_client(hostAddress, port)
	peer.get_host().compress(ENetConnection.COMPRESS_RANGE_CODER)
	multiplayer.set_multiplayer_peer(peer)
	GameManager.player.id = multiplayer.get_unique_id()
