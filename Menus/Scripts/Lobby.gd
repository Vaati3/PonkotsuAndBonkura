extends Panel
class_name Lobby

func _ready():
	if GameManager.player.id == 1:
		$SwitchCharacters.visible = true
		$StartLevel.visible = true

func _process(delta):
	UpdateCharacters()
	if GameManager.player.id == 1 and GameManager.otherPlayer:
		$StartLevel.disabled = false

func UpdateCharacters():
	if GameManager.player.isPonkotsu:
		$VBoxContainer/HBoxContainer/PlayerPonkotsu.text = GameManager.player.name
		if GameManager.otherPlayer:
			$VBoxContainer/HBoxContainer2/PlayerBonkura.text = GameManager.otherPlayer.name
	else:
		$VBoxContainer/HBoxContainer2/PlayerBonkura.text = GameManager.player.name
		if GameManager.otherPlayer:
			$VBoxContainer/HBoxContainer/PlayerPonkotsu.text = GameManager.otherPlayer.name

@rpc("any_peer", "call_local")
func StartLevel(mapName:String):
	GameManager.map.OpenMap(mapName)
	var scene = load("res://Level/Level.tscn").instantiate()
	get_tree().root.add_child(scene)
	self.hide()

@rpc("any_peer", "call_local")
func SwitchCharacters():
	GameManager.player.isPonkotsu = not GameManager.player.isPonkotsu
	if GameManager.otherPlayer:
		GameManager.otherPlayer.isPonkotsu = not GameManager.otherPlayer.isPonkotsu

func _on_switch_characters_pressed():
	SwitchCharacters.rpc()

func _on_start_level_pressed():
	StartLevel.rpc("testarea")
