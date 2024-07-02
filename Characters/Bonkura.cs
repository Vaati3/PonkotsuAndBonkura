using Godot;
using System;

public partial class Bonkura : Character
{
    float jumpHeight = 130;
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
        dir.Y = !canFall && dir.Y > 0 ? 0 : dir.Y;
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
        if (Input.IsActionJustPressed("move_up") && !IsFalling())
            dir.Y = jumpVelocity;

        if (dir != Vector3.Zero)
            Move(pos.Convert(dir));
    }

    public override void Move(Vector3 dir)
    {
        base.Move(dir);
        if (dir.Z != 0)
            UpdateMap();
    }

    public override CharacterType GetCharacterType()
    {
        return CharacterType.Bonkura;
    }

    public void RotateAxis()
    {
        if (pos.blindAxis == Axis.Z)
            pos.blindAxis = Axis.X;
        else
            pos.blindAxis = Axis.Z;
        if (isControlled)
        {
            Position = pos.GetLocalPos();
            UpdateMap();
        }
    }

    protected override bool UpdateTile(Vector3I tilePos, int x, int y)
    {
        Vector2I tile = Vector2I.One;
        if (base.UpdateTile(tilePos, x, y))
            return true;
        if (map.generator.GetTile(tilePos + pos.Convert(-1 , 0, 0)) != Tile.Block)
            tile.X -= 1;
        if (map.generator.GetTile(tilePos + pos.Convert(1 , 0, 0)) != Tile.Block)
            tile.X += tile.X < 1 ? 3 : 1;
        if (map.generator.GetTile(tilePos + pos.Convert(0 , -1, 0)) != Tile.Block)
            tile.Y -= 1;
        if (map.generator.GetTile(tilePos + pos.Convert(0 , 1, 0)) != Tile.Block)
            tile.Y += tile.Y < 1 ? 3 : 1;
        map.SetTile(x, y, tile);
        return true;
    }

    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Z = (int)pos.GetBlindAxisValue(pos.globalPos) / MapGenerator.tileSize;
		for (int y = 0; y < map.generator.size.Y; y++)
		{
			tilePos.X = 0;
			for (int x = 0; x < map.generator.size.X; x++)
			{
				UpdateTile(pos.Convert(tilePos), x, y);
				tilePos.X += 1;
			}
			tilePos.Y += 1;
		}
	}
}