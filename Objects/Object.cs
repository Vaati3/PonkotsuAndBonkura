using Godot;

public abstract partial class Object : Node2D
{
	protected Character player;
	public Vector3 position3D {get; protected set;}
	public bool activatable {get; protected set;}

	private bool hide = false;
	protected Sprite2D sprite;

	protected Character[] overlappingPlayers;
	protected bool detectOverlap = true;
	protected Vector3 overlapSize;
	protected Vector3 overlapOffset;

	private Texture2D topTexture;
	private Texture2D sideTexture;

	[Signal]public delegate void FreeObjectEventHandler(Object obj);

	static public T CreateObject<T>(Character character, Vector3I tilePos) where T : Object, new()
	{
		Vector3 pos =  Map.AlignPos(tilePos);
		T obj = new T();
		obj.InitObject(character, pos);

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

	virtual public void InitObject(Character player, Vector3 pos)
	{
		this.player = player;
		SetPosition(pos);
		UpdateVisibility();
	}

	protected void UpdateTexture(Texture2D topTexture, Texture2D sideTexture)
	{
		Visible = true;
		if (player.GetCharacterType() == CharacterType.Ponkotsu)
		{
			if (topTexture != null)
				sprite.Texture = topTexture;
			else
				Visible = false;
		} else {
			if (sideTexture != null)
				sprite.Texture = sideTexture;
			else
				Visible = false;
		}
	}

	protected virtual void UpdateTexture()
	{
		UpdateTexture(topTexture, sideTexture);
	}
	
	protected void SetTexture(string topTexturePath, string sideTexturePath)
	{

		if (FileAccess.FileExists(topTexturePath))
			topTexture = GD.Load<Texture2D>(topTexturePath);
		else
			topTexture = null;
		if (FileAccess.FileExists(sideTexturePath))
			sideTexture = GD.Load<Texture2D>(sideTexturePath);
		else
			sideTexture = null;

		UpdateTexture();
	}

	public bool IsOverlapping()
	{
		return overlappingPlayers[0] != null || overlappingPlayers[1] != null;
	}


	protected bool CheckOverlap(Character character)
	{
		Vector3 min = position3D + overlapOffset - overlapSize / 2;
		Vector3 max = position3D + overlapOffset + overlapSize / 2;
		if (character.pos.globalPos.X >= min.X && character.pos.globalPos.X <= max.X &&
			character.pos.globalPos.Y >= min.Y && character.pos.globalPos.Y <= max.Y &&
			character.pos.globalPos.Z >= min.Z && character.pos.globalPos.Z <= max.Z)
			return true;
		return false;
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

	public void Switch(Character character)
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

	private void UpdateVisibility()
	{
		Visible = overlappingPlayers[(int)player.GetCharacterType()] != null || (!hide && player.CanSee(position3D));
	}

	public void Update()
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