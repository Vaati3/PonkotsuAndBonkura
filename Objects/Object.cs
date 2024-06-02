using Godot;
using System;

public partial class Object : Node2D
{
	protected Character player;
	protected Vector3 position3D;

	bool hide = false;
	Sprite2D sprite;

	public Object()
	{
		sprite = new Sprite2D();
		AddChild(sprite);
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
		Visible = !hide && player.CanSee(position3D);
	}

	public void Update()
	{
		
		UpdateVisibility();
	}
}