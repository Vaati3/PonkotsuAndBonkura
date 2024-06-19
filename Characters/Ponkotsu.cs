using Godot;
using System;

public partial class Ponkotsu : Character
{
    float gravity = 150;
    public override void _PhysicsProcess(double delta)
    {
        if (!isControlled)
            return;
        Vector3 dir = Vector3.Zero;
        if (Input.IsActionPressed("move_right"))
        {
            dir.X += speed;
            Flip(false);
        }
        if (Input.IsActionPressed("move_left"))
        {
            dir.X -= speed;
            Flip(true);
        }
        if (Input.IsActionPressed("move_down"))
            dir.Z += speed;
        if (Input.IsActionPressed("move_up"))
            dir.Z -= speed;
        if (IsFalling())
            dir.Y += gravity * (float)delta;    
        if (dir != Vector3.Zero)
            Move(dir);
    }

    public override void Move(Vector3 dir)
    {
        base.Move(dir);
        if (isControlled && dir.Y != 0)
            UpdateMap();
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

    public override bool CanSee(Vector3 pos)
    {
        return (int)(position3D.Y / MapGenerator.tileSize) == (int)(pos.Y / MapGenerator.tileSize);
    }

    protected override bool UpdateTile(Vector3I tilePos, int x, int y)
    {
        if (map.generator.GetTile(tilePos) == Tile.Void && 
            map.generator.GetTile(tilePos.X, tilePos.Y + 1, tilePos.Z) == Tile.Void)
        {
            map.SetTile(x, y, new Vector2I(4,2));
            return true;
        }
        Vector2I tile = Vector2I.One;
        if (base.UpdateTile(tilePos, x, y))
            return true;
        if (map.generator.GetTile(tilePos.X - 1, tilePos.Y, tilePos.Z) != Tile.Block)
            tile.X -= 1;
        if (map.generator.GetTile(tilePos.X + 1, tilePos.Y, tilePos.Z) != Tile.Block)
            tile.X += tile.X < 1 ? 3 : 1;;
        if (map.generator.GetTile(tilePos.X, tilePos.Y, tilePos.Z - 1) != Tile.Block)
            tile.Y -= 1;
        if (map.generator.GetTile(tilePos.X, tilePos.Y, tilePos.Z + 1) != Tile.Block)
            tile.Y += tile.Y < 1 ? 3 : 1;
        map.SetTile(x, y, tile);
        return true;
    }
    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Y = MapGenerator.GetTilePos(position3D).Y;
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