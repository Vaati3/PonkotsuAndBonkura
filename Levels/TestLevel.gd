extends Node2D

func _ready():
	print($Ponkotsu)
	if GameManager.player.isPonkotsu:
		$Ponkotsu.Possess(GameManager.player.id, $Bonkura)
		$Bonkura.Possess(GameManager.otherPlayer.id, $Ponkotsu)
	else:
		$Bonkura.Possess(GameManager.player.id, $Ponkotsu)
		$Ponkotsu.Possess(GameManager.otherPlayer.id, $Bonkura)

func _process(delta):
	$PonkotsuPosDebug.text = "Ponkotsu pos: " + str($Ponkotsu.postion3D)
	$BonkuraPosDebug.text = "Bonkura pos: " + str($Bonkura.postion3D)
