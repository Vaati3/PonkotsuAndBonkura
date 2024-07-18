using Godot;

public partial class Box : Object
{
    const int speed = 20;
    const int range = 5;

    Map map;

    StaticBody2D collisionBody;
    CollisionShape2D collisionShape;
    public override void InitObject(Character player, Vector3 pos, Map map)
    {
        base.InitObject(player, pos, map);
        SetTexture("res://Objects/Textures/Box.png", "res://Objects/Textures/Box.png");

        overlapSize = new Vector3(MapGenerator.tileSize + range * 2, MapGenerator.tileSize + range * 2, MapGenerator.tileSize + range * 2);
        this.map = map;
        collisionBody = new StaticBody2D()
        {
            Position = Vector2.Zero,
            CollisionLayer = 2
        };
        AddChild(collisionBody);
        collisionShape = new CollisionShape2D()
        {
            Shape = new RectangleShape2D(){
                Size = new Vector2(MapGenerator.tileSize, MapGenerator.tileSize)
            }
        };
        collisionBody.AddChild(collisionShape);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!player.CanSee(position3D))
            return;
        if (overlappingPlayers[(int)player.GetCharacterType()] != null)
            Move(GetDirection(player.Position, player.Velocity, player.size, (float)delta));
    }

    private Vector2 GetDirection(Vector2 pos, Vector2 velocity, Vector2 size, float delta)
    {
        size /= 2;
        if (velocity.X == 0 && ((pos.Y > Position.Y && velocity.Y < 0) || (pos.Y < Position.Y && velocity.Y > 0)))
        {
            return new Vector2(0, player.Velocity.Y * delta);
        }
        if (velocity.Y == 0 && ((pos.X > Position.X && velocity.X < 0) || (pos.X < Position.X && velocity.X > 0)))
        {
            return new Vector2(player.Velocity.X * delta, 0);
        }
        return Vector2.Zero;
    }

    private void Move(Vector2 dir)
    {
        Vector3 newPos = player.pos.LocalToGlobal(Position + dir);
        float half = MapGenerator.tileSize / 2;
        if (map.generator.GetTile(newPos.X - half, newPos.Y, newPos.Z - half) != Tile.Void || 
            map.generator.GetTile(newPos.X - half, newPos.Y, newPos.Z + half) != Tile.Void ||
            map.generator.GetTile(newPos.X + half, newPos.Y, newPos.Z - half) != Tile.Void ||
            map.generator.GetTile(newPos.X + half, newPos.Y, newPos.Z + half) != Tile.Void)
            {
                player.Velocity = Vector2.Zero;
                return;
            }
        position3D = newPos;

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
