using Godot;
using System;

public abstract partial class Object : Node2D
{
	protected Character player;
	public Vector3 position3D {get; protected set;}
	public bool activatable {get; protected set;}

	private bool hide = false;
	protected Sprite2D sprite;

	protected bool playerOverlap = false;
	protected bool detectOverlap = true;
	protected Vector3 overlapSize;

	public Object()
	{
		sprite = new Sprite2D();
		AddChild(sprite);
		overlapSize = new Vector3(MapGenerator.tileSize, MapGenerator.tileSize, MapGenerator.tileSize);
		activatable = false;
	}

	virtual public void InitObject(Character player, Vector3 pos)
	{
		this.player = player;
		SetPosition(pos);
		UpdateVisibility();
	}
	
	protected void SetTexture(string topTexturePath, string sideTexturePath)
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
		Vector3 min = position3D - overlapSize / 2;
		Vector3 max = position3D + overlapSize / 2;
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
		playerOverlap = false;
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

	public virtual void Trigger(bool state)
	{
		throw new System.NotImplementedException();
	}
}