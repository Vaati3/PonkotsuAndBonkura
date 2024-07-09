using Godot;
using System.Collections.Generic;

public partial class Swapper : Object
{
    public List<Swapper> swappers = null;

    static public Swapper CreateSwapper(Character character, Vector3I pos, List<Object> objects)
    {
        Swapper swapper = Object.CreateObject<Swapper>(character, pos);

        swapper.SetupSwappers(objects);
        swapper.SetTexture("res://Objects/Textures/Swapper.png", "res://Objects/Textures/Swapper.png");
        return swapper;
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

    public Character CanSwap()
    {
        if (overlappingPlayers[0] != null && overlappingPlayers[1] != null)
            return null;
        if (overlappingPlayers[0] != null)
            return overlappingPlayers[0];
        if (overlappingPlayers[1] != null)
            return overlappingPlayers[1];
        return null;
    }

    protected override void OverlapStarted()
    {
        base.OverlapStarted();
        foreach (Swapper swapper in swappers)
        {
            if (swapper == this)
                continue;
            Character other = swapper.CanSwap();
            if (other != null)
            {
                Vector3 buf = other.pos.globalPos;
                other.SetPos(player.pos.globalPos);
                player.SetPos(buf);
            }
        }
    }
}
