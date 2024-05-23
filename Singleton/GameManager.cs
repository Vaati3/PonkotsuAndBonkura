using Godot;
using System;
using System.Collections.Generic;

public class Player {
	public long id;
	public string name;
	public int progression;
	public CharacterType characterType;
	public Player()
	{
		id = 0;
		name = "";
		progression = 0;
		characterType = CharacterType.Ponkotsu;
	}
    public override string ToString()
    {
        return base.ToString();
    }
	static public Player FromString(string str)
	{
		return new Player();
	}
}

public partial class GameManager : Node
{
	string savePath = "user://save.save";
	public Player player {get; private set;}
	public Player otherPlayer {get; private set;}

	public override void _Ready()
	{

	}

	public void PlayerJoined(Player newPlayer, bool isServer)
	{
		if (player.id != newPlayer.id)
			otherPlayer = player;
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
