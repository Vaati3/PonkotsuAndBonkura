extends BaseCharacter
class_name Bonkura

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
