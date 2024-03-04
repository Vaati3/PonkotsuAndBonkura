extends BaseCharacter
class_name Bonkura

var isOnXAxis:bool = true
var gravity:float = 9.8

func _physics_process(delta):
	if GameManager.player.id == controllerId:
		var direction:Vector3 = Vector3.ZERO
		if Input.is_action_pressed("move_right"):
			direction.x += speed * delta
		if Input.is_action_pressed("move_left"):
			direction.x -= speed * delta
		if Input.is_action_pressed("move_down"):
			direction.y += speed * delta
		if Input.is_action_pressed("move_up"):
			direction.y -= speed * delta
		UpdatePosition.rpc(direction)

func UpdatePosition(direction:Vector3):
	super(direction)
	position.x = postion3D.x
	position.y = postion3D.y

func CanSee(pos:Vector3) -> bool:
	var xGrid = floor(postion3D.x / gridSize)
	
	if pos.x >= xGrid * gridSize and pos.x <= xGrid * gridSize + gridSize:
		return true
	return false
