using Godot;
using System;

public partial class LevelButton : Button
{
	public string levelName {get; private set;}
	public LevelButton(string levelName, LevelPressedEventHandler levelPressed)
	{
		this.levelName = levelName.GetBaseName();

		Text = this.levelName;
		CustomMinimumSize = new Vector2(100, 100);

		Pressed += _on_button_pressed;
		LevelPressed += levelPressed;

		Theme = GD.Load<Theme>("res://Menus/Themes/Button.tres");
	}

	[Signal]public delegate void LevelPressedEventHandler(LevelButton levelButton);

	public void _on_button_pressed()
	{
		Disabled = true;
		EmitSignal(nameof(LevelPressed), this);
	}
}
