using Godot;
using System;
using System.Collections.Generic;

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

	private Texture2D topTexture;
	private Texture2D sideTexture;

	[Signal]public delegate void FreeObjectEventHandler(Object obj);

	public Object()
	{
		sprite = new Sprite2D();
		AddChild(sprite);
		overlapSize = new Vector3(MapGenerator.tileSize, MapGenerator.tileSize, MapGenerator.tileSize);
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
		Position = player.GetLocalPos(pos);
	}

	private void UpdateVisibility()
	{
		Visible = overlappingPlayers[(int)player.GetCharacterType()] != null || (!hide && player.CanSee(position3D));
	}

	public void Update()
	{	
		Position = player.GetLocalPos(position3D);

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