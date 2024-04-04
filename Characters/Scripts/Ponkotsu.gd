extends BaseCharacter
class_name Ponkotsu

func _physics_process(delta):
	if GameManager.player.id == controllerId:
		var direction:Vector3 = Vector3.ZERO
		if Input.is_action_pressed("move_right"):
			direction.x += speed * delta
		if Input.is_action_pressed("move_left"):
			direction.x -= speed * delta
		if Input.is_action_pressed("move_down"):
			direction.z += speed * delta
		if Input.is_action_pressed("move_up"):
			direction.z -= speed * delta
		UpdatePosition.rpc(direction)
		position.x = position3D.x
		position.y = position3D.z

func CanSee(pos:Vector3) -> bool:
	var gridSize = GameManager.map.tileSize
	var yGrid = floor(position3D.y / gridSize)
	
	if pos.y >= yGrid * gridSize and pos.y <= yGrid * gridSize + gridSize:
		otherRef.position.x = pos.x
		otherRef.position.y = pos.z
		return true
	return false

func UpdateTileMap():
	var gridPos: Vector3i = Vector3i.ZERO
	gridPos.y = GameManager.map.GetCoordFromPos(position3D).y
	for z in range(GameManager.map.size.z):
		gridPos.x = 0
		for x in range(GameManager.map.size.x):
			SetTile(x, z, gridPos)
			gridPos.x += 1
		gridPos.z += 1
