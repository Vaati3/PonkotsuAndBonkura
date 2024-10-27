using System;
using Godot;

public partial class Cube : MeshInstance3D
{
    public Vector3I pos {get; private set;}

    [Signal]public delegate void CubeClickedEventHandler(Cube cube);
    public Cube(Vector3I pos, Vector3 mapSize, Material material, CubeClickedEventHandler cubeClicked)
    {
        this.pos = pos;
        CubeClicked += cubeClicked;
        Mesh = new BoxMesh();
        Position = new Vector3(pos.X, mapSize.Y - pos.Y, pos.Z);
        MaterialOverride = material;

        Area3D area = new Area3D();
        area.AddChild(new CollisionShape3D(){
            Shape= new BoxShape3D()
        });
        area.InputEvent += InputEvent;
        AddChild(area);
    }

    public void Change(Material material)
    {
        MaterialOverride = material;
    }

    private void InputEvent(Node camera, InputEvent @event, Vector3 pos, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && @event.IsPressed() == true)
            {
                EmitSignal(nameof(CubeClicked), this);
            }
        }
    }
}