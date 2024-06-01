using Godot;
using System;

public partial class Elevator : Object
{
    public Vector3I stop {get;set;}
    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        UpdateSprite("res://Objects/Textures/Elevator.png", "");
    }

}
