using Godot;
using GodotSteam;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Player {
	public long id {get; set;}
	public string name {get; set;}
	public int progression {get; set;}
	public CharacterType characterType {get; set;}
	public bool isReady {get; set;}
	public Player(int progression = 0, CharacterType characterType = CharacterType.Ponkotsu, long id = 0, string name = "")
	{
		this.id = id;
		this.name = name;
		this.progression = progression;
		this.characterType = characterType;
		isReady = false;
	}
    public override string ToString()
    {
        return id + " " + name + " " + progression + " " + (int)characterType;
    }
	static public Player FromString(string str)
	{
		string[] data = str.Split(" ");
		long id = long.Parse(Regex.Replace(data[0], "[^0-9]", ""));
		return new Player(data[2].ToInt(), (CharacterType)data[3].ToInt(), id, data[1]);
	}

	public string Save()
	{
		return progression + " " + (int)characterType;
	}
	static public Player Load(string str)
	{
		string[] data = str.Split(" ");
		return new Player(data[0].ToInt(), (CharacterType)data[1].ToInt());
	}
}

public class Settings
{
	public Dictionary<string, float> volumes {get; private set;}
	public Dictionary<string, bool> muted {get; private set;}
	
	public Settings(float masterVolume = 0.5f, bool masterMuted = false, float musicVolume = 0.5f, bool musicMuted = false, float sfxVolume = 0.5f, bool sfxMuted = false)
	{
		volumes = new Dictionary<string, float>();
		muted = new Dictionary<string, bool>();

		volumes.Add("Master", masterVolume);
		muted.Add("Master", masterMuted);
		volumes.Add("Music", musicVolume);
		muted.Add("Music", musicMuted);
		volumes.Add("SFX", sfxVolume);
		muted.Add("SFX", sfxMuted);
	}
	public override string ToString()
    {
        return volumes["Master"] + " " + muted["Master"] + " " + volumes["Music"] + " " + muted["Music"] + " " + volumes["SFX"] + " " + muted["SFX"];
    }
	static public Settings FromString(string str)
	{
		string[] data = str.Split(" ");
		return new Settings(data[0].ToFloat(), bool.Parse(data[1]), data[2].ToFloat(), bool.Parse(data[3]), data[4].ToFloat(), bool.Parse(data[5]));
	}
}

public partial class GameManager : Node
{
	string savePath = "user://save.save";
	public bool isAlone {get; set;} = false;
	public Settings settings {get; private set;}
	public Player player {get; private set;}
	public Player otherPlayer {get; private set;}

	//steam
	const uint appId = 480;

	[Signal]public delegate void UpdateServerEventHandler();

    public override void _EnterTree()
    {
        OS.SetEnvironment("SteamAppId", appId.ToString());
        OS.SetEnvironment("SteamGameId", appId.ToString());
    }

    public override void _Ready()
    {
		Load();
		Steam.SteamInit();
        if (!Steam.IsSteamRunning())
        {
            GD.Print("Steam is not running");
			if (OS.HasEnvironment("USERNAME"))
				player.name = OS.GetEnvironment("USERNAME");
			else
				player.name = "player";
			return;
        }
		ulong id = Steam.GetSteamID();
		player.name = Steam.GetFriendPersonaName(id);
    }

    public void PlayerJoined(Player newPlayer, bool isServer)
	{
		if (player.id != newPlayer.id)
			otherPlayer = newPlayer;
		if (isServer)
			otherPlayer.characterType = player.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		else
			player.characterType = otherPlayer.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		if (newPlayer.progression > player.progression)
			player.progression = newPlayer.progression;
		else
			newPlayer.progression = player.progression;
		if (isServer)
			EmitSignal(nameof(UpdateServer));
	}

	public void Save()
	{
		FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);

		file.StoreLine(player.Save());
		file.StoreLine(settings.ToString());
		file.Close();
	}

	public bool Load()
	{
		FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			player = new Player();
			settings = new Settings();
			return false;
		}
		player = Player.Load(file.GetLine());
		settings = Settings.FromString(file.GetLine());
		file.Close();
		return true;
	}

	public void Clear()
	{
		otherPlayer = null;
		isAlone = false;
	}
}
