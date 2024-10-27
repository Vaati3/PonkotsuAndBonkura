using System;
using Godot;

public partial class Cube : MeshInstance3D
{
    MapGenerator map;
    Vector3I pos;
    public Cube(MapGenerator map, Vector3I pos, Material material)
    {
        this.map = map;
        this.pos = pos;
        Mesh = new BoxMesh();
        Position = new Vector3(pos.X, map.size.Y - pos.Y, pos.Z);
        MaterialOverride = material;

        Area3D area = new Area3D();
        area.AddChild(new CollisionShape3D(){
            Shape= new BoxShape3D()
        });
        area.InputEvent += InputEvent;
        AddChild(area);
    }

    private void InputEvent(Node camera, InputEvent @event, Vector3 pos, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && @event.IsPressed() == true)
            {
                QueueFree();
            }
        }
    }
}