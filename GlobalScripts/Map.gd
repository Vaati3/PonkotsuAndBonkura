extends Node
class_name Map

#spawns set to none
enum Tile {
	None,
	Block,
	PonkotsuSpawn,
	BonkuraSpawn
}

var size: Vector3
var data: Array[Tile]

func _init():
	pass

func OpenMap(mapName):
	var path = "res://Maps/" + mapName + ".dat"
	if not FileAccess.file_exists(path):
		print("map file missing")
		return
	
	var file = FileAccess.open(path, FileAccess.READ)
	var tmp = file.get_line().split("x")
	size = Vector3(int(tmp[0]), int(tmp[2]), int(tmp[1]))
	data.resize(size.x * size.y * size.z)
	
	var i:int = 0
	var buffer = file.get_buffer(size.x * size.y * size.z)
	for c in buffer:
		data[i] = c
		#if c >= 2: c = 0
		i += 1

func GetTile(x: int, y: int, z: int):
	return data[x + (z * size.x) + (y * size.x * size.z)]

func GetTileV(pos: Vector3):
	return data[pos.x + (pos.z * size.x) + (pos.y * size.x * size.z)]
