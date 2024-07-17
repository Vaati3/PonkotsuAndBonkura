using Godot;

public partial class Box : Object
{
    const int speed = 20;
    const int range = 5;
    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        SetTexture("res://Objects/Textures/Box.png", "res://Objects/Textures/Box.png");

        overlapSize = new Vector3(MapGenerator.tileSize + range * 2, MapGenerator.tileSize, MapGenerator.tileSize + range * 2);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!player.CanSee(position3D))
            return;
        if (overlappingPlayers[(int)player.GetCharacterType()] != null)
            Move(GetDirection(player.Position, player.pos.GlobalToLocal(player.direction), player.size, (float)delta));
    }

    private Vector2 GetDirection(Vector2 pos, Vector2 dir, Vector2 size, float delta)
    {
        size /= 2;
        // float halfTile = MapGenerator.tileSize/2;
        if (player.GetCharacterType() == CharacterType.Ponkotsu && pos.X + size.X > Position.X && pos.X + size.X < Position.X + MapGenerator.tileSize && dir.X == 0)
        {
            if (pos.Y + size.Y < Position.Y && dir.Y > 0)
                return new Vector2(0, player.Velocity.Y * delta);
            if (pos.Y + size.Y > Position.Y + MapGenerator.tileSize && dir.Y < 0 )
                return new Vector2(0, -(player.Velocity.Y * delta));
        }
        if (pos.Y + size.Y > Position.Y && pos.Y + size.Y < Position.Y + MapGenerator.tileSize && dir.Y == 0)
        {
            if (pos.X + size.X < Position.X && dir.X > 0)
                return new Vector2(player.Velocity.X * delta, 0);
            if (pos.X + size.X > Position.X + MapGenerator.tileSize && dir.X < 0)
                return new Vector2(-(player.Velocity.X * delta), 0);
        }
        return Vector2.Zero;
    }

    private void Move(Vector2 dir)
    {
        position3D = player.pos.LocalToGlobal(Position + dir);

        Rpc(nameof(UpdatePosition), position3D);
        Update();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdatePosition(Vector3 newPos)
    {
        position3D = newPos;
        Position = player.other.pos.GlobalToLocal(newPos);
    }
}
