using Godot;
using System;
using System.Collections.Generic;

public partial class Elevator : Object
{
    const float speed = 25;
    List<Vector3> stops;
    int nextStop = 1;
    bool isMoving = false;
    bool turnArround = false;
    Vector3 forward;
    int direction;
    Axis axis;
    Timer pauseTimer;

    static public Elevator CreateElevator(Map map, Character character, Vector3I pos, Tile tile)
    {
        List<Vector3I> stops = map.generator.Search(Tile.ElevatorStop, (Axis)((int)tile-(int)Tile.ElevatorX), pos);
		if (stops.Count != 0)
		{
			Elevator elevator = CreateObject<Elevator>(character, pos);
            foreach(Vector3I stop in stops)
            {
                elevator.stops.Add(Map.AlignPos(stop));
                map.generator.SetTile(Tile.Void, stop);
            }
            Axis axis = (Axis)((int)tile-(int)Tile.ElevatorX);
            elevator.axis = axis;
            elevator.forward = axis == Axis.X ? Vector3.Right : axis == Axis.Y ? Vector3.Up : Vector3.Back;
            elevator.direction = elevator.GetAxisValue(elevator.position3D) > elevator.GetAxisValue(elevator.stops[elevator.nextStop]) ? -1 : 1;
            return elevator;
		}
        return null;
    }

    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        stops = new List<Vector3>
        {
            position3D
        };
        pauseTimer = new Timer
        {
            Autostart = false,
            WaitTime = 3,
            OneShot = true
        };
        AddChild(pauseTimer);

        SetTexture("res://Objects/Textures/ElevatorTop.png", "res://Objects/Textures/ElevatorSide.png");
        activatable = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isMoving && pauseTimer.IsStopped())
        {
            if ((direction > 0 && GetAxisValue(position3D) >= GetAxisValue(stops[nextStop])) ||
                (direction < 0 && GetAxisValue(position3D) <= GetAxisValue(stops[nextStop])))
            {
                nextStop += direction;
                if (nextStop + direction >= stops.Count || nextStop + direction <= 0)
                    direction *= -1;
                pauseTimer.Start();
            }
            overlappingPlayers[0]?.Move(forward * direction * speed * (float)delta);
            overlappingPlayers[1]?.Move(forward * direction * speed * (float)delta);
            position3D += forward * direction * speed * (float)delta;
            Update();
        }
    }

    private float GetAxisValue(Vector3 pos)
    {
        return axis == Axis.X ? pos.X : axis == Axis.Y ? pos.Y : pos.Z;
    }
    private float SetAxisValue(Vector3 pos)
    {
        return axis == Axis.X ? pos.X : axis == Axis.Y ? pos.Y : pos.Z;
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        player.canFall = false;
    }

    protected override void OverlapEnded()
    {
        base.OverlapEnded();
        player.canFall = true;
    }

    public override void Trigger(bool state)
    {
        isMoving = state;
    }
}
