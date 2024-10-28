using Godot;
using System;

public partial class Editor : Node3D
{
    public EditorMenu menu {get; private set;}
    EditorCamera camera;
    Material[] materials;
    MeshInstance3D[,,] meshes;
    public MapGenerator map {get; private set;} = null;
    Node3D mapOrigin;

    private void LoadMaterials()
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
        menu = GetNode<EditorMenu>("EditorMenu");
        mapOrigin = GetNode<Node3D>("MapOrigin");
        map = new MapGenerator();
        LoadMaterials();
	}

    public void LoadMap(string mapName, string folder)
    {
        if (map != null)
        {
            foreach(Node node in mapOrigin.GetChildren())
            {
                node.QueueFree();
            }
        }
        map.Read(mapName, folder, true);
        camera.Center(map.size);
        meshes = new MeshInstance3D[map.size.X, map.size.Y, map.size.Z];
        map.LoopAction(SetMesh);
        Visible = true;
        menu.Visible = true;
    }

    private void SetMesh(Tile tile, Vector3I pos)
    {
        if (tile == Tile.Void)
            return;
        meshes[pos.X, pos.Y, pos.Z] = new Cube(pos, map.size, materials[(int)tile], CubeClicked);
        mapOrigin.AddChild(meshes[pos.X, pos.Y, pos.Z]);
    }

    private void CubeClicked(Cube cube)
    {
        switch (menu.selectedMode)
        {
            case EditMode.Add:
                break;
            case EditMode.Remove:
            map.SetTile(Tile.Void, cube.pos);
                cube.QueueFree();
                break;
            case EditMode.Replace:
                map.SetTile(menu.selectedTile, cube.pos);
                cube.Change(materials[(int)menu.selectedTile]);
                break;
        }
    }
}
