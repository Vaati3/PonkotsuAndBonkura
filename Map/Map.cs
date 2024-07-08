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
	public List<Object> objects {get; private set;}
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
		generator.CreateItem += CreateItem;
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
		FillObjects();
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

	private void FillObjects()
	{
		Character character = gameManager.player.characterType == CharacterType.Ponkotsu ? ponkotsu : bonkura;
		Dictionary<Vector3I, Tile> buttons = new Dictionary<Vector3I, Tile>();
		MapGenerator.Action action = (tile, pos) => {
			if (tile > Tile.BonkuraGoal && tile != Tile.ElevatorStop)
			{
				Tile newTile = Tile.Void;
				switch (tile)
				{
					case Tile.ElevatorX: case Tile.ElevatorY : case Tile.ElevatorZ:
						AddObject(Elevator.CreateElevator(this, character, pos, tile));
						break;
					case Tile.ButtonX: case Tile.ButtonY: case Tile.ButtonZ:
						buttons.Add(pos, tile);
						break;
					case Tile.Rotator:
						AddObject(Object.CreateObject<Rotator>(character, pos));
						break;
					case Tile.Door:
						AddObject(Object.CreateObject<Door>(character, pos));
						newTile = Tile.Block;
						break;
				}
				generator.SetTile(newTile, pos);
			}
		};
		generator.LoopAction(action);
		foreach(KeyValuePair<Vector3I, Tile> button in buttons)
			AddObject(PressurePlate.CreatePressurePlate(character, button.Key, button.Value, objects));
		UpdateObjects();
	}

	private void AddObject(Object obj)
	{
		if (obj == null)
			return;
		obj.FreeObject += FreeObject;
		objectLayer.AddChild(obj);
		objects.Add(obj);
	}

	private void CreateItem(ItemType type, Vector3I pos)
	{
		Character character = gameManager.player.characterType == CharacterType.Ponkotsu ? ponkotsu : bonkura;
		ItemPickup itemPickup = Object.CreateObject<ItemPickup>(character, pos);
		itemPickup.CreateItem(type);
		AddObject(itemPickup);
	}

	public void FreeObject(Object obj)
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

	public void FreeMap()
	{
		ClearMap();
		ponkotsu.QueueFree();
		bonkura.QueueFree();
		QueueFree();
	}
}
