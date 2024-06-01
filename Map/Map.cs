using Godot;
using System;
using System.Collections.Generic;

public partial class Map : TileMap
{
	GameManager gameManager;
	public MapGenerator generator {get; private set;}
	Ponkotsu ponkotsu;
	Bonkura bonkura;
	Node objectLayer;
	List<Object> objects;

	Label ponkotsuDebug;
	Label bonkuraDebug;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		generator = new MapGenerator();
		AddChild(generator);

		ponkotsu = GetNode<Ponkotsu>("Ponkotsu");
		bonkura = GetNode<Bonkura>("Bonkura");
		ponkotsu.init(this, bonkura);
		bonkura.init(this, ponkotsu);

		objectLayer = GetNode<Node>("Objects");
		objects = new List<Object>();
		ponkotsuDebug = GetNode<Label>("Debug/Ponkotsu");
		bonkuraDebug = GetNode<Label>("Debug/Bonkura");

	}

    public override void _Process(double delta)
    {
        ponkotsuDebug.Text = "Ponkotsu X: " + ponkotsu.position3D.X + ", Y: " + ponkotsu.position3D.Y + ", Z: " + ponkotsu.position3D.Z;
		bonkuraDebug.Text = "Bonkura X: " + bonkura.position3D.X + ", Y: " + bonkura.position3D.Y + ", Z: " + bonkura.position3D.Z;
    }

    public void StartMap(string mapName)
	{
		generator.Read(mapName);
		//Test to be removed
		//generator.spawns[0] = new Vector3(550, 250, 40);
		CreateObject<Elevator>(new Vector3I(3,5,3));
		//Test to be removed
		if (gameManager.player.characterType == CharacterType.Ponkotsu)
			ponkotsu.Possess(generator.spawns);
		else
			bonkura.Possess(generator.spawns);
	}

	public void SetTile(int x, int y, Vector2I tile)
	{
		SetCell(0, new Vector2I(x, y), 0, tile);
	}

	public void UpdateObjects()
	{
		foreach(Object obj in objects)
		{
			obj.UpdateVisibility();
		}
	}

	private void ObjectsFromMap()
	{
		for (int i = 0; i < generator.dataSize; i++)
		{
			if (generator.data[i] > Tile.BonkuraGoal)
			{
				switch (generator.data[i])
				{
					case Tile.ElevatorX: case Tile.ElevatorY : case Tile.ElevatorZ:
						Vector3I pos = generator.indexToPos(i);
						Vector3I? stop = generator.Search(Tile.ElevatorStop, (Axis)((int)generator.data[i]-(int)Tile.ElevatorX), pos);
						if (stop != null)
						{
							Elevator elevator = CreateObject<Elevator>(pos);
							elevator.stop = stop.Value;
						}
						break;
				}
				generator.data[i] = Tile.Void;
			}
		}
	}

	private T CreateObject<T>(Vector3I tilePos) where T : Object, new()
	{
		Character character = gameManager.player.characterType == CharacterType.Ponkotsu ? ponkotsu : bonkura;
		Vector3 pos = generator.GetWorldPos(tilePos);
		pos.Y += MapGenerator.tileSize - 1;
		T obj = new T();
		obj.InitObject(character, pos);

		objectLayer.AddChild(obj);
		objects.Add(obj);
		return obj;
	}
}
