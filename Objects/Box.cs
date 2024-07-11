using Godot;

public partial class Box : Object
{
    CharacterBody2D collisionBody = null;
    const int speed = 20;
    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        SetTexture("res://Objects/Textures/Box.png", "res://Objects/Textures/Box.png");
        updatePos = false;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            collisionBody.QueueFree();
    }

    public override void _EnterTree()
    {
        if (collisionBody == null)
        {
            collisionBody = new CharacterBody2D
            {
                CollisionLayer = 2,
                Position = player.pos.GlobalToLocal(position3D)
            };
            GetParent().AddChild(collisionBody);

            RemoteTransform2D remoteTransform = new RemoteTransform2D() {
                RemotePath = GetPath()
            };
            collisionBody.AddChild(remoteTransform);

            CollisionShape2D collisionShape = new CollisionShape2D()
            {
                Shape = new RectangleShape2D(){
                    Size = new Vector2(100, 100)
                }
            };
            collisionBody.AddChild(collisionShape);
        }
    }

    public override void Update()
    {
        base.Update();
        if (collisionBody != null)
            collisionBody.CollisionLayer = Visible ? 2 : 0u;
    }

    public override void Switch(Character character)
    {
        collisionBody.Position = character.pos.GlobalToLocal(position3D);
        base.Switch(character);
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!player.CanSee(position3D))
            return;
        if(player.GetLastSlideCollision() != null && player.GetLastSlideCollision().GetCollider() == collisionBody)
        {
            Move(GetDirection(player.Position, player.pos.GlobalToLocal(player.direction), player.GetSize()) * (float)delta);
        }
    }

    private Vector2 GetDirection(Vector2 pos, Vector2 dir, Vector2 size)
    {
        size /= 2;
        if (player.GetCharacterType() == CharacterType.Ponkotsu && pos.X + size.X > Position.X && pos.X + size.X < Position.X + MapGenerator.tileSize && dir.X == 0)
        {
            if (pos.Y + size.Y < Position.Y && dir.Y > 0)
                return new Vector2(0, speed);
            if (pos.Y + size.Y > Position.Y + MapGenerator.tileSize && dir.Y < 0 )
                return new Vector2(0, -speed);
        }
        if (pos.Y + size.Y > Position.Y && pos.Y + size.Y < Position.Y + MapGenerator.tileSize && dir.Y == 0)
        {
            if (pos.X + size.X < Position.X && dir.X > 0)
                return new Vector2(speed, 0);
            if (pos.X + size.X > Position.X + MapGenerator.tileSize && dir.X < 0)
                return new Vector2(-speed, 0);
        }
        return Vector2.Zero;
    }

    private void Move(Vector2 dir)
    {
        collisionBody.MoveAndCollide(dir);
        position3D = player.pos.LocalToGlobal(Position);

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
