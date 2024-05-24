using Godot;
using System;

public partial class Ponkotsu : Character
{

    public void _physics_process(float delta)
    {
        if (gameManager.player.id != controllerId)
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
        if (dir != Vector3.Zero)
            Rpc(nameof(UpdatePosition), dir);
    }

    public override void UpdatePosition(Vector3 dir)
    {
        base.UpdatePosition(dir);
        if (controllerId == gameManager.player.id)
            Position = new Vector2(position3D.X, position3D.Z);
        else
            Position = new Vector2(position3D.Z, position3D.Y);
        
    }

    public override CharacterType GetCharacterType()
    {
        return CharacterType.Ponkotsu;
    }
    protected override bool CanSee(Vector3 pos)
    {
        int yLayer = (int)(position3D.Y / MapGenerator.tileSize);

        return pos.Y >= yLayer * MapGenerator.tileSize && pos.Y <= yLayer * MapGenerator.tileSize + MapGenerator.tileSize;
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