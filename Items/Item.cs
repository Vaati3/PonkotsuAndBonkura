using Godot;

public enum ItemType {
    Key,
    Hookshot
}

public abstract partial class Item : Node2D
{
    Sprite2D sprite;
    protected Character owner = null;

    public Item()
    {
        sprite = new Sprite2D()
        {
            Texture = GD.Load<Texture2D>("res://Items/Textures/" + GetItemType().ToString() +  ".png"),
        };
        AddChild(sprite);
        Unequip();
    }

    public void Unequip()
    {
        Position = new Vector2(-35, 10);
        Scale = new Vector2(2, 2);
    }

    public virtual void Equip(Character character)
    {
        owner = character;
        Position = new Vector2(0, 24);
        Scale = Vector2.One;
    }

    public void Flip(bool state)
    {
        sprite.FlipH = state;
    }

    public abstract ItemType GetItemType();
    public abstract CharacterType GetUserType();
    public abstract void Use();

}
