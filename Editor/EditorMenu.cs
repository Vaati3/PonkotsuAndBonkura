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

	Label xLabel;
	Label yLabel;
	Label zLabel;

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
		modeBtn = new EditorButton(EditMode.Add, EditorButtonPressed);
		modeBtn.Disabled = true;
		vBox.AddChild(modeBtn);
		vBox.AddChild(new EditorButton(EditMode.Remove, EditorButtonPressed));
		vBox.AddChild(new EditorButton(EditMode.Replace, EditorButtonPressed));

		filename = GetNode<TextEdit>("Border/Filename");

		xLabel = GetNode<Label>("Size/X");
		yLabel = GetNode<Label>("Size/Y");
		zLabel = GetNode<Label>("Size/Z");
	}

	public void UpdateSize(Vector3I size)
	{
		xLabel.Text = "X\n" + size.X;
		yLabel.Text = "Y\n" + size.Y;
		zLabel.Text = "Z\n" + size.Z;
	}

	public void EditorButtonPressed(EditorButton button)
	{
		if (button.isMode)
		{
			if (modeBtn != null)
				modeBtn.Disabled = false;
			selectedMode = button.editMode;
			modeBtn = button;
			if (selectedMode == EditMode.Remove && tileBtn != null)
			{
				tileBtn.Disabled = false;
				selectedTile = Tile.Void;
			}
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

	//resize
	[Signal] public delegate void ResizeEventHandler(Vector3I dif);

	public void _on_x_sub_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(-1, 0, 0));
	}
	public void _on_x_add_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(1, 0, 0));
	}
	public void _on_y_sub_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(0, -1, 0));
	}
	public void _on_y_add_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(0, 1, 0));
	}
	public void _on_z_sub_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(0, 0, -1));
	}
	public void _on_z_add_pressed()
	{
		EmitSignal(nameof(Resize), new Vector3I(0, 0, 1));
	}
}
