using Godot;
using System.Collections.Generic;

public partial class Swapper : Object
{
    public List<Swapper> swappers {get; private set;} = null;
    public Vector3I tilePos {get; private set;}

    Timer timer;

    static public Swapper CreateSwapper(Character character, Vector3I pos, List<IObject> objects)
    {
        Swapper swapper = Object.CreateObject<Swapper>(character, pos);

        swapper.SetupSwappers(objects);
        swapper.SetTexture("res://Objects/Textures/Swapper.png", "res://Objects/Textures/Swapper.png");
        swapper.tilePos = pos;
        swapper.timer = new Timer() {
            OneShot = true,
            WaitTime = 5,
            Autostart = false
        };
        swapper.AddChild(swapper.timer);
        return swapper;
    }

    private void SetupSwappers(List<IObject> objects)
    {
        foreach(Object obj in objects)
        {
            if (obj is Swapper swapper)
            {
                swapper.swappers.Add(this);
                swappers = swapper.swappers;
                break;
            }
        }
        if (swappers == null)
            swappers = new List<Swapper>{this};
    }

    public bool CanSwap()
    {
        if (!timer.IsStopped())
            return false;
        return CheckOverlap(player.other);
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        if (!timer.IsStopped())
            return;

        foreach (Swapper swapper in swappers)
        {
            if (swapper == this)
                continue;
            if (swapper.CanSwap())
            {
                //add swap noise
                //and particles or shader effect
                timer.Start();
                player.SetPos(Map.AlignPos(swapper.tilePos));
                Rpc(nameof(UpdateOther), Map.AlignPos(tilePos));
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdateOther(Vector3 pos)
    {
        timer.Start();
        player.SetPos(pos);
    }
}
