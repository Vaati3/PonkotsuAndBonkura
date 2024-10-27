using Godot;
using System;

public partial class EditorSelection : Control
{
	const string folder = "user://CustomMaps/";

	SoundManager soundManager;
	Editor editor = null;
	VBoxContainer maps;

	string selectedMap = "";

	private void UpdateLevels()
	{
		DirAccess dir = DirAccess.Open(folder);

		if (dir == null)
		{
			DirAccess.MakeDirAbsolute(folder);
			return;
		}

		dir.ListDirBegin();
		string mapName = dir.GetNext();
		while (mapName != "")
		{
			if (!dir.CurrentIsDir())
			{
				maps.AddChild(new MapButton(mapName, 0, 1, LevelPressed));
			}
			mapName = dir.GetNext();
		}
	}

	private void LevelPressed(MapButton mapButton)
	{
		if (selectedMap == "")
			GetNode<Button>("Open").Disabled = false;
		selectedMap = mapButton.mapName;
	}

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		maps = GetNode<VBoxContainer>("Maps");
	}

	public void Open()
	{
		if (editor == null)
		{
			editor = GD.Load<PackedScene>("res://Editor/Editor.tscn").Instantiate<Editor>();
			GetTree().Root.AddChild(editor);
		}
		Visible = true;
		UpdateLevels();
	}

	//buttons
	public void _on_back_pressed()
	{
		soundManager.PlaySFX("button");
		Visible = false;
	}
	public void _on_open_pressed()
	{
		soundManager.PlaySFX("button");
		editor.LoadMap(selectedMap, folder);
		GetParent<Control>().Hide();
		// foreach(Node node in GetTree().Root.GetChildren())
		// {
		// 	GD.Print(node.GetType());
		// }
		// editor.LoadMap(selectedMap, folder);
	}
	public void _on_new_pressed()
	{
		soundManager.PlaySFX("button");
	}
}
