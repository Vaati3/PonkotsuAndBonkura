using Godot;

public partial class Box : RigidBody2D, IObject
{
        BoxObject boxObject;
    
    static public Box NTM(Character character, Vector3I pos)
    { 
        BoxObject test = Object.CreateObject<BoxObject>(character, pos);
        Vector2 ok = test.Position;
        test.Position = Vector2.Zero;
        Box box = new Box
        {
            Position = ok,
            boxObject = test,
            GravityScale = 0,
            LinearDamp = 100,
            LockRotation = true,
            CollisionLayer = 1
        };
        box.AddChild(box.boxObject);
        CollisionShape2D collisionShape = new CollisionShape2D()
        {
            Shape = new RectangleShape2D(){
                Size = new Vector2(100, 100)
            }
        };
        box.AddChild(collisionShape);
        return box;
    }

    // public Box(Character character, Vector3I pos)
    // {
    //     
    //     //AddChild(boxObject);
    // }

    public IObject.FreeObjectEventHandler FreeObject { get; set; }

    public Node GetNode()
    {
        return this;
    }

    public bool IsOverlapping()
    {
        return boxObject.IsOverlapping();
    }

    public void Switch(Character character)
    {
        boxObject.Switch(character);
    }

    public void Update()
    {
        GD.Print(boxObject.Position);
        boxObject.Update();
    }
}

public partial class BoxObject : Object
{
    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        SetTexture("res://Objects/Textures/Box.png", "res://Objects/Textures/Box.png");
        overlapOffset.Y -= 100;
        updatePos = false;
    }
}
