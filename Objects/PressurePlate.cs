using Godot;

public partial class PressurePlate : Object
{
    public bool pressed {get; private set;}
    [Signal] public delegate void ButtonPressedEventHandler(bool state);

    Texture2D topTexturePressed;
    Texture2D sideTexturePressed;

    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);

        SetTexture("res://Objects/Textures/ButtonTop.png", "res://Objects/Textures/ButtonSide.png");
        topTexturePressed = GD.Load<Texture2D>("res://Objects/Textures/ButtonTopPressed.png");
        sideTexturePressed = GD.Load<Texture2D>("res://Objects/Textures/ButtonSidePressed.png"); 
    }

    protected override void UpdateTexture()
    {
        if (pressed)
            UpdateTexture(topTexturePressed, sideTexturePressed);
        else
            base.UpdateTexture();
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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void UpdatePressed(bool state)
    {
        pressed = state;
        EmitSignal(nameof(ButtonPressed), state);
        UpdateTexture();
    }
}