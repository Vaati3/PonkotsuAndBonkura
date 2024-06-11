using Godot;
using System;

public partial class ItemPickup : Object
{
    Item item;

    public void CreateItem(ItemType itemType)
    {
        Type type = Type.GetType(nameof(itemType));
        item = (Item)Activator.CreateInstance(type);
        AddChild(item);
    }


    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        PickedUp();
    }


    private void PickedUp()
    {
        if (player.GetCharacterType() != item.GetUserType())
            return;

        RemoveChild(item);
        Item playerItem = player.SwitchItem(item);

        if (playerItem == null)
            QueueFree();
        
        item = playerItem;
        AddChild(item);
    }

}