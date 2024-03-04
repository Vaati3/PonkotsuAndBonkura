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

func UpdatePosition(direction:Vector3):
	super(direction)
	position.x = postion3D.x
	position.y = postion3D.z

func CanSee(pos:Vector3) -> bool:
	var yGrid = floor(postion3D.y / gridSize)
	
	if pos.y >= yGrid * gridSize and pos.y <= yGrid * gridSize + gridSize:
		return true
	return false
