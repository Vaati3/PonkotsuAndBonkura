extends Node

var savePath:String = "user://save.save"

var player:Dictionary = {}
var otherPlayer:Dictionary = {}

var map:Map

func _init():
	map = Map.new()
	map.OpenMap("testarea")
	
	for y in range(map.size.y):
		for z in range(map.size.z):
			var s:String = ""
			for x in range(map.size.x):
				s += str(map.GetTile(x, y, z))
			print(s)
		print("")

func Save():
	var file = FileAccess.open(savePath, FileAccess.WRITE)
	var data = JSON.stringify({
		"name": player.name,
		"isPonkotsu": player.isPonkotsu,
		"progression": player.progression
	})
	file.store_line(data)

func Load() -> bool:
	if not FileAccess.file_exists(savePath):
		player = {
			"name": "",
			"isPonkotsu": true,
			"progression": 0
		}
		return false
	var file = FileAccess.open(savePath, FileAccess.READ)
	var json = JSON.new()
	if json.parse(file.get_as_text()) != OK:
		print("json parse failed")
		return false
	player = json.data
	return true
