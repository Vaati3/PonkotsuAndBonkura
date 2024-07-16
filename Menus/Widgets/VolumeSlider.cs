using Godot;
using System;

public partial class VolumeSlider : Control
{
	[Export]string label;
	[Export]string busName;

	SoundManager soundManager;
	GameManager gameManager;
	int busIndex;

	public override void _Ready()
	{
		soundManager = GetNode<SoundManager>("/root/SoundManager");
		gameManager = GetNode<GameManager>("/root/GameManager");

		GetNode<Label>("Label").Text = label;
		busIndex = AudioServer.GetBusIndex(busName);

		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(gameManager.settings.volumes[busName]));
		GetNode<HSlider>("Slider").Value = gameManager.settings.volumes[busName];
		AudioServer.SetBusMute(busIndex, gameManager.settings.muted[busName]);
		CheckBox checkBox = GetNode<CheckBox>("Mute");
		checkBox.ButtonPressed = gameManager.settings.muted[busName];
		checkBox.Toggled += _on_mute_toggled;

	}

	public void _on_slider_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(value));
		gameManager.settings.volumes[busName] = value;
	}

	public void _on_mute_toggled(bool state)
	{
		soundManager.PlaySFX("button");
		AudioServer.SetBusMute(busIndex, state);
		gameManager.settings.muted[busName] = state;
	}
}
