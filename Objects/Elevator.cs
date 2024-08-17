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

    bool isPlayingAudio = false;

    StaticBody2D collisionBody;
    CollisionShape2D collisionShape;

    public override void InitObject(Character player, Vector3 pos, Map map)
    {
        base.InitObject(player, pos, map);
        Tile tile = map.generator.GetTile(pos);
        List<Vector3I> allStops = map.generator.Search(Tile.ElevatorStop, (Axis)((int)tile-(int)Tile.ElevatorX), MapGenerator.GetTilePos(pos));
        stops = new List<Vector3>
        {
            position3D
        };
        if (allStops.Count != 0)
		{
            foreach(Vector3I stop in allStops)
            {
                stops.Add(Map.AlignPos(stop));
                map.generator.SetTile(Tile.Void, stop);
            }
            axis = (Axis)((int)tile-(int)Tile.ElevatorX);
            forward = axis == Axis.X ? Vector3.Right : axis == Axis.Y ? Vector3.Up : Vector3.Back;
            direction = GetAxisValue(position3D) > GetAxisValue(stops[nextStop]) ? -1 : 1;
		}

        pauseTimer = new Timer
        {
            Autostart = false,
            WaitTime = 3,
            OneShot = true
        };
        AddChild(pauseTimer);

        SetTexture("res://Objects/Textures/ElevatorTop.png", "res://Objects/Textures/ElevatorSide.png");
        activatable = true;

        collisionBody = new StaticBody2D()
        {
            Position = new Vector2(0, (MapGenerator.tileSize-5)/2),
            CollisionLayer = 2
        };
        AddChild(collisionBody);
        collisionShape = new CollisionShape2D()
        {
            Shape = new RectangleShape2D(){
                Size = new Vector2(MapGenerator.tileSize, 5)
            },
            Disabled = player.GetCharacterType() == CharacterType.Ponkotsu
        };
        collisionBody.AddChild(collisionShape);
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

                soundManager.StopSFX("hover");
                isPlayingAudio = false;
            }
            overlappingPlayers[0]?.Move(forward * direction * speed * (float)delta);
            overlappingPlayers[1]?.Move(forward * direction * speed * (float)delta);
            position3D += forward * direction * speed * (float)delta;
            Update();

            if (!isPlayingAudio)
            {
                soundManager.PlaySFX("hover");
                isPlayingAudio = true;
            }
            
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

    public override void Switch(Character character)
    {
        base.Switch(character);
        collisionShape.Disabled = player.GetCharacterType() == CharacterType.Ponkotsu;
    }

    protected override void UpdateVisibility()
    {
        base.UpdateVisibility();
        if (player.GetCharacterType() == CharacterType.Bonkura && collisionShape != null)
        {
            collisionShape.Disabled = !Visible;
        }
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        if (player.GetCharacterType() == CharacterType.Ponkotsu)
            player.canFall = false;
    }

    protected override void OverlapEnded()
    {
        base.OverlapEnded();
        if (player.GetCharacterType() == CharacterType.Ponkotsu)
            player.canFall = true;
    }

    public override void Trigger(bool state)
    {
        isMoving = state;

        if (!isMoving && isPlayingAudio)
        {
            soundManager.StopSFX("hover");
            isPlayingAudio = false;
        }
    }
}
