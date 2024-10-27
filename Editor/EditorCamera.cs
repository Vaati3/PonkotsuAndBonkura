using Godot;
using System;

public partial class EditorCamera : Node3D
{
	Camera3D camera;
	bool isRotating = false;
	bool isTranslating = false;
	public const float sensitivity = 0.3f;
	public const float limit = 85f;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera");
    }

    private void Zoom(int speed = 1)
	{
		if (speed < 0 && camera.Position.Z < 2)
			return;
		camera.Position += Vector3.Back * speed;
	}

	public void Center(Vector3 mapSize)
	{
		Position = mapSize/2;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{		
			switch (mouseButton.ButtonIndex)
			{
				case MouseButton.Right:
					isRotating = !isRotating;
					break;
				case MouseButton.WheelUp:
					Zoom(-1);
					break;
				case MouseButton.WheelDown:
					Zoom(1);
					break;
				case MouseButton.Middle:
					//isTranslating = !isTranslating;
					break;
			}
		}
		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (isRotating)
			{
				RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * sensitivity));
				float x = Mathf.Cos(Rotation.Y);
				float z = -Mathf.Sin(Rotation.Y);
				Rotate(new Vector3(x, 0, z).Normalized(), Mathf.DegToRad(-mouseMotion.Relative.Y * sensitivity));
				// cameraOrigin.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * sensitivity));
				// float clampedHead = Mathf.Clamp(cameraOrigin.Rotation.X, -limit, limit);
				// if (Mathf.Abs(clampedHead) == limit)
				// 	cameraOrigin.Rotation = new Vector3(clampedHead, cameraOrigin.Rotation.Y, cameraOrigin.Rotation.Z);
			} 
			else if (isTranslating)
			{
				float x = Mathf.Cos(Rotation.Y);
				float z = -Mathf.Sin(Rotation.Y);
				int dir = mouseMotion.Relative.Y == 0 ? 0 : mouseMotion.Relative.Y < 0 ? -1 : 1;
				Position += new Vector3(x, 0, z) * dir;
			}
		}
	}
}
