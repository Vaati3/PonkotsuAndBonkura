using Godot;
using System;

public partial class EditorButton : Button
{
	[Signal]public delegate void EditButtonEventHandler(EditorButton leButton);

	public bool isMode {get; private set;}
	public Tile tile {get; private set;}
	public EditMode editMode {get; private set;}

	public EditorButton(Tile tile, EditButtonEventHandler buttonPressed)
	{
		isMode = false;
		this.tile = tile;
		Text = tile.ToString();
		Init(buttonPressed);
	}
	public EditorButton(EditMode editMode, EditButtonEventHandler buttonPressed)
	{
		isMode = true;
		this.editMode = editMode;
		Text = editMode.ToString();
		Init(buttonPressed);
	}

	private void Init(EditButtonEventHandler buttonPressed)
	{
		Pressed += _on_button_pressed;
		EditButton += buttonPressed;

		Theme = GD.Load<Theme>("res://Menus/Themes/Button.tres");
		AddThemeFontSizeOverride("font_size", 10);
		CustomMinimumSize = new Vector2(60, 60);
	}

	public void _on_button_pressed()
	{
		GetNode<SoundManager>("/root/SoundManager").PlaySFX("button", true);
		Disabled = true;
		EmitSignal(nameof(EditButton), this);
	}
}
