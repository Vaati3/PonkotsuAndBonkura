using System;
using Godot;

public partial class Cube : MeshInstance3D
{
    readonly Color hoverColor;
    public Vector3I pos {get; private set;}

    Vector3 mousePos;

    [Signal]public delegate void CubeClickedEventHandler(Cube cube, Vector3I addPos);
    [Signal]public delegate void CubeHoverEventHandler(Cube cube, Vector3I addPos, bool exit);
    public Cube(Vector3I pos, Vector3 mapSize, Material material, CubeClickedEventHandler cubeClicked, CubeHoverEventHandler cubeHover)
    {
        CubeClicked += cubeClicked;
        CubeHover += cubeHover;
        this.pos = pos;
        Mesh = new BoxMesh();
        Position = new Vector3(pos.X, mapSize.Y - pos.Y, pos.Z);
        MaterialOverride = (Material)material.Duplicate();

        hoverColor = new Color(1, 0, 0, 1);

        Area3D area = new Area3D();
        area.AddChild(new CollisionShape3D(){
            Shape= new BoxShape3D()
        });
        area.InputEvent += InputEvent;
        area.MouseEntered += MouseEnteredEvent;
        area.MouseExited += MouseExitedEvent;

        AddChild(area);
    }

    public void Change(Material material)
    {
        MaterialOverride = (Material)material.Duplicate();
    }

    public void Hover()
    {
        ((StandardMaterial3D)MaterialOverride).AlbedoColor = hoverColor;
    }

    private Vector3I CalculateAddPos(Vector3 mousePos)
    {
        Vector3 dist = Position - mousePos;
        Vector3I addPos = pos;
        if (Mathf.Abs(dist.X) > 0.45)
            addPos.X += dist.X > 0 ? -1 : 1;
        else if (Mathf.Abs(dist.Y) > 0.45)
            addPos.Y += dist.Y > 0 ? 1 : -1;
        else
            addPos.Z += dist.Z > 0 ? -1 : 1;

        return addPos;
    }

    private void InputEvent(Node camera, InputEvent @event, Vector3 mousePos, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && @event.IsPressed() == true)
            {
                EmitSignal(nameof(CubeClicked), this, CalculateAddPos(mousePos));
            }
        }
        if (@event is InputEventMouseMotion mouseMotion)
        {
            this.mousePos = mousePos;
        }
    }

    private void MouseEnteredEvent()
    {
        EmitSignal(nameof(CubeHover), this, CalculateAddPos(mousePos), false);
    }
    private void MouseExitedEvent()
    {
        ((StandardMaterial3D)MaterialOverride).AlbedoColor = new Color(1, 1, 1, 1);
    }
}
