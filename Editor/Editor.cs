using Godot;
using System;

public partial class Editor : Node3D
{
    EditorCamera camera;
    Material[] materials;
    MeshInstance3D[,,] meshes;
    MapGenerator map = null;
    Node3D mapOrigin;
    private void LoadTextures()
    {
        string[] tiles = Enum.GetNames(typeof(Tile));
        materials = new Material[tiles.Length];
        materials[0] = null;

        for (int i = 1; i < tiles.Length; i++)
        {
            materials[i] = new StandardMaterial3D()
            {
                AlbedoTexture = GD.Load<Texture2D>("res://Editor/Textures/" + tiles[i] + ".png")
            };
        }
    }

	public override void _Ready()
	{
        camera = GetNode<EditorCamera>("CameraOrigin");
        mapOrigin = GetNode<Node3D>("MapOrigin");
        LoadTextures();
        map = new MapGenerator();
	}

    public void LoadMap(string mapName, string folder)
    {
        if (map != null)
        {
            MapGenerator.Action action = (tile, pos) => {
			if (tile != Tile.Void)
                meshes[pos.X, pos.Y, pos.Z].QueueFree();
		    };
        }
        map.Read(mapName, folder, true);
        camera.Center(map.size);
        meshes = new MeshInstance3D[map.size.X, map.size.Y, map.size.Z];
        map.LoopAction(SetMesh);
        Visible = true;
    }

    private void SetMesh(Tile tile, Vector3I pos)
    {
        if (tile == Tile.Void)
            return;
        meshes[pos.X, pos.Y, pos.Z] = new Cube(map, pos, materials[(int)tile]);
        mapOrigin.AddChild(meshes[pos.X, pos.Y, pos.Z]);
    }

    public override void _Input(InputEvent @event)
    {
    }
}
