extends Panel
class_name Lobby

func _ready():
	if GameManager.player.id == 1:
		$SwitchCharacters.visible = true

func _process(delta):
	UpdateCharacters()

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
func SwitchCharacters():
	GameManager.player.isPonkotsu = not GameManager.player.isPonkotsu
	if GameManager.otherPlayer:
		GameManager.otherPlayer.isPonkotsu = not GameManager.otherPlayer.isPonkotsu

func _on_switch_characters_pressed():
	SwitchCharacters.rpc()
