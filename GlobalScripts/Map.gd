extends Node
class_name Map

#spawns set to none
enum Tile {
	None,
	Block,
	PonkotsuSpawn,
	BonkuraSpawn
}

var tileValues: Array[Vector2i]= [
	Vector2i(0, 0),
	Vector2i(1, 0),
	Vector2i(2, 0),
	Vector2i(0, 1),
	Vector2i(1, 1),
	Vector2i(2, 1),
	Vector2i(0, 2),
	Vector2i(1, 2),
	Vector2i(2, 2)
]

var tileSize: int = 100
var size: Vector3i
var data: Array[Tile]

var ponkotsuSpawn: Vector3
var bonkuraSpawn: Vector3

func _init():
	pass

func SetTile(c, index: int):
	var pos: Vector3 = Vector3(
		floor(index / size.x % size.y),
		(size.y - 1) - floor(index / (size.y * size.x)),
		index % size.x
	)
	if (c == 2 or c == 3):
		print (str(pos) + " " + str(c))
		var half = tileSize / 2
		var realPos = pos * tileSize
		if c == 2:
			ponkotsuSpawn = Vector3(realPos.x + half, realPos.y + half, realPos.z + half)
		else :
			bonkuraSpawn = Vector3(realPos.x + half, realPos.y + half, realPos.z + half)
	data[pos.x + (pos.z * size.x) + (pos.y * size.x * size.z)] = c

func OpenMap(mapName):
	var path = "res://Maps/" + mapName + ".dat"
	if not FileAccess.file_exists(path):
		print("map file missing")
		return
	
	var file = FileAccess.open(path, FileAccess.READ)
	var tmp = file.get_line().split("x")
	size = Vector3i(int(tmp[0]), int(tmp[2]), int(tmp[1]))
	data.resize(size.x * size.y * size.z)
	
	var i:int = 0
	var buffer = file.get_buffer(size.x * size.y * size.z)
	while i < size.x * size.y * size.z:
		SetTile(buffer[i], i)
		i += 1

func Print():
	for y in range(size.y):
		for z in range(size.z):
			var s:String = ""
			for x in range(size.x):
				s += str(GetTile(x, y, z))
			print(s)
		print("")

func GetTile(x: int, y: int, z: int):
	return data[x + (z * size.x) + (y * size.x * size.z)]

func GetTileV(pos: Vector3):
	return data[pos.x + (pos.z * size.x) + (pos.y * size.x * size.z)]

func GetCoordFromPos(pos: Vector3) -> Vector3i:
	return Vector3i(floor(pos.x / tileSize), floor(pos.y / tileSize), floor(pos.z / tileSize))

func GetPosFromCoord(pos: Vector3i) -> Vector3:
	return (pos * tileSize)

func GetTileValue(tile: Tile):
	return tileValues[tile]
