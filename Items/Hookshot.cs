using System.Collections.Generic;
using Godot;

public partial class Hookshot : Item
{    
    const int lenght = 500;

    public Hookshot() : base()
    {
    }
    public override CharacterType GetUserType()
    {
        return CharacterType.Ponkotsu; 
    }
    public override ItemType GetItemType()
    {
        return ItemType.Hookshot;
    }
    public override void Use()
    {
        // GameManager gameManager = GetNode<GameManager>("/root/GameManager");
        // Vector2 target = owner.GetLocalPos(owner.direction).Normalized() * lenght;
        // PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        // PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(Vector2.Zero, target);
        // Godot.Collections.Dictionary result = spaceState.IntersectRay(query);

        // if (result["collider"].Obj is TileMap)
        // {
        //     Vector2 pos = (Vector2)result["position"].Obj;
        //     Vector3 dir = owner.position3D - owner.GetGlobalPos(pos.X, pos.Y);
        //     GD.Print(gameManager.map.generator.GetTile(owner.GetGlobalPos(pos.X, pos.Y)));
        //     owner.Move(dir);
        // }
    }
}