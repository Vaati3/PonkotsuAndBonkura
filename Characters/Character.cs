using Godot;
using System;

public enum CharacterType {
	Ponkotsu,
	Bonkura,
	Both
}

public abstract partial class Character : CharacterBody2D
{
	protected GameManager gameManager;
	protected Map map;
	protected bool isControlled = false;
	public Character other {get; private set;}
	public Vector3 position3D {get; private set;}
	public Vector3 direction {get; private set;}
	[Export]public float speed = 100;
	protected Sprite2D sprite;

	public bool canFall{get; set;}
	private Item item = null;

	public override void _Ready()
	{
		position3D = Vector3.Zero;
		direction = Vector3.Zero;
		canFall = true;
		gameManager = GetNode<GameManager>("/root/GameManager");
		sprite = GetNode<Sprite2D>("Sprite");

		
	}

	public override void _Input(InputEvent @event)
	{
		if (!isControlled)
			return;
		if (@event.IsActionPressed("use_item") && !@event.IsEcho())
		{
			item?.Use();
		}
		GD.Print(map.switchCooldown.TimeLeft);
		if (map.switchCooldown.IsStopped() && gameManager.isAlone && @event.IsActionPressed("switch") && !@event.IsEcho())
		{
			map.switchCooldown.Start();
			UnPossess();
			other.Possess();
		}
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

		GetNode<Camera2D>("Camera").Enabled = true;
	}

	public void Possess()
	{
		isControlled = true;
		Position = GetLocalPos(position3D);
		other.Position = GetLocalPos(other.position3D);
		UpdateMap();
		UpdateVisibility();

		GetNode<Camera2D>("Camera").Enabled = true;
	}

	public void UnPossess()
	{
		GD.Print(GetCharacterType());
		isControlled = false;
		GetNode<Camera2D>("Camera").Enabled = false;
	}

	public virtual void Move(Vector3 dir)
	{
		direction = dir;
		Velocity = GetLocalPos(dir);
		MoveAndSlide();
		position3D = GetGlobalPos(Position, dir);
		UpdateVisibility();
		map.UpdateObjects();

		Rpc(nameof(UpdatePosition), position3D);
		
		if (isControlled && map.generator.GetTile(position3D) == (Tile)((int)Tile.PonkotsuGoal + GetCharacterType()) &&
			map.generator.GetTile(other.position3D) == (Tile)((int)Tile.PonkotsuGoal + other.GetCharacterType()))
			map.MapCompleted();
	}

	public Item SwitchItem(Item newItem)
	{
		if (item != null)
			RemoveChild(item);
		Item oldItem = item;
		item = newItem;
		AddChild(item);
		return oldItem;
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
		Visible = true;
		if (!isControlled)
			return;
		other.Visible = CanSee(other.position3D);
		if (!gameManager.isAlone)
			other.UpdateVisibility(gameManager.otherPlayer.id);
	}
	protected void UpdateVisibility(long id)
	{
		RpcId(id, nameof(UpdateVisibility));
	}

	protected bool IsFalling()
	{
		if (!canFall)
			return false;
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
				map.SetTile(x, y, new Vector2I(4, (int)tile - 4));
				return true;
		}
		return false;
	}

	protected void Flip(bool state)
	{
		sprite.FlipH = state;
		item?.Flip(state);
	}

	public abstract CharacterType GetCharacterType();
	public abstract Vector2 GetLocalPos(Vector3 pos);
	public abstract Vector3 GetGlobalPos(float x, float y);
	public abstract Vector3 GetGlobalPos(Vector2 pos, Vector3 dir);
	public abstract bool CanSee(Vector3 pos);
	protected abstract void UpdateMap();
}
