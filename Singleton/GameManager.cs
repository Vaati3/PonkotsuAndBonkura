using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Player {
	public long id;
	public string name;
	public int progression;
	public CharacterType characterType;
	public Player(long id = 0, string name = "", int progression = 0, CharacterType characterType = CharacterType.Ponkotsu)
	{
		this.id = id;
		this.name = name;
		this.progression = progression;
		this.characterType = characterType;
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
	public Player player {get; private set;}
	public Player otherPlayer {get; private set;}

	public void PlayerJoined(Player newPlayer, bool isServer)
	{
		GD.Print(player.id + " " + newPlayer.id);
		if (player.id != newPlayer.id)
			otherPlayer = newPlayer;
		if (isServer)
			otherPlayer.characterType = player.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
		else
			player.characterType = otherPlayer.characterType == CharacterType.Ponkotsu ? CharacterType.Bonkura : CharacterType.Ponkotsu;
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
}
