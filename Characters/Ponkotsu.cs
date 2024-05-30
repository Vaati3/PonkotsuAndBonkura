using Godot;
using System;

public partial class Ponkotsu : Character
{
    float gravity = 9.8f;
    public void _physics_process(float delta)
    {
        if (!isControlled)
            return;
        Vector3 dir = Vector3.Zero;
        if (Input.IsActionPressed("move_right"))
            dir.X += speed;
        if (Input.IsActionPressed("move_left"))
            dir.X -= speed;
        if (Input.IsActionPressed("move_down"))
            dir.Z += speed;
        if (Input.IsActionPressed("move_up"))
            dir.Z -= speed;
        if (IsFalling())
            dir.Y += gravity * delta;    
        if (dir != Vector3.Zero)
            Move(dir);
    }

    public override CharacterType GetCharacterType()
    {
        return CharacterType.Ponkotsu;
    }
    public override Vector2 GetLocalPos(Vector3 pos)
    {
        return new Vector2(pos.X, pos.Z);
    }
    public override Vector3 GetGlobalPos(float x, float y)
    {
        return new Vector3(x, position3D.Y, y);
    }
    public override Vector3 GetGlobalPos(Vector2 pos, Vector3 dir)
    {
        return new Vector3(pos.X, position3D.Y + dir.Y, pos.Y);
    }

    protected override bool CanSee(Vector3 pos)
    {
        return (int)(position3D.Y / MapGenerator.tileSize) == (int)(pos.Y / MapGenerator.tileSize);
    }

    protected override void UpdateTile(Vector3I tilePos, int x, int y)
    {
        Vector2I tile = Vector2I.One;
        if (map.generator.GetTile(tilePos) == Tile.Void)
        {
            map.SetTile(x, y, tile);
            return;
        }
        if (map.generator.GetTile(tilePos.X - 1, tilePos.Y, tilePos.Z) == Tile.Void)
            tile.X -= 1;
        else if (map.generator.GetTile(tilePos.X + 1, tilePos.Y, tilePos.Z) == Tile.Void)
            tile.X += 1;
        if (map.generator.GetTile(tilePos.X, tilePos.Y, tilePos.Z - 1) == Tile.Void)
            tile.Y -= 1;
        else if (map.generator.GetTile(tilePos.X, tilePos.Y, tilePos.Z + 1) == Tile.Void)
            tile.Y += 1;
        map.SetTile(x, y, tile);
    }
    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Y = map.generator.GetTilePos(position3D).Y;
		for (int z = 0; z < map.generator.size.Z; z++)
		{
			tilePos.X = 0;
			for (int x = 0; x < map.generator.size.X; x++)
			{
				UpdateTile(tilePos, x, z);
				tilePos.X += 1;
			}
			tilePos.Z += 1;
		}
	}
}