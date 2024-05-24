using Godot;
using System;

public partial class Map : TileMap
{
	GameManager gameManager;
	MapGenerator mapGenerator;
	Ponkotsu ponkotsu;
	Bonkura bonkura;
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		mapGenerator = new MapGenerator();
		AddChild(mapGenerator);

		ponkotsu = GetNode<Ponkotsu>("Ponkotsu");
		bonkura = GetNode<Bonkura>("Bonkura");
		if (gameManager.player.characterType == CharacterType.Ponkotsu)
			ponkotsu.Possess(gameManager.player.id, bonkura);
		else
			bonkura.Possess(gameManager.player.id, ponkotsu);
	}

	public void StartMap(string mapName)
	{
		mapGenerator.Read(mapName);
	}

	public void UpdateTile(Vector3I tilePos, int x, int y)
	{
		Tile tile = mapGenerator.GetTile(tilePos);
		Vector2I tileValue = tile == Tile.Void? Vector2I.Zero : new Vector2I(1, 0);
		SetCell(0, new Vector2I(x, y), 0, tileValue);
	}

	public void UpdateMapPonkotsu(Vector3 pos)
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Y = mapGenerator.GetTilePos(pos).Y;
		for (int z = 0; z < mapGenerator.size.Z; z++)
		{
			tilePos.X = 0;
			for (int x = 0; x < mapGenerator.size.X; x++)
			{
				UpdateTile(tilePos, x, z);
				tilePos.X += 1;
			}
			tilePos.Z += 1;
		}
	}
	public void UpdateMapBonkura(Vector3 pos)
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.X = mapGenerator.GetTilePos(pos).X;
		for (int y = 0; y < mapGenerator.size.Y; y++)
		{
			tilePos.Y = 0;
			for (int z = 0; z < mapGenerator.size.Z; z++)
			{
				UpdateTile(tilePos, z, y);
				tilePos.Z += 1;
			}
			tilePos.Y += 1;
		}
	}
}
