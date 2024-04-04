extends Node2D
class_name Level

func _ready():
	$Ponkotsu.UpdatePosition(GameManager.map.ponkotsuSpawn)
	$Bonkura.UpdatePosition(GameManager.map.bonkuraSpawn)
	print($Ponkotsu.position3D)
	print($Bonkura.position3D)
	if GameManager.player.isPonkotsu:
		$Ponkotsu.Possess(GameManager.player.id, $Bonkura, $TileMap)
		#$Bonkura.Possess(GameManager.otherPlayer.id, $Ponkotsu)
	else:
		$Bonkura.Possess(GameManager.player.id, $Ponkotsu, $TileMap)
		#$Ponkotsu.Possess(GameManager.otherPlayer.id, $Bonkura)

func _process(delta):
	$PonkotsuPosDebug.text = "Ponkotsu pos: " + str($Ponkotsu.position3D)
	$BonkuraPosDebug.text = "Bonkura pos: " + str($Bonkura.position3D)
