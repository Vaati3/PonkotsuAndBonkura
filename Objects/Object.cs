using Godot;
using System;

public abstract partial class Object : Node2D
{
	protected Character player;
	public Vector3 position3D {get; protected set;}

	private bool hide = false;
	private Sprite2D sprite;

	protected bool playerOverlap = false;
	protected bool detectOverlap = true;
	protected Vector3 overlapSize; 

	public Object()
	{
		sprite = new Sprite2D();
		AddChild(sprite);
		overlapSize = new Vector3(MapGenerator.tileSize, MapGenerator.tileSize, MapGenerator.tileSize);
	}

	virtual public void InitObject(Character player, Vector3 pos)
	{
		this.player = player;
		SetPosition(pos);
	}
	
	protected void UpdateSprite(string topTexturePath, string sideTexturePath)
	{
		
		if (player.GetCharacterType() == CharacterType.Ponkotsu)
		{
			if (topTexturePath == "")
				hide = true;
			else
				sprite.Texture = GD.Load<Texture2D>(topTexturePath);
		} else {
			if (sideTexturePath == "")
				hide = true;
			else
				sprite.Texture = GD.Load<Texture2D>(sideTexturePath);
		}
		UpdateVisibility();
	}

	private void SetPosition(Vector3 pos)
	{
		position3D = pos;
		Position = player.GetLocalPos(pos);
	}

	private void UpdateVisibility()
	{
		Visible = playerOverlap || (!hide && player.CanSee(position3D));
	}

	private bool CheckOverlap()
	{
		Vector3 min = new Vector3(position3D.X - overlapSize.X/2, position3D.Y - overlapSize.Y, position3D.Z - overlapSize.Z/2);
		Vector3 max = new Vector3(position3D.X + overlapSize.X/2, position3D.Y, position3D.Z + overlapSize.Z/2);
		// GD.Print("min : " + min + " max: " + max + " player: " + player.position3D);
		if (player.position3D.X >= min.X && player.position3D.X <= max.X &&
			player.position3D.Y >= min.Y && player.position3D.Y <= max.Y &&
			player.position3D.Z >= min.Z && player.position3D.Z <= max.Z)
			return true;
		return false;
	}

	protected virtual void OverlapStarted()
	{
		playerOverlap = true;
	}

	protected virtual void OverlapEnded()
	{
		playerOverlap = false;;
	}

	public void Update()
	{	
		Position = player.GetLocalPos(position3D);

		if (detectOverlap && CheckOverlap())
		{
			if (!playerOverlap)
				OverlapStarted();
		} else {
			if (playerOverlap)
				OverlapEnded();
		}

		UpdateVisibility();
	}

	public abstract void Trigger(bool state);
}