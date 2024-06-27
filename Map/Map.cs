using Godot;
using System;
using System.Collections.Generic;

public partial class Map : TileMap
{
	GameManager gameManager;
	public GameMenu gameMenu {get; private set;}
	public MapGenerator generator {get; private set;}
	Ponkotsu ponkotsu;
	Bonkura bonkura;
	Node objectLayer;
	List<Object> objects;
	Queue<Object> freeQueue;

	Label ponkotsuDebug;
	Label bonkuraDebug;

	public Timer switchCooldown {get; private set;}

	[Signal]public delegate void UnloadMapEventHandler();

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		gameMenu = GetNode<GameMenu>("GameMenu");
		generator = new MapGenerator();
		AddChild(generator);

		ponkotsu = GetNode<Ponkotsu>("Ponkotsu");
		bonkura = GetNode<Bonkura>("Bonkura");
		ponkotsu.init(this, bonkura);
		bonkura.init(this, ponkotsu);

		objectLayer = GetNode<Node>("Objects");
		objects = new List<Object>();
		freeQueue =  new Queue<Object>();

		ponkotsuDebug = GetNode<Label>("Shader/Debug/Ponkotsu");
		bonkuraDebug = GetNode<Label>("Shader/Debug/Bonkura");

		switchCooldown = new Timer()
		{
			WaitTime = 5,
			Autostart = false,
			OneShot = true			
		};
		AddChild(switchCooldown);
	}

    public override void _Process(double delta)
    {
        ponkotsuDebug.Text = "Ponkotsu X: " + ponkotsu.pos.globalPos.X + ", Y: " + ponkotsu.pos.globalPos.Y + ", Z: " + ponkotsu.pos.globalPos.Z;
		bonkuraDebug.Text = "Bonkura X: " + bonkura.pos.globalPos.X + ", Y: " + bonkura.pos.globalPos.Y + ", Z: " + bonkura.pos.globalPos.Z;
    }

    public void StartMap(string mapName, int mapNumber)
	{
		gameMenu.Init(mapName, mapNumber);
		generator.Read(mapName);
		GenerateObjects();
		if (gameManager.player.characterType == CharacterType.Ponkotsu)
			ponkotsu.Possess(generator.spawns);
		else
			bonkura.Possess(generator.spawns);
		GetNode<CanvasLayer>("Shader").Show();
	}

	public void SetTile(int x, int y, Vector2I tile)
	{
		SetCell(0, new Vector2I(x, y), 0, tile);
	}

	public void UpdateObjects()
	{
		foreach(Object obj in objects)
		{
			obj.Update();
		}
		while (freeQueue.Count > 0)
		{
			Object obj = freeQueue.Dequeue();
			objects.Remove(obj);
			obj.QueueFree();//object stays lingering and cause 2 "node" not found errors on server
		}
	}

	public void SwitchObject(Character character)
	{
		foreach(Object obj in objects)
		{
			obj.Switch(character);
		}
	}

	private void GenerateObjects()
	{

		Dictionary<Vector3I, Tile> buttons = new Dictionary<Vector3I, Tile>();
		MapGenerator.Action action = (tile, pos) => {
			if (tile > Tile.BonkuraGoal && tile != Tile.ElevatorStop)
			{
				switch (tile)
				{
					case Tile.ElevatorX: case Tile.ElevatorY : case Tile.ElevatorZ:
						List<Vector3I> stops = generator.Search(Tile.ElevatorStop, (Axis)((int)tile-(int)Tile.ElevatorX), pos);
						if (stops.Count != 0)
						{
							Elevator elevator = CreateObject<Elevator>(pos);
							elevator.Setup((Axis)((int)tile-(int)Tile.ElevatorX), stops, generator);
						}
						break;
					case Tile.ButtonX: case Tile.ButtonY: case Tile.ButtonZ:
						buttons.Add(pos, tile);
						break;
				}
				generator.SetTile(Tile.Void, pos);
			}
		};
		generator.LoopAction(action);
		foreach(KeyValuePair<Vector3I, Tile> button in buttons)
		{
			PressurePlate buttonObj = CreateObject<PressurePlate>(button.Key);
			foreach(Object obj in objects)
			{
				if (!obj.activatable)
					continue;
				Vector3I pos = MapGenerator.GetTilePos(obj.position3D);
				if ((button.Value == Tile.ButtonX && button.Key.X == pos.X) ||
					(button.Value == Tile.ButtonY && button.Key.Y == pos.Y) ||
					(button.Value == Tile.ButtonZ && button.Key.Z == pos.Z) )
				{
					buttonObj.ButtonPressed += obj.Trigger;
				}
			}
		}
		UpdateObjects();
	}

	private T CreateObject<T>(Vector3I tilePos) where T : Object, new()
	{
		Character character = gameManager.player.characterType == CharacterType.Ponkotsu ? ponkotsu : bonkura;
		Vector3 pos = AlignPos(tilePos);
		T obj = new T();
		obj.InitObject(character, pos);
		obj.FreeObject += FreeObject;

		objectLayer.AddChild(obj);
		objects.Add(obj);
		return obj;
	}

	private void FreeObject(Object obj)
	{
		freeQueue.Enqueue(obj);
	}

	static public Vector3 AlignPos(Vector3I tilePos)
	{
		Vector3 pos = MapGenerator.GetWorldPos(tilePos);
		pos.X += MapGenerator.tileSize / 2;
		pos.Z += MapGenerator.tileSize / 2;
		pos.Y += MapGenerator.tileSize / 2;

		return pos;
	}

	public void ClearMap()
	{
		foreach(Object obj in objects)
		{
			obj.QueueFree();
		}
		objects.Clear();

		Clear();
	}

	public void BacktoLobby()
	{
		ponkotsu.UnPossess();
		bonkura.UnPossess();
		ClearMap();
		GetNode<CanvasLayer>("Shader").Hide();
		EmitSignal(nameof(UnloadMap));
	}
}
