using Godot;
using System.Collections.Generic;

public partial class Swapper : Object
{
    public List<Swapper> swappers {get; private set;} = null;
    public Vector3I tilePos {get; private set;}

    Timer timer;

    public override void InitObject(Character player, Vector3 pos, Map map)
    {
        base.InitObject(player, pos, map);

        SetupSwappers(map.objects);
        SetTexture("res://Objects/Textures/Swapper.png", "res://Objects/Textures/Swapper.png");
        tilePos = MapGenerator.GetTilePos(pos);
        timer = new Timer() {
            OneShot = true,
            WaitTime = 5,
            Autostart = false
        };
        AddChild(timer);
    }

    private void SetupSwappers(List<Object> objects)
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
                if (GetNode<GameManager>("/root/GameManager").isAlone)
                    player.other.SetPos(Map.AlignPos(tilePos));
                else
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
