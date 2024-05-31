using Godot;
using System;

public partial class Map : TileMap
{
	GameManager gameManager;
	public MapGenerator generator {get; private set;}
	Ponkotsu ponkotsu;
	Bonkura bonkura;

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
		generator.spawns[0] = new Vector3(550, 250, 40);
		if (gameManager.player.characterType == CharacterType.Ponkotsu)
			ponkotsu.Possess(generator.spawns);
		else 
			bonkura.Possess(generator.spawns);
	}

	public void SetTile(int x, int y, Vector2I tile)
	{
		// Tile tile = generator.GetTile(tilePos);
		// Vector2I tileValue = tile == Tile.Void? Vector2I.Zero : new Vector2I(1, 0);
		SetCell(0, new Vector2I(x, y), 0, tile);
	}
}
