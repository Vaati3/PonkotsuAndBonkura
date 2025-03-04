using Godot;
using System;

public partial class EditorSelection : Control
{
	const string folder = "user://CustomMaps/";

	SoundManager soundManager;
	Editor editor = null;
	VBoxContainer maps;

	MapButton selectedMap = null;

	private void UpdateMaps()
	{
		foreach(Node node in maps.GetChildren())
		{
			node.QueueFree();
		}
		GetNode<Button>("Open").Disabled = true;
		selectedMap = null;

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
		if (selectedMap == null)
			GetNode<Button>("Open").Disabled = false;
		else
			selectedMap.Disabled = false;
		selectedMap = mapButton;
	}

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		maps = GetNode<VBoxContainer>("Scroll/Maps");
	}

	public void Open()
	{
		if (editor == null)
		{
			editor = GD.Load<PackedScene>("res://Editor/Editor.tscn").Instantiate<Editor>();
			GetTree().Root.AddChild(editor);
			editor.menu.CloseEditor += CloseEditor;
		}
		UpdateMaps();
		Visible = true;
	}

	public void CloseEditor(bool save)
	{
		if (save)
		{
			editor.map.Save(editor.menu.filename.Text, folder); //add filename check
		}
		editor.menu.Visible = false;
		editor.Visible = false;
		editor.CloseMap();
		UpdateMaps();
		((Control)GetParent()).Visible = true;
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
		editor.OpenMap(selectedMap.mapName, folder);
		((Control)GetParent()).Visible = false;
	}
	public void _on_new_pressed()
	{
		soundManager.PlaySFX("button");
		editor.NewMap();
		((Control)GetParent()).Visible = false;
	}
}
