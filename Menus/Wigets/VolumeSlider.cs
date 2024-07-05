using Godot;
using System;

public partial class VolumeSlider : Control
{
	[Export]string label;
	[Export]string busName;

	SoundManager soundManager;
	int busIndex;
	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		GetNode<Label>("Label").Text = label;
		busIndex = AudioServer.GetBusIndex(busName);
	}

	public void _on_slider_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(value));
	}

	public void _on_mute_toggled(bool state)
	{
		soundManager.PlaySFX("button");
		AudioServer.SetBusMute(busIndex, state);
	}
}
