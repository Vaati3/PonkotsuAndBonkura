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
	protected Sprite2D sprite;

	public override void _Ready()
	{
        gameManager = GetNode<GameManager>("/root/GameManager");
		sprite = GetNode<Sprite2D>("Sprite");
	}

    public void init(Map map, Character other)
    {
		this.map = map;
		this.other = other;
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
		Velocity = GetLocalPos(dir);
		MoveAndSlide();
		position3D = GetGlobalPos(Position, dir);
		UpdateVisibility();
		map.UpdateObjects();

		RpcId(gameManager.otherPlayer.id, nameof(UpdatePosition), position3D);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdatePosition(Vector3 pos)
	{
		position3D = pos;
		Position = other.GetLocalPos(pos);
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

	protected bool IsFalling()
	{
		float half = sprite.Texture.GetSize().Y * sprite.Scale.Y / 2f + 1;
        if (map.generator.GetTile(position3D.X, position3D.Y + half, position3D.Z) != Tile.Block)
			return true;
		return false;
	}
	protected virtual bool UpdateTile(Vector3I tilePos, int x, int y)
	{
		Tile tile = map.generator.GetTile(tilePos);
		switch (tile)
		{
			case Tile.Void:
				map.SetTile(x, y, Vector2I.One);
				return true;
			case Tile.PonkotsuGoal: case Tile.BonkuraGoal: 
				map.SetTile(x, y, new Vector2I((int)tile - 1, 0));
				return true;
		}
		return false;
	}

	public abstract CharacterType GetCharacterType();
	public abstract Vector2 GetLocalPos(Vector3 pos);
	public abstract Vector3 GetGlobalPos(float x, float y);
	public abstract Vector3 GetGlobalPos(Vector2 pos, Vector3 dir);
	public abstract bool CanSee(Vector3 pos);
	protected abstract void UpdateMap();
}
