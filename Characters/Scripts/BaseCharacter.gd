extends Node2D
class_name BaseCharacter

var otherRef:BaseCharacter

var position3D:Vector3 = Vector3.ZERO
var controllerId:int

var speed:float = 100

var tileMap: TileMap

func Possess(playerId:int, other:BaseCharacter, tileMapRef: TileMap):
	controllerId = playerId
	otherRef = other
	tileMap = tileMapRef
	UpdateVisibility()
	UpdateTileMap()

@rpc("any_peer", "call_local")
func UpdatePosition(direction:Vector3):
	position3D += direction
	UpdateVisibility()

func UpdateVisibility():
	if GameManager.player.id != controllerId:
		return
	if CanSee(otherRef.position3D):
		otherRef.visible = true
	else:
		otherRef.visible = false

func CanSee(position:Vector3) -> bool:
	return false

func SetTile(x: int, y: int, gridPos: Vector3i):
	var tile = GameManager.map.GetTileV(gridPos)
	tileMap.set_cell(0, Vector2(x, y), 0, GameManager.map.GetTileValue(tile))

func UpdateTileMap():
	pass
