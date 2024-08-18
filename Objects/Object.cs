using Godot;

public abstract partial class Object : Node2D
{
	protected SoundManager soundManager;
	protected Character player;
	public Vector3 position3D {get; protected set;}
	public bool activatable {get; protected set;}

	protected bool hide = false;
	protected Sprite2D sprite;

	protected Character[] overlappingPlayers;
	protected bool detectOverlap = true;
	protected Vector3 overlapSize;
	protected Vector3 overlapOffset;

	private Texture2D topTexture;
	private Texture2D sideTexture;

	protected Tile newTile = Tile.Void;

	[Signal]public delegate void FreeObjectEventHandler(Object obj);

	static public T CreateObject<T>(Character character, Vector3I tilePos, Map map = null) where T : Object, new()
	{
		Vector3 pos =  Map.AlignPos(tilePos);
		T obj = new T();
		obj.InitObject(character, pos, map);
		map.generator.SetTile(obj.newTile, MapGenerator.GetTilePos(pos));

		return obj;
	}

	public Object()
	{
		sprite = new Sprite2D();
		AddChild(sprite);
		overlapSize = new Vector3(MapGenerator.tileSize, MapGenerator.tileSize, MapGenerator.tileSize);
		overlapOffset = Vector3.Zero;
		activatable = false;
		overlappingPlayers = new Character[2];
	}

    public override void _EnterTree()
    {
		soundManager = GetNode<SoundManager>("/root/SoundManager");
    }

    virtual public void InitObject(Character player, Vector3 pos, Map map)
	{
		this.player = player;
		SetPosition(pos);
		UpdateVisibility();
	}

	protected void UpdateTexture(Texture2D topTexture, Texture2D sideTexture)
	{
		if (player.GetCharacterType() == CharacterType.Ponkotsu)
		{
			sprite.Texture = topTexture;
		} else {
			sprite.Texture = sideTexture;
		}
	}

	protected virtual void UpdateTexture()
	{
		UpdateTexture(topTexture, sideTexture);
	}
	
	protected void SetTexture(string topTexturePath, string sideTexturePath)
	{

		if (topTexturePath != "" && ResourceLoader.Exists(topTexturePath))
			topTexture = GD.Load<Texture2D>(topTexturePath);
		else
			topTexture = null;
		if (sideTexturePath != "" && ResourceLoader.Exists(sideTexturePath))
			sideTexture = GD.Load<Texture2D>(sideTexturePath);
		else
			sideTexture = null;

		UpdateTexture();
	}

	public bool IsOverlapping()
	{
		return overlappingPlayers[0] != null || overlappingPlayers[1] != null;
	}

	protected bool CheckOverlap(float x, float y, float z)
	{
		Vector3 min = position3D + overlapOffset - overlapSize / 2;
		Vector3 max = position3D + overlapOffset + overlapSize / 2;
		if (x >= min.X && x <= max.X && y >= min.Y && y <= max.Y && z >= min.Z && z <= max.Z)
			return true;
		return false;
	}

	protected bool CheckOverlap(Character character)
	{
		Vector3 pos = character.pos.globalPos;
		Vector2 halfSpr = character.size / 2;
		Vector3 half;
		if (character.pos.blindAxis == Axis.Y)
			half = new Vector3(halfSpr.X, halfSpr.Y, halfSpr.Y);
		else
			half = new Vector3(halfSpr.X, halfSpr.Y, halfSpr.X);

		return CheckOverlap(pos.X - half.X, pos.Y - half.Y, pos.Z - half.Z) ||
			   CheckOverlap(pos.X - half.X, pos.Y + half.Y, pos.Z - half.Z) ||
			   CheckOverlap(pos.X + half.X, pos.Y - half.Y, pos.Z - half.Z) ||
			   CheckOverlap(pos.X + half.X, pos.Y + half.Y, pos.Z - half.Z) ||
			   CheckOverlap(pos.X - half.X, pos.Y - half.Y, pos.Z + half.Z) ||
			   CheckOverlap(pos.X - half.X, pos.Y + half.Y, pos.Z + half.Z) ||
			   CheckOverlap(pos.X + half.X, pos.Y - half.Y, pos.Z + half.Z) ||
			   CheckOverlap(pos.X + half.X, pos.Y + half.Y, pos.Z + half.Z);
	}
	protected bool CheckOverlap()
	{
		return CheckOverlap(player);
	}

	protected virtual void OverlapStarted()
	{
		overlappingPlayers[(int)player.GetCharacterType()] = player;
	}

	protected virtual void OverlapEnded()
	{
		overlappingPlayers[(int)player.GetCharacterType()] = null;
	}

	public virtual void Switch(Character character)
	{
		player = character;

		UpdateTexture();
		Update();
	}

	private void SetPosition(Vector3 pos)
	{
		position3D = pos;
		Position = player.pos.GlobalToLocal(pos);
	}

	protected virtual void UpdateVisibility()
	{
		Visible = overlappingPlayers[(int)player.GetCharacterType()] != null || (!hide && player.CanSee(position3D));
	}

	public virtual void Update()
	{	
		Position = player.pos.GlobalToLocal(position3D);

		if (detectOverlap && CheckOverlap())
		{
			if (overlappingPlayers[(int)player.GetCharacterType()] == null)
				OverlapStarted();
		} else {
			if (overlappingPlayers[(int)player.GetCharacterType()] != null)
				OverlapEnded();
		}

		UpdateVisibility();
	}

	public virtual void Trigger(bool state)
	{
		throw new System.NotImplementedException();
	}
}