using Godot;

public partial class Key : Item
{
    Map map;
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
        GD.Print("You try to use the key. It does nothing");
    }
}