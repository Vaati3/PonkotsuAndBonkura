extends Node2D
class_name BaseCharacter

var gridSize:int = 50

var otherRef:BaseCharacter

var postion3D:Vector3 = Vector3.ZERO
var controllerId:int

var speed:float = 100

func Possess(playerId:int, other:BaseCharacter):
	controllerId = playerId
	otherRef = other
	UpdateVisibility()

@rpc("any_peer", "call_local")
func UpdatePosition(direction:Vector3):
	postion3D += direction
	UpdateVisibility()

func UpdateVisibility():
	if GameManager.player.id != controllerId:
		return
	if CanSee(otherRef.postion3D):
		otherRef.visible = true
	else:
		otherRef.visible = false

func CanSee(position:Vector3) -> bool:
	return false
