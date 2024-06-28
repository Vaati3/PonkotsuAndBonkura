using System.Diagnostics;
using Godot;

public enum Axis {
	X,
	Y,
	Z
}

public class Position
{
    public Axis blindAxis {get; set;}
    readonly Axis originalBlindAxis;
    public Vector3 globalPos {get; set;}

    public Position(CharacterType type)
    {
        blindAxis = type == CharacterType.Ponkotsu ? Axis.Y : Axis.Z;
        originalBlindAxis = blindAxis;
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

    static public float GetAxisValue(Axis axis, Vector3 pos)
    {
        if (axis == Axis.Z)
            return pos.Z;
        else if (axis == Axis.Y)
            return pos.Y;
        else
            return pos.X;
    }

    public float GetBlindAxisValue(Vector3 pos)
    {
        return GetAxisValue(blindAxis, pos);
    }

    public Vector3I Convert(int x, int y, int z)
    {
        if (originalBlindAxis == blindAxis)
            return new Vector3I(x, y, z);
        
        if (originalBlindAxis == Axis.Y)
        {
            if (blindAxis == Axis.Z)
                return new Vector3I(x, z, y);
            else
                return new Vector3I(y, x, z);
        } else {
            if (blindAxis == Axis.X)
                return new Vector3I(z, y, x);
            else
                return new Vector3I(x, z, y);
        }
    }

    public Vector3 Convert(float x, float y, float z)
    {
        if (originalBlindAxis == blindAxis)
            return new Vector3(x, y, z);
        
        if (originalBlindAxis == Axis.Y)
        {
            if (blindAxis == Axis.Z)
                return new Vector3(x, z, y);
            else
                return new Vector3(y, x, z);
        } else {
            if (blindAxis == Axis.X)
                return new Vector3(z, y, x);
            else
                return new Vector3(x, z, y);
        }
    }

    public Vector3I Convert(Vector3I pos)
    {
        if (originalBlindAxis == blindAxis)
            return pos;
        
        return Convert(pos.X, pos.Y, pos.Z);
    }

    public Vector3 Convert(Vector3 pos)
    {
        if (originalBlindAxis == blindAxis)
            return pos;
        
        return Convert(pos.X, pos.Y, pos.Z);
    }
}
