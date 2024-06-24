using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Player {
	public long id {get; set;}
	public string name {get; set;}
	public int progression {get; private set;}
	public CharacterType characterType {get; set;}
	public bool isReady {get; set;}
	public Player(long id = 0, string name = "", int progression = 0, CharacterType characterType = CharacterType.Ponkotsu)
	{
		this.id = id;
		this.name = name;
		this.progression = progression;
		this.characterType = characterType;
		isReady = false;
	}
    public override string ToString()
    {
        return id + "," + name + "," + progression + "," + (int)characterType;
    }
	static public Player FromString(string str)
	{
		string[] data = str.Split(",");
		long id = long.Parse(Regex.Replace(data[0], "[^0-9]", ""));
		return new Player(id, data[1], data[2].ToInt(), (CharacterType)data[3].ToInt());
	}
}

public partial class GameManager : Node
{
	string savePath = "user://save.save";
	public bool isAlone {get; set;} = false;
	public Player player {get; private set;}
	public Player otherPlayer {get; private set;}

	[Signal]public delegate void UpdateServerEventHandler();

	public void PlayerJoined(Player newPlayer, bool isServer)
	{
		if (player.id != newPlayer.id)
			otherPlayer = newPlayer;
		if (isServer)
			otherPlayer.characterType = player.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		else
			player.characterType = otherPlayer.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		if (isServer)
			EmitSignal(nameof(UpdateServer));
	}

	public void Save()
	{
		FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);

		file.StoreLine(player.ToString());
	}

	public bool Load()
	{
		if (!FileAccess.FileExists(savePath))
		{
			player = new Player();
			return false;
		}
		FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
		player = Player.FromString(file.GetAsText());
		return true;
	}

	public void Clear()
	{
		otherPlayer = null;
		isAlone = false;
	}
}
