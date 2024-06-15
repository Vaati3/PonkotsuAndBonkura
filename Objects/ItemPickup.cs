using Godot;
using System;

public partial class ItemPickup : Object
{
    Item item;

    public void CreateItem(ItemType itemType)
    {
        Type type = Type.GetType(itemType.ToString());
        item = (Item)Activator.CreateInstance(type);
        AddChild(item);
    }


    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        PickedUp(player);
        Rpc(nameof(UpdatePickup));
    }

    private void PickedUp(Character character)
    {
        if (character.GetCharacterType() != item.GetUserType())
            return;
        
        RemoveChild(item);
        Item playerItem = character.SwitchItem(item);
        item.Equip(character);

        if (playerItem == null)
        {
            EmitSignal(nameof(FreeObject), this);
        } else {
            item = playerItem;
            AddChild(item);
            item.Unequip();
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdatePickup()
    {
        PickedUp(player.other);
    }

}
