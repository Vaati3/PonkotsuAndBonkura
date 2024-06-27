using System.Diagnostics;
using Godot;

public enum Axis {
	X,
	Y,
	Z
}

public class Position
{
    public Axis blindAxis {get; private set;}
    public Vector3 globalPos {get; set;}

    public Position(CharacterType type)
    {
        this.blindAxis = type == CharacterType.Ponkotsu ? Axis.Y : Axis.Z;
        globalPos = Vector3.Zero;
    }

    public void Move(Vector2 localPos, Vector3 dir)
    {
        if (blindAxis == Axis.Z)
            globalPos = new Vector3(localPos.X, localPos.Y, globalPos.Z + dir.Z);
        else if (blindAxis == Axis.Y)
            globalPos = new Vector3(localPos.X, globalPos.Y + dir.Y, localPos.Y);
        else
            globalPos = new Vector3(globalPos.X + dir.X, localPos.Y, localPos.X);
    }

    public Vector3 LocalToGlobal(float x, float y)
    {

        if (blindAxis == Axis.Z)
            return new Vector3(x, y, globalPos.Z);
        else if (blindAxis == Axis.Y)
            return new Vector3(x, globalPos.Y, y);
        else
            return new Vector3(globalPos.X, y, x);
    }

    public Vector3 LocalToGlobal(Vector2 localPos)
    {
        return LocalToGlobal(localPos.X, localPos.Y);
    }

    public Vector2 GlobalToLocal(Vector3 pos)
    {
        if (blindAxis == Axis.Z)
            return new Vector2(pos.X, pos.Y);
        else if (blindAxis == Axis.Y)
            return new Vector2(pos.X, pos.Z);
        else
            return new Vector2(pos.Z, pos.Y);
    }

    public Vector2 GetLocalPos()
    {
        return GlobalToLocal(globalPos);
    }
}
