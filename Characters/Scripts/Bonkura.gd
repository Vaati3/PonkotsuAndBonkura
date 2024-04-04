extends BaseCharacter
class_name Bonkura

var isOnXAxis:bool = true
var gravity:float = 9.8

func _physics_process(delta):
	if GameManager.player.id == controllerId:
		var direction:Vector3 = Vector3.ZERO
		if Input.is_action_pressed("move_right"):
			direction.z += speed * delta
		if Input.is_action_pressed("move_left"):
			direction.z -= speed * delta
		if Input.is_action_pressed("move_down"):
			direction.y += speed * delta
		if Input.is_action_pressed("move_up"):
			direction.y -= speed * delta
		UpdatePosition.rpc(direction)
		position.x = position3D.z
		position.y = position3D.y

func CanSee(pos:Vector3) -> bool:
	var gridSize = GameManager.map.tileSize
	var xGrid = floor(position3D.x / gridSize)
	
	if pos.x >= xGrid * gridSize and pos.x <= xGrid * gridSize + gridSize:
		otherRef.position.x = pos.z
		otherRef.position.y = pos.y
		return true
	return false

func UpdateTileMap():
	var gridPos: Vector3i = Vector3i.ZERO
	gridPos.x = GameManager.map.GetCoordFromPos(position3D).x
	for y in range(GameManager.map.size.y):
		gridPos.z = 0
		for z in range(GameManager.map.size.z):
			SetTile(z, y, gridPos)
			gridPos.z += 1
		gridPos.y += 1
