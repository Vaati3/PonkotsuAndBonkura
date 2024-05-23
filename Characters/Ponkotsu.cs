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
        Rpc(nameof(UpdatePosition), dir);
        Position = new Vector2(position3D.X, position3D.Z);
    }
    protected override bool CanSee(Vector3 pos)
    {
        int tilesize = 10;
        int yLayer = (int)Math.Floor(position3D.Y / tilesize);

        if (pos.Y >= yLayer * tilesize && pos.Y <= yLayer * tilesize + tilesize)
        {
            other.Position = new Vector2(pos.X, pos.Z);
            return true;
        }
        return false;
    }

    protected override void UpdateMap()
    {
        // Vector3I tilePos = Vector3I.Zero;
        throw new NotImplementedException();
    }
}