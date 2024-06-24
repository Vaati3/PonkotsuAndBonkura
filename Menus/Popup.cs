using Godot;
using System;

public partial class Popup : CanvasLayer
{
	[Signal]public delegate void ConfirmEventHandler();
	static public Popup Open(string text, ConfirmEventHandler confirm = null)
	{
		Popup popup = GD.Load<PackedScene>("res://Menus/Popup.tscn").Instantiate<Popup>();

		popup.GetNode<Label>("Label").Text = text;
		if (confirm == null)
		{
			popup.GetNode<Button>("HBoxContainer/YesButton").Visible = false;
			popup.GetNode<Button>("HBoxContainer/NoButton").Text = "OK";
		} else {
			popup.Confirm += confirm;
		}

		return popup;
	}

	public void _on_yes_button_pressed()
	{
		EmitSignal(nameof(Confirm));
		QueueFree();
	}
	public void _on_no_button_pressed()
	{
		QueueFree();
	}
}
