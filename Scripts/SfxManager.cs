using Godot;
using System;
using System.Collections.Generic;

public partial class SfxManager : Node
{
    public static SfxManager Instance { get; private set; }

    private Dictionary<string, AudioStreamPlayer3D> sfxs = new Dictionary<string, AudioStreamPlayer3D>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;

        foreach (Node child in GetChildren())
        {
            sfxs.Add(child.Name, (AudioStreamPlayer3D)child);
            GD.Print(child.Name);
        }

        foreach (KeyValuePair<string, AudioStreamPlayer3D> entry in sfxs)
        {
            entry.Value.Finished += () => QueueFree();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Play(string name, Node node)
    {
        if (!sfxs.ContainsKey(name))
            return;

        /*
        foreach (KeyValuePair<string, AudioStreamPlayer3D> entry in sfxs)
        {
            // If the current song is not the one we want to play, stop it
            if (entry.Key != name)
            {
                entry.Value.Stop();
            }
        }

        // Only play the song if it's not already playing
        if (!sfxs[name].Playing)
        {
            sfxs[name].Play();
        }
        */

        AudioStreamPlayer3D duplicate = (AudioStreamPlayer3D)sfxs[name].Duplicate();
       // duplicate.Finished += QueueFree;
        node.AddChild(duplicate);   
        node.GetNode<AudioStreamPlayer3D>(name).Play();
        GD.Print("ADDED DEATH EFFECT");
    }
}
