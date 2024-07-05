using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SoundManager : Node
{
    const int playerAmount = 8;
    AudioStreamPlayer[] players;
    bool[] availablePlayers;
    Dictionary<string, AudioStream> sfx;
    Queue<AudioStream> sfxQueue;

    private void CreatePlayers()
    {
        players = new AudioStreamPlayer[playerAmount];
        availablePlayers = new bool[playerAmount];
        
        for (int i = 0; i < playerAmount; i++)
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.Finished += StreamFinished;
            AddChild(player);
            players[i] = player;
            //change bus to sfx
            availablePlayers[i] = true;
        }
    }

    private void StreamFinished()
    {
        for(int i = 0; i > playerAmount; i++)
        {
            if(!availablePlayers[i] && !players[i].Playing)
                availablePlayers[i] = true;
        }
    }

    private void  PlayStream(AudioStream stream)
    {
        for(int i = 0; i < playerAmount; i++)
        {
            if(availablePlayers[i])
            {
                availablePlayers[i] = false;
                players[i].Stream = stream;
                players[i].Play();
                return ;
            }
        }
        return;
    }

    private void LoadSFX()
    {
        DirAccess dir = DirAccess.Open("res://Sounds/SFX/");

        if (dir == null)
            return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while(fileName != "")
        {
            if (!dir.CurrentIsDir() && !fileName.Contains("import"))
                AddSFX(fileName);
            fileName = dir.GetNext();
        }
    }

    private void AddSFX(string fileName)
    {
        AudioStream audio = GD.Load<AudioStream>("res://Sounds/SFX/" + fileName);

        if (audio == null)
            return;

        sfx.Add(fileName.GetBaseName(), audio);
    }

    public override void _Ready()
    {
        CreatePlayers();
        
        sfx = new Dictionary<string, AudioStream>();
        sfxQueue = new Queue<AudioStream>();
        LoadSFX();
    }

    public override void _Process(double delta)
    {
        if (sfxQueue.Count > 0)
        {
            PlayStream(sfxQueue.Dequeue());
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void PlaySFX(string sfxName, bool rpc = false)
    {
        AudioStream audio;
        if (sfx.TryGetValue(sfxName, out audio))
        {
            sfxQueue.Enqueue(audio);
            if (rpc)
                Rpc(nameof(PlaySFX), sfxName);
        }
    }

}