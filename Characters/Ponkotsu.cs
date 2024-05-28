using Godot;
using System;

public partial class Ponkotsu : Character
{
    public void _physics_process(float delta)
    {
        if (!isControlled)
            return;
        Vector3 dir = Vector3.Zero;
        if (Input.IsActionPressed("move_right"))
            dir.X += speed * delta;
        if (Input.IsActionPressed("move_left"))
            dir.X -= speed * delta;
        if (Input.IsActionPressed("move_down"))
            dir.Z += speed * delta;
        if (Input.IsActionPressed("move_up"))
            dir.Z -= speed * delta;
        float half = sprite.Texture.GetSize().Y * sprite.Scale.Y / 2f;
        if (map.generator.GetTile(position3D.X, position3D.Y + half, position3D.Z) == Tile.Void)
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

    protected override void UpdateMap()
	{
		Vector3I tilePos = Vector3I.Zero;
		tilePos.Y = map.generator.GetTilePos(position3D).Y;
		for (int z = 0; z < map.generator.size.Z; z++)
		{
			tilePos.X = 0;
			for (int x = 0; x < map.generator.size.X; x++)
			{
				map.UpdateTile(tilePos, x, z);
				tilePos.X += 1;
			}
			tilePos.Z += 1;
		}
	}
}