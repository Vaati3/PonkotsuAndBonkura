using Godot;

public partial class PressurePlate : Object
{
    public bool pressed {get; private set;}
    [Signal] public delegate void ButtonPressedEventHandler(bool state);

    Texture2D texture;
    Texture2D texturePressed;

    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);

        SetTexture("res://Objects/Textures/ButtonTopPressed.png", "res://Objects/Textures/ButtonSidePressed.png");
        texturePressed = sprite.Texture;
        SetTexture("res://Objects/Textures/ButtonTop.png", "res://Objects/Textures/ButtonSide.png");
        texture = sprite.Texture;   
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        if (pressed)
            return;
        Rpc(nameof(UpdatePressed), true);
    }

    protected override void OverlapEnded()
    {
        base.OverlapEnded();
        if (!pressed)
            return;
        Rpc(nameof(UpdatePressed), false);
    }

    public override void Trigger(bool state)
    {
        throw new System.NotImplementedException();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void UpdatePressed(bool state)
    {
        pressed = state;
        EmitSignal(nameof(ButtonPressed), state);
        if (pressed)
            sprite.Texture = texturePressed;
        else
            sprite.Texture = texture;
    }
}