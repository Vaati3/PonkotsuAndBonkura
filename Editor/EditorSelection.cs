using Godot;
using System;

public partial class EditorSelection : Control
{
	const string folder = "user://CustomMaps/";

	SoundManager soundManager;
	VBoxContainer maps;

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
		GD.Print(mapButton.mapName);
	}

	public override void _Ready()
	{
		maps = GetNode<VBoxContainer>("Maps");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
	}

	public void Open()
	{
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
	}
	public void _on_new_pressed()
	{
		soundManager.PlaySFX("button");
	}
}
