using Godot;

//only visible to other player

public partial class Rotator : Object
{
    Timer timer;

    public override void InitObject(Character player, Vector3 pos)
    {
        base.InitObject(player, pos);
        activatable = true;
        SetTexture("res://Objects/Textures/Rotator.png", "");
        
        timer = new Timer()
        {
            WaitTime = 4,
            OneShot = true,
            Autostart = false
        };
        AddChild(timer);
    }

    public void RotatePlayer(Character character)
    {
        if (character?.GetCharacterType() == CharacterType.Bonkura)
        {
           ((Bonkura)character).RotateAxis();
           //Rpc(nameof(FlipSprite));
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void FlipSprite()
    {
        sprite.FlipH = !sprite.FlipH;
    }

    public override void Trigger(bool state)
    {
        if (!timer.IsStopped())
            return;
        RotatePlayer(overlappingPlayers[0]);
        RotatePlayer(overlappingPlayers[1]);
        timer.Start();
    }
}