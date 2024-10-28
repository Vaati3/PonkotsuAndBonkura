using Godot;
using System;

public enum EditMode {
    Add,
    Remove,
    Replace
}

public partial class EditorMenu : CanvasLayer
{
	public EditMode selectedMode {get; private set;}
	public Tile selectedTile {get; private set;}
	public TextEdit filename {get; private set;}

	Button modeBtn = null;
	Button tileBtn = null;
	public override void _Ready()
	{
		selectedMode = EditMode.Add;
		selectedTile = Tile.Block;

		Tile[] tiles = (Tile[])Enum.GetValues(typeof(Tile));
		GridContainer grid = GetNode<GridContainer>("Tiles");
		for (int i = 1; i < tiles.Length; i++)
		{
			grid.AddChild(new EditorButton(tiles[i], EditorButtonPressed));
		}
		VBoxContainer vBox = GetNode<VBoxContainer>("EditModes");
		vBox.AddChild(new EditorButton(EditMode.Add, EditorButtonPressed));
		vBox.AddChild(new EditorButton(EditMode.Remove, EditorButtonPressed));
		vBox.AddChild(new EditorButton(EditMode.Replace, EditorButtonPressed));

		filename = GetNode<TextEdit>("Border/Filename");
	}

	public void EditorButtonPressed(EditorButton button)
	{
		if (button.isMode)
		{
			if (modeBtn != null)
				modeBtn.Disabled = false;
			selectedMode = button.editMode;
			modeBtn = button;
		} else {
			if (tileBtn != null)
				tileBtn.Disabled = false;
			selectedTile = button.tile;
			tileBtn = button;
		}
	}

	[Signal] public delegate void CloseEditorEventHandler(bool save);

	public void _on_save_quit_pressed()
	{
		GetNode<SoundManager>("/root/SoundManager").PlaySFX("button", true);
		EmitSignal(nameof(CloseEditor), true);
	}

	public void _on_quit_pressed()
	{
		GetNode<SoundManager>("/root/SoundManager").PlaySFX("button", true);
		EmitSignal(nameof(CloseEditor), false);
	}
}
