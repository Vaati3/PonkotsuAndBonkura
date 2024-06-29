using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

public enum Tile {
	Void,
	Block,
	PonkotsuSpawn, BonkuraSpawn,
	PonkotsuGoal, BonkuraGoal,
	ElevatorX, ElevatorY, ElevatorZ, ElevatorStop,
	ButtonX, ButtonY, ButtonZ,
	Rotator
}

public partial class MapGenerator : Node
{
	public const int tileSize = 100;
	public Vector3I size {get; private set;}
	public Tile[,,] data {get; private set;}
	public Vector3[] spawns {get; private set;}

	[Signal] public delegate void CreateObjectEventHandler(Vector3I pos); 

    public override void _Ready()
    {
		spawns = new Vector3[2];
    }

	private void SetData(byte b, int x, int y, int z)
	{
		Vector3I pos = new Vector3I(x, y, z);
		if (b == 2 || b == 3){
			spawns[b-2] = pos * tileSize + new Vector3(tileSize/2, tileSize/2, tileSize/2);
			b = 0;
		}
		//Check and create items
		SetTile((Tile)b, pos);
	}

	public bool Read(string mapName)
	{
		string path = "res://Map/Maps/" + mapName + ".dat";
		if (! FileAccess.FileExists(path))
			return false;
		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string[] sizeStr = file.GetLine().Split("x");
		size = new Vector3I(sizeStr[0].ToInt(), sizeStr[2].ToInt(), sizeStr[1].ToInt());
		data = new Tile[size.X, size.Y, size.Z];
		byte[] buffer = file.GetBuffer(size.X * size.Y * size.Z);
		int i = 0;
		for (int y = size.Y - 1; y >= 0; y--)
		{
			for(int z = 0; z < size.Z; z++)
			{
				for (int x = 0; x < size.X; x++)
				{
					SetData(buffer[i], x, y, z);
					i++;
				}
			}
		}
		return true;
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

	public void SetTile(Tile tile, Vector3I pos)
	{
		data[pos.X, pos.Y, pos.Z] = tile;
	}

	public Tile GetTile(Vector3 pos)
	{
		return GetTile(GetTilePos(pos));
	}
	public Tile GetTile(Vector3I pos)
	{
		if (pos.X < 0 || pos.X >= size.X || pos.Y < 0 || pos.Y >= size.Y || pos.Z < 0 || pos.Z >= size.Z)
			return Tile.Block;
		return data[pos.X, pos.Y, pos.Z];
	}
	public Tile GetTile(float x, float y, float z)
	{
		return GetTile(GetTilePos(x, y, z));
	}
	public Tile GetTile(int x, int y, int z)
	{
		if (x < 0 || x >= size.X || y < 0 || y >= size.Y || z < 0 || z >= size.Z)
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
