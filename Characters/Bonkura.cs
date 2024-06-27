using Godot;
using System;

public partial class Bonkura : Character
{
    float jumpHeight = 100;
    float jumpAscendTime = 0.5f;
    float jumpFallTime = 0.4f;
    float jumpVelocity;
    float jumpGravity;
    float fallGravity;

    public override void _Ready()
    {
        base._Ready();
        jumpVelocity = 2 * jumpHeight / jumpAscendTime * -1;
        jumpGravity = -2 * jumpHeight / (jumpAscendTime * jumpAscendTime) * -1;
        fallGravity = -2 * jumpHeight / (jumpFallTime * jumpFallTime) * -1;

        sprite.Texture = GD.Load<Texture2D>("res://Characters/Bonkura.png");
    }

    private float GetGravity()
    {
        return Velocity.Y < 0 ? jumpGravity : fallGravity;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isControlled)
            return;
        Vector3 dir = Vector3.Zero;
        dir.Y = Velocity.Y + (GetGravity() * (float)delta);
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
        if (Input.IsActionPressed("move_up") && !IsFalling())
            dir.Y = jumpVelocity;
        
        if (dir != Vector3.Zero)
            Move(dir);
    }

    public override void Move(Vector3 dir)
    {
        base.Move(dir);
        if (dir.X != 0)
            UpdateMap();
    }

    public override CharacterType GetCharacterType()
    {
        return CharacterType.Bonkura;
    }

    public override bool CanSee(Vector3 otherPos)
    {
        return (int)(pos.globalPos.Z / MapGenerator.tileSize) == (int)(otherPos.Z / MapGenerator.tileSize);
    }

    protected override bool UpdateTile(Vector3I tilePos, int x, int y)
    {
        Vector2I tile = Vector2I.One;
        if (base.UpdateTile(tilePos, x, y))
            return true;
        if (map.generator.GetTile(tilePos.X - 1, tilePos.Y, tilePos.Z) != Tile.Block)
            tile.X -= 1;
        if (map.generator.GetTile(tilePos.X + 1, tilePos.Y, tilePos.Z) != Tile.Block)
            tile.X += tile.X < 1 ? 3 : 1;
        if (map.generator.GetTile(tilePos.X, tilePos.Y - 1, tilePos.Z) != Tile.Block)
            tile.Y -= 1;
        if (map.generator.GetTile(tilePos.X, tilePos.Y + 1, tilePos.Z) != Tile.Block)
            tile.Y += tile.Y < 1 ? 3 : 1;
        map.SetTile(x, y, tile);
        return true;
    }

    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Z = MapGenerator.GetTilePos(pos.globalPos).Z;
		for (int y = 0; y < map.generator.size.Y; y++)
		{
			tilePos.X = 0;
			for (int x = 0; x < map.generator.size.X; x++)
			{
				UpdateTile(tilePos, x, y);
				tilePos.X += 1;
			}
			tilePos.Y += 1;
		}
	}
}