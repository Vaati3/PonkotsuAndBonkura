extends Node2D

func _ready():
	if GameManager.player.isPonkotsu:
		$Ponkotsu.Possess(GameManager.player.id)
		$Bonkura.Possess(GameManager.otherPlayer.id)
	else:
		$Bonkura.Possess(GameManager.player.id)
		$Ponkotsu.Possess(GameManager.otherPlayer.id)

