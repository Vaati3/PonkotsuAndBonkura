extends Panel
class_name MainMenu

func _ready():
	MultiplayerController.SetMenu(self)
	if not GameManager.Load():
		$Menu.visible = false
		$NamePopup.visible = true

func OpenLobby():
	var scene = load("res://Menus/Scenes/Lobby.tscn").instantiate()
	get_tree().root.add_child(scene)
	self.hide()

#main menu
func _on_host_pressed():
	MultiplayerController.Host()

func _on_join_pressed():
	$Menu.visible = false
	$JoinMenu.visible = true

func _on_options_pressed():
	pass # Replace with function body.

func _on_quit_pressed():
	get_tree().quit()

#join menu
func _on_back_pressed():
	$JoinMenu.visible = false
	$Menu.visible = true

func _on_confirm_join_pressed():
	MultiplayerController.Join("127.0.0.1")

#name menu
func _on_confirm_name_pressed():
	if not $NamePopup/NameTextbox.text.is_empty():
		GameManager.player.name = $NamePopup/NameTextbox.text
		GameManager.Save()
		$NamePopup.visible = false
		$Menu.visible = true
