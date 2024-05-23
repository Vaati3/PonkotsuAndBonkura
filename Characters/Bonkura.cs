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
        Rpc(nameof(UpdatePosition), dir);
        Position = new Vector2(position3D.Z, position3D.Y);
    }
    protected override bool CanSee(Vector3 pos)
    {
        int tilesize = 10;
        int xLayer = (int)Math.Floor(position3D.X / tilesize);

        if (pos.X >= xLayer * tilesize && pos.X <= xLayer * tilesize + tilesize)
        {
            other.Position = new Vector2(pos.Z, pos.Y);
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