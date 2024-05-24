using Godot;
using System;

public partial class Bonkura : Character
{
    public void _physics_process(float delta)
    {
        if (gameManager.player.id != controllerId)
            return;
        Vector3 dir = Vector3.Zero;
        if (Input.IsActionPressed("move_right"))
            dir.Z += speed * delta;
        if (Input.IsActionPressed("move_left"))
            dir.Z -= speed * delta;
        if (Input.IsActionPressed("move_down"))
            dir.Y += speed * delta;
        if (Input.IsActionPressed("move_up"))
            dir.Y -= speed * delta;
        if (dir != Vector3.Zero)
            Rpc(nameof(UpdatePosition), dir);
    }
    public override void UpdatePosition(Vector3 dir)
    {
        base.UpdatePosition(dir);
        if (controllerId == gameManager.player.id)
            Position = new Vector2(position3D.Z, position3D.Y);
        else
            Position = new Vector2(position3D.X, position3D.Z);
    }
    public override CharacterType GetCharacterType()
    {
        return CharacterType.Bonkura;
    }
    protected override bool CanSee(Vector3 pos)
    {
        int xLayer = (int)(position3D.X / MapGenerator.tileSize);

        return pos.X >= xLayer * MapGenerator.tileSize && pos.X <= xLayer * MapGenerator.tileSize + MapGenerator.tileSize;
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