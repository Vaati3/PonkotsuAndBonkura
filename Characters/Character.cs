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
	public Position pos {get; protected set;}
	public Vector3 direction {get; private set;}
	[Export]public float speed = 100;
	protected Sprite2D sprite;

	public bool canFall{get; set;}
	private Item item = null;

	public override void _Ready()
	{
		pos = new Position(GetCharacterType());
		direction = Vector3.Zero;
		canFall = true;
		gameManager = GetNode<GameManager>("/root/GameManager");
		sprite = GetNode<Sprite2D>("Sprite");
	}

	public override void _Input(InputEvent @event)
	{
		if (!isControlled)
			return;
		if (!map.gameMenu.Visible && @event.IsActionPressed("pause") && !@event.IsEcho())
			map.gameMenu.Pause();
		if (@event.IsActionPressed("use_item") && !@event.IsEcho())
		{
			map.gameMenu.MapCompleted();
			item?.Use();
		}
		if (map.switchCooldown.IsStopped() && gameManager.isAlone && @event.IsActionPressed("switch") && !@event.IsEcho())
		{
			map.switchCooldown.Start();
			UnPossess();
			other.Possess();
			gameManager.player.characterType = other.GetCharacterType();
			map.SwitchObject(other);
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

		pos.globalPos = spawns[(int)GetCharacterType()];
		Position = pos.GetLocalPos();
		other.pos.globalPos = spawns[(int)other.GetCharacterType()];
		other.Position = pos.GlobalToLocal(other.pos.globalPos);
		UpdateMap();
		UpdateVisibility();

		GetNode<Camera2D>("Camera").Enabled = true;
	}

	public void Possess()
	{
		isControlled = true;
		Position = pos.GetLocalPos();
		other.Position = pos.GlobalToLocal(other.pos.globalPos);
		UpdateMap();
		UpdateVisibility();

		GetNode<Camera2D>("Camera").Enabled = true;
	}

	public void UnPossess()
	{
		isControlled = false;
		GetNode<Camera2D>("Camera").Enabled = false;
	}

	public virtual void Move(Vector3 dir)
	{
		if (gameManager.isAlone && !isControlled)
		{
			pos.globalPos += dir;
			return;
		}
		direction = dir;
		Velocity = pos.GlobalToLocal(dir);
		MoveAndSlide();
		pos.Move(Position, dir);
		UpdateVisibility();
		map.UpdateObjects();

		Rpc(nameof(UpdatePosition), pos.globalPos);
		
		if (isControlled && map.generator.GetTile(pos.globalPos) == (Tile)((int)Tile.PonkotsuGoal + GetCharacterType()) &&
			map.generator.GetTile(other.pos.globalPos) == (Tile)((int)Tile.PonkotsuGoal + other.GetCharacterType()))
			map.gameMenu.MapCompleted();
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
	protected void UpdatePosition(Vector3 newPos)
	{
		pos.globalPos = newPos;
		Position = other.pos.GlobalToLocal(newPos);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdateVisibility()
	{
		Visible = true;
		if (!isControlled)
			return;
		other.Visible = CanSee(other.pos.globalPos);
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
        if (map.generator.GetTile(pos.globalPos.X, pos.globalPos.Y + half, pos.globalPos.Z) != Tile.Block)
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
	public abstract bool CanSee(Vector3 otherPos);
	protected abstract void UpdateMap();
}
