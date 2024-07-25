using Godot;
using System;

public partial class MapButton : Button
{
	public string mapName {get; private set;}
	public int mapNumber {get; private set;}

	[Signal]public delegate void LevelPressedEventHandler(MapButton leButton);

	public MapButton(string mapName, int number, int progression, LevelPressedEventHandler mapPressed)
	{
		
		this.mapName = mapName.GetBaseName();
		mapNumber = number;
		Text = this.mapName;

		Pressed += _on_button_pressed;
		LevelPressed += mapPressed;

		Theme = GD.Load<Theme>("res://Menus/Themes/Button.tres");
		CustomMinimumSize = new Vector2(100, 100);

		if (number > progression)
			Visible = false;
	}

	public void _on_button_pressed()
	{
		GetNode<SoundManager>("/root/SoundManager").PlaySFX("button", true);
		Disabled = true;
		EmitSignal(nameof(LevelPressed), this);
	}
}
