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
        bool falling = IsFalling();
        if (!falling && Input.IsActionPressed("move_right"))
        {
            dir.X += speed;
            Flip(false);
        }
        if (!falling && Input.IsActionPressed("move_left"))
        {
            dir.X -= speed;
            Flip(true);
        }
        if (!falling && Input.IsActionPressed("move_down"))
            dir.Z += speed;
        if (!falling && Input.IsActionPressed("move_up"))
            dir.Z -= speed;
        if (falling)
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

    public override bool CanSee(Vector3 otherPos)
    {
        return (int)(pos.globalPos.Y / MapGenerator.tileSize) == (int)(otherPos.Y / MapGenerator.tileSize);
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
		tilePos.Y = MapGenerator.GetTilePos(pos.globalPos).Y;
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