using Godot;

public partial class PressurePlate : Object
{
    public bool pressed {get; private set;}
    [Signal] public delegate void ButtonPressedEventHandler(bool state);

    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);

        UpdateSprite("", "res://Objects/Textures/Elevator.png");
    }

    protected override void OverlapStarted()
    {
        if (pressed)
            return;
        Rpc(nameof(UpdatePressed), true);
    }

    protected override void OverlapEnded()
    {
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
    }
}