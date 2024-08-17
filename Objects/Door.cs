using System;
using Godot;

public partial class Door : Object
{
    Vector3I tilePos;

    public override void InitObject(Character player, Vector3 pos, Map map)
    {
        base.InitObject(player, pos, map);
        newTile = Tile.Block;
        tilePos = MapGenerator.GetTilePos(pos);
        SetTexture("res://Objects/Textures/Door.png", "res://Objects/Textures/Door.png");
    }

    public void Open(Map map)
    {
        map.generator.SetTile(Tile.Void, tilePos);
        player.UpdateMap();
        soundManager.PlaySFX("unlock", true);
        EmitSignal(nameof(FreeObject), this);
    }
}