using Godot;
using System;

public enum CharacterType {
	Ponkotsu,
	Bonkura
}

public abstract partial class Character : CharacterBody2D
{
	protected GameManager gameManager;
	protected Map map;
	protected bool isControlled = false;
	protected Character other;
	public Vector3 position3D = Vector3.Zero;
	[Export]public float speed = 100;

	private Sprite2D sprite;

    public void init(Map map, Character other)
    {
		this.map = map;
		this.other = other;
        gameManager = GetNode<GameManager>("/root/GameManager");
		sprite = GetNode<Sprite2D>("Sprite");
    }

    public void Possess(Vector3[] spawns)
	{
		isControlled = true;

		position3D = spawns[(int)GetCharacterType()];
		Position = GetLocalPos(position3D);
		other.position3D = spawns[(int)other.GetCharacterType()];
		other.Position = GetLocalPos(other.position3D);
		UpdateMap();
		UpdateVisibility();
	}

	protected void Move(Vector3 dir)
	{
        MoveAndCollide(GetLocalPos(dir));
		position3D = GetGlobalPos(Position, dir);
		UpdateVisibility();

		RpcId(gameManager.otherPlayer.id, nameof(UpdatePosition), position3D);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdatePosition(Vector3 pos)
	{
		position3D = pos;
		Position = other.GetLocalPos(pos);
		//MoveAndCollide(other.GetLocalPos(dir));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdateVisibility()
	{
		if (!isControlled)
			return;
		other.Visible = CanSee(other.position3D);
		other.UpdateVisibility(gameManager.otherPlayer.id);
	}
	protected void UpdateVisibility(long id)
	{
		RpcId(id, nameof(UpdateVisibility));
	}

	public abstract CharacterType GetCharacterType();
	public abstract Vector2 GetLocalPos(Vector3 pos);
	public abstract Vector3 GetGlobalPos(float x, float y);
	public abstract Vector3 GetGlobalPos(Vector2 pos, Vector3 dir);
	protected abstract bool CanSee(Vector3 pos);
	protected abstract void UpdateMap();
}
