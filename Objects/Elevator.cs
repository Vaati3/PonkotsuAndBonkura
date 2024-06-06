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

        UpdateSprite("res://Objects/Textures/Elevator.png", "");
    }

    public void Setup(Axis axis, List<Vector3I> stops, MapGenerator generator)
    {
        foreach(Vector3I stop in stops)
        {
            this.stops.Add(Map.AlignPos(stop));
            generator.SetTile(Tile.Void, stop);
        }
        this.axis = axis;
        forward = axis == Axis.X ? Vector3.Right : axis == Axis.Y ? Vector3.Up : Vector3.Back;
        direction = GetAxisValue(position3D) > GetAxisValue(this.stops[nextStop]) ? -1 : 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isMoving && pauseTimer.TimeLeft == 0)
        {
            if ((direction > 0 && GetAxisValue(position3D) >= GetAxisValue(stops[nextStop])) ||
                (direction < 0 && GetAxisValue(position3D) <= GetAxisValue(stops[nextStop])))
            {
                nextStop += direction;
                if (nextStop + direction >= stops.Count || nextStop + direction <= 0)
                    direction *= -1;
                pauseTimer.Start();
            }
            if (playerOverlap)
                player.Move(forward * direction * speed * (float)delta);
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
