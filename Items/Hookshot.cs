using Godot;

public partial class Hookshot : Item
{
    public override CharacterType GetUserType()
    {
        return CharacterType.Ponkotsu; 
    }
    public override ItemType GetItemType()
    {
        return ItemType.Hookshot;
    }
    public override void Use()
    {
        
    }

}