extends Node2D
class_name BaseCharacter

var postion3D:Vector3 
var controllerId:int

var speed:float = 100

func Possess(playerId:int):
	controllerId = playerId

@rpc("any_peer", "call_local")
func UpdatePosition(direction:Vector3):
	postion3D += direction
	position.x = postion3D.x
	position.y = postion3D.z
