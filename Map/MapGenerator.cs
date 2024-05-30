using Godot;
using System;

public enum Tile {
	Void,
	Block,
	PonkotsuSpawn,
	BonkuraSpawn
}

public partial class MapGenerator : Node
{
	public const int tileSize = 100;
	public Vector3I size {get; private set;}
	public Tile[] data {get; private set;}
	public Vector3[] spawns {get; private set;}

    public override void _Ready()
    {
		spawns = new Vector3[2];
    }

	private void SetData(byte b, int i)
	{
		Vector3I pos = new Vector3I(
			i / size.X % size.Y,
			size.Y - 1 - (i / (size.Y * size.X)),
			i % size.X
		);

		if (b == 2 || b == 3){
			spawns[b-2] = pos * tileSize + new Vector3(tileSize/2, tileSize/2, tileSize/2);
			b = 0;
		}
		data[pos.X + (pos.Z * size.X) + (pos.Y * size.X * size.Z)] = (Tile)b;
	}

	public bool Read(string mapName)
	{
		string path = "res://Map/Maps/" + mapName + ".dat";
		if (! FileAccess.FileExists(path))
			return false;
		FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string[] sizeStr = file.GetLine().Split("x");
		size = new Vector3I(sizeStr[0].ToInt(), sizeStr[1].ToInt(), sizeStr[2].ToInt());
		int totalSize = size.X * size.Y * size.Z;
		data = new Tile[totalSize];
		byte[] buffer = file.GetBuffer(totalSize);
		for (int i = 0; i < totalSize; i++)
			SetData(buffer[i], i);
		return true;
	}

	public Tile GetTile(Vector3 pos)
	{
		return GetTile(GetTilePos(pos));
	}
	public Tile GetTile(Vector3I pos)
	{
		if (pos.X < 0 || pos.X >= size.X || pos.Y < 0 || pos.Y >= size.Y || pos.Z < 0 || pos.Z >= size.Z)
			return Tile.Block;
		return data[pos.X + (pos.Z * size.X) + (pos.Y * size.X * size.Z)];
	}
	public Tile GetTile(float x, float y, float z)
	{
		return GetTile(GetTilePos(x, y, z));
	}
	public Tile GetTile(int x, int y, int z)
	{
		if (x < 0 || x >= size.X || y < 0 || y >= size.Y || z < 0 || z >= size.Z)
			return Tile.Block;
		return data[x + (z * size.X) + (y * size.X * size.Z)];
	}
	public Vector3I GetTilePos(Vector3 pos)
	{
		return new Vector3I((int)pos.X / tileSize, (int) pos.Y / tileSize, (int) pos.Z / tileSize);
	}
	public Vector3I GetTilePos(float x, float y, float z)
	{
		return new Vector3I((int)x / tileSize, (int) y / tileSize, (int) z / tileSize);
	}
	public Vector3 GetWorldPos(Vector3I pos)
	{
		return pos * tileSize;
	}
}
