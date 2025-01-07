using Godot;
using System;

public partial class Editor : Node3D
{
    public EditorMenu menu {get; private set;}
    EditorCamera camera;
    Material[] materials;

    public MeshInstance3D previewCube {get; private set;}
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
        menu.Resize += Resize;
        mapOrigin = GetNode<Node3D>("MapOrigin");
        map = new MapGenerator();
        LoadMaterials();

        // previewCube = new MeshInstance3D()
        // {
        //     Position = new Vector3(-1, -1, -1),
        //     MaterialOverride = materials[1],
        //     Visible = true
        // };
        //((StandardMaterial3D)previewCube.MaterialOverride).AlbedoColor = new Color(1, 1, 1, 0.3f);
        // AddChild(previewCube);
	}

    public void OpenMap(string mapName, string folder)
    {
        map.Read(mapName, folder, true);
        LoadMap(mapName);
    }

    public void NewMap()
    {
        map.New();
        LoadMap("New Map");
    }

    private void LoadMap(string mapName)
    {
        camera.Center(map.size);
        map.LoopAction(SetMesh);
        Visible = true;
        menu.filename.Text = mapName;
        menu.Visible = true;
        menu.UpdateSize(map.size);
    }

    public void CloseMap()
    {
        if (map != null)
        {
            foreach(Node node in mapOrigin.GetChildren())
            {
                node.QueueFree();
            }
        }
    }

    public delegate void SetMeshAction(Tile tile, Vector3I pos);
    private void SetMesh(Tile tile, Vector3I pos)
    {
        if (tile == Tile.Void)
            return;
        Cube cube = new Cube(pos, map.size, materials[(int)tile], CubeClicked, CubeHover);
        mapOrigin.AddChild(cube);
    }

    private void Resize(Vector3I dif)
    {
        if (map.size.X + dif.X <= 0 || map.size.Y + dif.Y <= 0 || map.size.Z + dif.Z <= 0)
            return;
        map.Resize(map.size + dif, SetMesh);
        foreach(Cube cube in mapOrigin.GetChildren())
        {
            if (dif.Y != 0)
            {
                if (dif.Y < 0) {
                    cube.pos = new Vector3I(cube.pos.X, cube.pos.Y - 1, cube.pos.Z);
                } else {
                    cube.pos = new Vector3I(cube.pos.X, cube.pos.Y + 1, cube.pos.Z);
                }
                cube.Position = new Vector3(cube.pos.X,  map.size.Y - cube.pos.Y, cube.pos.Z);
            }
            if (map.IsOutOfBound(cube.pos))
            {
                cube.QueueFree();
            }
        }
        menu.UpdateSize(map.size);
        camera.Center(map.size);
    }

    private void CubeClicked(Cube cube, Vector3I addPos)
    {
        switch (menu.selectedMode)
        {
            case EditMode.Add:
                if (menu.selectedTile == Tile.Void)
                    return;
                if (!map.SetTile(menu.selectedTile, addPos))
                    return;
                SetMesh(menu.selectedTile, addPos);
                break;
            case EditMode.Remove:
                if (cube.pos.Y == map.size.Y - 1)
                    return;
                map.SetTile(Tile.Void, cube.pos);
                cube.QueueFree();
                break;
            case EditMode.Replace:
                if (menu.selectedTile == Tile.Void || cube.pos.Y == map.size.Y - 1)
                    return;
                map.SetTile(menu.selectedTile, cube.pos);
                cube.Change(materials[(int)menu.selectedTile]);
                break;
        }
    }

    private void CubeHover(Cube cube, Vector3I addPos, bool exit)
    {
        // if (exit)
        // {
        //     if (menu.selectedMode == EditMode.Add)
        //         previewCube.Visible = false;
        //     return;
        // }
        switch (menu.selectedMode)
        {
            case EditMode.Add:
                // addPos.Y = map.size.Y - addPos.Y;
                // previewCube.Position = addPos;
                // previewCube.Visible = true;
                break;
            case EditMode.Remove: case EditMode.Replace:
                if (cube.pos.Y == map.size.Y - 1)
                    return;
                cube.Hover();
                break;
        }
    }
}
