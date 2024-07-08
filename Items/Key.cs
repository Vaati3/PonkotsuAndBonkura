using System;
using Godot;

public partial class Key : Item
{
    Map map = null;
    
    const int range = 100;

    public override void Equip(Character character)
    {
        base.Equip(character);
        if (this.map == null && GetParent().GetParent() is Map map)
            this.map = map;
    }

    public override ItemType GetItemType()
    {
        return ItemType.Key;
    }
    public override CharacterType GetUserType()
    {
        return CharacterType.Both;
    }
    public override void Use()
    {
        Door nearestDoor = null;
        float nearestDist = 0;

        foreach(Object obj in map.objects)
        {
            if (obj is Door door)
            {
                float dist = Math.Abs(obj.position3D.X - owner.pos.globalPos.X + obj.position3D.Y - owner.pos.globalPos.Y + obj.position3D.Z - owner.pos.globalPos.Z);
                if (nearestDoor == null || dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestDoor = door;
                }
            }
        }
        if (nearestDist <= range)
        {
            Rpc(nameof(OpenDoor), nearestDoor, owner);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void OpenDoor(Door door, Character keyOwner)
    {
        door.Open(map);
        owner.RemoveItem();
        QueueFree();
    }
}