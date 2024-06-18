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
	Queue<Object> freeQueue;

	Label ponkotsuDebug;
	Label bonkuraDebug;

	public Timer switchCooldown {get; private set;}

	[Signal]public delegate void UnloadMapEventHandler();

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
		freeQueue =  new Queue<Object>();
		ponkotsuDebug = GetNode<Label>("Debug/Ponkotsu");
		bonkuraDebug = GetNode<Label>("Debug/Bonkura");

		if (gameManager.player.id == 1)
			GetNode<Button>("Popup/Panel/Button").Visible = true;

		switchCooldown = new Timer()
		{
			WaitTime = 5,
			Autostart = true,
			OneShot = true			
		};
		AddChild(switchCooldown);
	}

    public override void _Process(double delta)
    {
        ponkotsuDebug.Text = "Ponkotsu X: " + ponkotsu.position3D.X + ", Y: " + ponkotsu.position3D.Y + ", Z: " + ponkotsu.position3D.Z;
		bonkuraDebug.Text = "Bonkura X: " + bonkura.position3D.X + ", Y: " + bonkura.position3D.Y + ", Z: " + bonkura.position3D.Z;
    }

    public void StartMap(string mapName)
	{
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

	private void GenerateObjects()
	{
		Dictionary<Vector3I, Tile> buttons = new Dictionary<Vector3I, Tile>();
		for (int i = 0; i < generator.dataSize; i++)
		{
			if (generator.data[i] > Tile.BonkuraGoal && generator.data[i] != Tile.ElevatorStop)
			{
				switch (generator.data[i])
				{
					case Tile.ElevatorX: case Tile.ElevatorY : case Tile.ElevatorZ:
						Vector3I pos = generator.indexToPos(i);
						List<Vector3I> stops = generator.Search(Tile.ElevatorStop, (Axis)((int)generator.data[i]-(int)Tile.ElevatorX), pos);
						if (stops.Count != 0)
						{
							Elevator elevator = CreateObject<Elevator>(pos);
							elevator.Setup((Axis)((int)generator.data[i]-(int)Tile.ElevatorX), stops, generator);
						}
						break;
					case Tile.ButtonX: case Tile.ButtonY: case Tile.ButtonZ:
						buttons.Add(generator.indexToPos(i), generator.data[i]);
						break;
				}
				generator.data[i] = Tile.Void;
			}
		}
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

	public void MapCompleted()
	{
		Rpc(nameof(ShowPopup), true);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ShowPopup(bool state)
	{
		GetNode<CanvasLayer>("Popup").Visible = state;
	}

	public void _on_button_pressed()
	{
		Rpc(nameof(BacktoLobby));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void BacktoLobby()
	{
		foreach(Object obj in objects)
		{
			obj.QueueFree();
		}
		objects.Clear();
		ShowPopup(false);
		if (gameManager.player.characterType == CharacterType.Ponkotsu)
			ponkotsu.UnPossess();
		else
			bonkura.UnPossess();
		GetNode<CanvasLayer>("Shader").Hide();
		EmitSignal(nameof(UnloadMap));
	}
}
