using Godot;
using System;

public enum CharacterType {
	Ponkotsu,
	Bonkura
}

public abstract partial class Character : Node2D
{
	protected GameManager gameManager;
	protected Map map;
	protected long controllerId = -1;
	protected Character other;
	public Vector3 position3D = Vector3.Zero;
	[Export]public float speed = 100;

    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("/root/GameManager");
    }

    public void Possess(long id, Character other, Map map)
	{
		this.map = map;
		controllerId = id;
		this.other = other;

		UpdateMap();
		Rpc(nameof(UpdatePosition), Vector3.Zero);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	public virtual void UpdatePosition(Vector3 dir)
	{
		//check colision
		position3D += dir;
		UpdateVisibility();
	}

	protected void UpdateVisibility()
	{
		if (gameManager.player.id != controllerId)
			return;
		other.Visible = CanSee(other.position3D);
	}

	public abstract CharacterType GetCharacterType();
	protected abstract bool CanSee(Vector3 pos);
	protected abstract void UpdateMap();
}
