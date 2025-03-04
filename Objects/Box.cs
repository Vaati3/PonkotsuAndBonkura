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

    protected bool IsFalling()
	{
        float half = MapGenerator.tileSize/2;
		float y = position3D.Y + half;
        if (map.generator.GetTile(position3D.X - half, y, position3D.Z - half) != Tile.Block && 
            map.generator.GetTile(position3D.X - half, y, position3D.Z + half) != Tile.Block &&
            map.generator.GetTile(position3D.X + half, y, position3D.Z - half) != Tile.Block &&
            map.generator.GetTile(position3D.X + half, y, position3D.Z + half) != Tile.Block)
			return true;
		return false;
	}

    public override void _PhysicsProcess(double delta)
    {
        if(!player.CanSee(position3D))
            return;
        if (overlappingPlayers[(int)player.GetCharacterType()] != null
            && player.pos.globalPos.Y > position3D.Y)
        {
            Move(GetDirection(player.Position, player.Velocity, player.size, (float)delta));
        }
    }

    protected override void UpdateVisibility()
    {
        Visible = !hide && player.CanSee(position3D);
        if (collisionShape != null)
            collisionShape.Disabled = !Visible;
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        if (player.GetCharacterType() == CharacterType.Ponkotsu && player.pos.globalPos.Y < position3D.Y)
        {
            player.canFall = false;
        }
    }

    protected override void OverlapEnded()
    {
        base.OverlapEnded();
        if(!player.canFall)
            player.canFall = true;
    }

    private Vector3 GetDirection(Vector2 pos, Vector2 velocity, Vector2 size, float delta)
    {
        size /= 2;
        if (velocity.Y == 0 && ((pos.X > Position.X && velocity.X < 0) || (pos.X < Position.X && velocity.X > 0)))
        {
            return new Vector3(player.Velocity.X * delta, 0, 0);
        }
        if (player.GetCharacterType() == CharacterType.Bonkura)
            return Vector3.Zero;
        if (velocity.X == 0 && ((pos.Y > Position.Y && velocity.Y < 0) || (pos.Y < Position.Y && velocity.Y > 0)))
        {
            return new Vector3(0, 0,player.Velocity.Y * delta);
        }
        return Vector3.Zero;
    }

    private void Move(Vector3 dir)
    {
        if (dir == Vector3.Zero)
            return;
        Vector3 newPos = position3D + dir;
        float half = MapGenerator.tileSize / 2 - 1;
        if (map.generator.GetTile(newPos.X - half, newPos.Y, newPos.Z - half) != Tile.Void || 
            map.generator.GetTile(newPos.X - half, newPos.Y, newPos.Z + half) != Tile.Void ||
            map.generator.GetTile(newPos.X + half, newPos.Y, newPos.Z - half) != Tile.Void ||
            map.generator.GetTile(newPos.X + half, newPos.Y, newPos.Z + half) != Tile.Void)
        {
            return;
        }
        if (IsFalling())
        {
            Vector3I pos = MapGenerator.GetTilePos(newPos);
            while (map.generator.GetTile(pos) != Tile.Block)
                pos.Y++;
            newPos.Y = MapGenerator.GetWorldPos(pos).Y - half;
        }
        Rpc(nameof(UpdatePosition), newPos - position3D);
        position3D = newPos;
        Update();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdatePosition(Vector3 dir)
    {
        position3D += dir;

        if (overlappingPlayers[(int)player.GetCharacterType()] != null && player.pos.globalPos.Y < position3D.Y)
        {
            player.SetPos(player.pos.globalPos + dir);
        }
    }
}
