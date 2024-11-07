using Godot;
using System.Collections.Generic;
using System.Drawing;

public enum Tile {
	Void,
	Block,
	PonkotsuSpawn, BonkuraSpawn,
	PonkotsuGoal, BonkuraGoal,
	ElevatorX, ElevatorY, ElevatorZ, ElevatorStop,
	ButtonX, ButtonY, ButtonZ,
	Rotator, Door, Swapper, Box
}

public partial class MapGenerator : Node
{
	public const int tileSize = 100;
	public Vector3I size {get; private set;}
	public Tile[,,] data {get; private set;}
	public Vector3[] spawns {get; private set;}

	public delegate void CreateItemEventHandler(ItemType item, Vector3I pos);
	public CreateItemEventHandler CreateItem;
    public override void _Ready()
    {
		spawns = new Vector3[2];
    }

	private void SetData(byte b, int x, int y, int z, bool raw)
	{
		Vector3I pos = new Vector3I(x, y, z);
		if (raw)
		{
			SetTile((Tile)b, pos);
			return;
		}
		if (b == 2 || b == 3){
			spawns[b-2] = pos * tileSize + new Vector3(tileSize/2, tileSize/2, tileSize/2);
			b = 0;
		}
		if (b > 200)
		{
			CreateItem((ItemType)(255 - b), pos);
			b = 0;
		}

		SetTile((Tile)b, pos);
	}

	public bool Read(string mapName, string folder = "res://Map/Maps/", bool raw = false)
	{
		string path = folder + mapName + ".dat";
		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (file == null)
			return false;
		string[] sizeStr = file.GetLine().Split("x");
		size = new Vector3I(sizeStr[0].ToInt(), sizeStr[2].ToInt(), sizeStr[1].ToInt());
		data = new Tile[size.X, size.Y, size.Z];
		byte[] buffer = file.GetBuffer(size.X * size.Y * size.Z);
		file.Close();
		int i = 0;
		for (int y = size.Y - 1; y >= 0; y--)
		{
			for(int z = 0; z < size.Z; z++)
			{
				for (int x = 0; x < size.X; x++)
				{
					SetData(buffer[i], x, y, z, raw);
					i++;
				}
			}
		}
		return true;
	}

	public bool Save(string fileName, string folder)
	{
		string path = folder + fileName + ".dat";

		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		if (file == null)
			return false;
		byte[] buffer = new byte[size.X * size.Y * size.Z];
		int i = 0;
		for (int y = size.Y - 1; y >= 0; y--)
		{
			for(int z = 0; z < size.Z; z++)
			{
				for (int x = 0; x < size.X; x++)
				{
					buffer[i] = (byte)data[x, y, z];
					i++;
				}
			}
		}
		file.StoreLine(size.X + "x" + size.Z + "x" + size.Y);
		file.StoreBuffer(buffer);
		file.Close();
		return true;
	}

	public void New()
	{
		size = new Vector3I(3, 3, 3);
		data = new Tile[size.X, size.Y, size.Z];
		
		for (int y = 0; y < size.Y; y++)
		{
			for(int z = 0; z < size.Z; z++)
			{
				for (int x = 0; x < size.X; x++)
				{
					data[x,y,z] = y == size.Y - 1 ? Tile.Block : Tile.Void;
				}
			}
		}
	}

	//brute force resize
	public void Resize(Vector3I newSize, Editor.SetMeshAction setMesh)
	{
		Tile[,,] newData = new Tile[newSize.X, newSize.Y, newSize.Z];
		
		for (int y = 0; y < newSize.Y; y++)
		{
			for(int z = 0; z < newSize.Z; z++)
			{
				for (int x = 0; x < newSize.X; x++)
				{
					if (x >= size.X || y >= size.Y || z >= size.Z) {
						newData[x,y,z] = y == newSize.Y - 1 ? Tile.Block : Tile.Void;
						setMesh(newData[x,y,z], new Vector3I(x,y,z));
					} else {
						newData[x,y,z] = data[x,y,z];
					}
				}
			}
		}
		
		size = newSize;
		data = newData;
	}

	public delegate void Action(Tile tile, Vector3I pos);
	public void LoopAction(Action action)
	{
		Vector3I pos = Vector3I.Zero;
		for (pos.Y = 0; pos.Y < size.Y; pos.Y++)
		{
			for(pos.Z = 0; pos.Z < size.Z; pos.Z++)
			{
				for (pos.X = 0; pos.X < size.X; pos.X++)
				{
					action(GetTile(pos), pos);
				}
			}
		}
	}

	public List<Vector3I> Search(Tile tile, Axis axis, Vector3I pos)
	{
		List<Vector3I> result = new List<Vector3I>();
		Vector3I increment = axis == Axis.X ? Vector3I.Right : axis == Axis.Y ? Vector3I.Up : Vector3I.Back;
		int limit = axis == Axis.X ? size.X : axis == Axis.Y ? size.Y : size.Z;
		pos.X = axis == Axis.X ? 0 : pos.X;
		pos.Y = axis == Axis.Y ? 0 : pos.Y;
		pos.Z = axis == Axis.Z ? 0 : pos.Z;

		for (int i = 0; i < limit; i++)
		{
			if (GetTile(pos) == tile)
				result.Add(pos);
			pos += increment;
		}
		return result;
	}

	public bool IsOutOfBound(Vector3I pos)
	{
		return pos.X < 0 || pos.X >= size.X || pos.Y < 0 || pos.Y >= size.Y || pos.Z < 0 || pos.Z >= size.Z;
	}
	public bool IsOutOfBound(int x, int y, int z)
	{
		return x < 0 || x >= size.X || y < 0 || y >= size.Y || z < 0 || z >= size.Z;
	}

	public bool SetTile(Tile tile, Vector3I pos)
	{
		if (IsOutOfBound(pos))
			return false;
		data[pos.X, pos.Y, pos.Z] = tile;
		return true;
	}

	public Tile GetTile(Vector3 pos)
	{
		return GetTile(GetTilePos(pos));
	}
	public Tile GetTile(Vector3I pos)
	{
		if (IsOutOfBound(pos))
			return Tile.Block;
		return data[pos.X, pos.Y, pos.Z];
	}
	public Tile GetTile(float x, float y, float z)
	{
		return GetTile(GetTilePos(x, y, z));
	}
	public Tile GetTile(int x, int y, int z)
	{
		if (IsOutOfBound(x, y, z))
			return Tile.Block;
		return data[x, y, z];
	}
	static public Vector3I GetTilePos(Vector3 pos)
	{
		return new Vector3I((int)pos.X / tileSize, (int) pos.Y / tileSize, (int) pos.Z / tileSize);
	}
	static public Vector3I GetTilePos(float x, float y, float z)
	{
		return new Vector3I((int)x / tileSize, (int) y / tileSize, (int) z / tileSize);
	}
	static public Vector3 GetWorldPos(Vector3I pos)
	{
		return pos * tileSize;
	}
}
