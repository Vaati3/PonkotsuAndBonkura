using Godot;
using System;

public partial class Bonkura : Character
{
    [Export]float jumpHeight = 100;
    [Export]float jumpAscendTime = 0.5f;
    [Export]float jumpFallTime = 0.4f;

    
    float jumpVelocity;
    float jumpGravity;
    float fallGravity;

    public override void _Ready()
    {
        jumpVelocity = 2 * jumpHeight / jumpAscendTime * -1;
        jumpGravity = -2 * jumpHeight / (jumpAscendTime * jumpAscendTime) * -1;
        fallGravity = -2 * jumpHeight / (jumpFallTime * jumpFallTime) * -1;
    }

    private float GetGravity()
    {
        return Velocity.Y < 0 ? jumpGravity : fallGravity;
    }

    public void _physics_process(float delta)
    {
        if (!isControlled)
            return;
        Vector3 dir = Vector3.Zero;
        dir.Y = Velocity.Y + (GetGravity() * delta);
        if (Input.IsActionPressed("move_right"))
            dir.Z += speed;
        if (Input.IsActionPressed("move_left"))
            dir.Z -= speed;
        if (Input.IsActionPressed("move_up") && !IsFalling())
            dir.Y = jumpVelocity;
        
        if (dir != Vector3.Zero)
            Move(dir);
    }

    public override CharacterType GetCharacterType()
    {
        return CharacterType.Bonkura;
    }
    public override Vector2 GetLocalPos(Vector3 pos)
    {
        return new Vector2(pos.Z, pos.Y);
    }
    public override Vector3 GetGlobalPos(float x, float y)
    {
        return new Vector3(position3D.X, y, x);
    }
    public override Vector3 GetGlobalPos(Vector2 pos, Vector3 dir)
    {
        return new Vector3(position3D.X + dir.X, pos.Y, pos.X);
    }

    protected override bool CanSee(Vector3 pos)
    {
        return (int)(position3D.X / MapGenerator.tileSize) == (int)(pos.X / MapGenerator.tileSize);
    }
    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.X = map.generator.GetTilePos(position3D).X;
		for (int y = 0; y < map.generator.size.Y; y++)
		{
			tilePos.Y = 0;
			for (int z = 0; z < map.generator.size.Z; z++)
			{
				map.UpdateTile(tilePos, z, y);
				tilePos.Z += 1;
			}
			tilePos.Y += 1;
		}
	}
}