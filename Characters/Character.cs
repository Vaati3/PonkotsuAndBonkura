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
	private Camera2D camera;

	public bool canFall{get; set;}
	private Item item = null;

	public Vector2 size {get; private set;}

	public override void _Ready()
	{
		pos = new Position(GetCharacterType());
		direction = Vector3.Zero;
		canFall = true;
		gameManager = GetNode<GameManager>("/root/GameManager");
		sprite = GetNode<Sprite2D>("Sprite");
		if (GetNode<CollisionShape2D>("CollisionShape").Shape is RectangleShape2D shape)
			size = shape.Size;
	}

	public override void _Input(InputEvent @event)
	{
		if (!isControlled)
			return;
		if (!map.gameMenu.Visible && @event.IsActionPressed("pause") && !@event.IsEcho())
			map.gameMenu.Pause();
		if (@event.IsActionPressed("use_item") && !@event.IsEcho())
			item?.Use();
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
		camera = map.GetNode<Camera2D>("Camera");
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

		camera.Enabled = true;
		UpdateCamera();
	}

	public void Possess()
	{
		isControlled = true;
		Position = pos.GetLocalPos();
		other.Position = pos.GlobalToLocal(other.pos.globalPos);
		UpdateMap();
		UpdateVisibility();
		camera.Enabled = true;		
		UpdateCamera();
	}

	public void UnPossess()
	{
		isControlled = false;
		camera.Enabled = false;
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
		UpdateCamera();
		map.UpdateObjects();

		Rpc(nameof(UpdatePosition), pos.globalPos);
		
		if (isControlled && map.generator.GetTile(pos.globalPos) == (Tile)((int)Tile.PonkotsuGoal + GetCharacterType()) &&
			map.generator.GetTile(other.pos.globalPos) == (Tile)((int)Tile.PonkotsuGoal + other.GetCharacterType()))
			map.gameMenu.MapCompleted();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void SetPos(Vector3 newPos)
	{
		if (!isControlled)
			return;
		pos.globalPos = newPos;
		Position = pos.GetLocalPos();
		Rpc(nameof(UpdatePosition), newPos);
		UpdateVisibility();
		UpdateMap();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdatePosition(Vector3 newPos)
	{
		pos.globalPos = newPos;
		Position = other.pos.GlobalToLocal(newPos);
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

	public void RemoveItem()
	{
		if (item == null)
			return;

		RemoveChild(item);
		item = null;
	}

	protected void UpdateCamera()
	{
		Vector2I size = pos.GlobalToLocal(map.generator.size) * MapGenerator.tileSize;
		Vector2 screen = GetViewportRect().Size;
		Vector2 cameraPos = Vector2.Zero;

		if (size.X > screen.X)
		{
			if (Position.X <= -MapGenerator.tileSize)
				cameraPos.X = -MapGenerator.tileSize;
			else if (Position.X >= size.X)
				cameraPos.X = size.X;
			else 
				cameraPos.X = Position.X;
		} else {
			cameraPos.X = size.X/2;
		}
		if (size.Y > screen.Y)
		{
			if (Position.Y <= -MapGenerator.tileSize)
				cameraPos.Y = -MapGenerator.tileSize;
			else if (Position.Y >= size.Y)
				cameraPos.Y = size.Y;
			else 
				cameraPos.Y = Position.Y;
		} else { 
			cameraPos.Y = size.Y/2;
		}
		camera.Position = cameraPos;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	protected void UpdateVisibility()
	{
		Visible = true;
		if (!isControlled)
			return;
		other.Visible = CanSee(other.pos.globalPos);
		if (!gameManager.isAlone && gameManager.otherPlayer != null)
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
		float half = size.Y / 2f + 1;
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
	public bool CanSee(Vector3 otherPos)
    {
        return (int)(pos.GetBlindAxisValue(pos.globalPos) / MapGenerator.tileSize) == (int)(pos.GetBlindAxisValue(otherPos) / MapGenerator.tileSize);
    }
	public abstract void UpdateMap();
}
