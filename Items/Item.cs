using Godot;

public enum ItemType {
    none
}

public abstract partial class Item : Node2D
{
    Sprite2D sprite;
    Character owner = null;
    Node2D parent;

    public Item()
    {
        sprite = new Sprite2D()
        {
            Texture = GD.Load<Texture2D>("res://Items/Textures/" + GetItemType() +  ".png")
        };
    } 

    public abstract ItemType GetItemType();//replace with getType
    public abstract CharacterType GetUserType();//replace with getType
    public abstract void Use();

}
