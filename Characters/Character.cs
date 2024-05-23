using Godot;
using System;

public abstract partial class Character : Node2D
{
	protected GameManager gameManager;
	protected Vector3 position3D = Vector3.Zero;
	protected int controllerId;
	protected Character other;
	[Export]public float speed = 100;

	public void Possess(int id, Character other)
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		controllerId = id;
		this.other = other;

		UpdateVisibility();
		UpdateMap();
	}

	[Rpc(mode:MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	protected void UpdatePosition(Vector3 dir)
	{
		//check colision
		position3D += dir;
		UpdateVisibility();
	}

	protected void UpdateVisibility()
	{
		if (gameManager.player.id != controllerId)
			return;
		if (CanSee(other.position3D))
			other.Visible = true;
		else
			other.Visible = false;
	}

	protected abstract bool CanSee(Vector3 pos);
	protected abstract void UpdateMap();
}
