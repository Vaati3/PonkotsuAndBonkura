using Godot;
using System;

public enum CharacterType {
	Ponkotsu,
	Bonkura
}

public abstract partial class Character : Node2D
{
	protected GameManager gameManager;
	protected Vector3 position3D = Vector3.Zero;
	protected long controllerId;
	protected Character other;
	[Export]public float speed = 100;

	[Signal]public delegate void UpdateMapEventHandler(Vector3 pos);

	public void Possess(long id, Character other)
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		controllerId = id;
		this.other = other;

		UpdateVisibility();
		EmitSignal(nameof(UpdateMap), position3D, (int)GetCharacterType());
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
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

	public abstract CharacterType GetCharacterType();
	protected abstract bool CanSee(Vector3 pos);
}
