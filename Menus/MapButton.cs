using Godot;
using System;

public partial class MapButton : Button
{
	public string mapName {get; private set;}
	public int mapNumber {get; private set;}

	public MapButton(string mapName, int number, int progression, LevelPressedEventHandler mapPressed)
	{
		this.mapName = mapName.GetBaseName();
		mapNumber = number;

		Text = this.mapName;
		CustomMinimumSize = new Vector2(100, 100);

		Pressed += _on_button_pressed;
		LevelPressed += mapPressed;

		Theme = GD.Load<Theme>("res://Menus/Themes/Button.tres");

		if (number > progression)
			Visible = false;
	}

	[Signal]public delegate void LevelPressedEventHandler(MapButton leButton);

	public void _on_button_pressed()
	{
		Disabled = true;
		EmitSignal(nameof(LevelPressed), this);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
