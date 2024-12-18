using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

public partial class VfxManager : Node
{
    public static VfxManager Instance { get; private set; }

    private Dictionary<string, Vfx> vfxs = new Dictionary<string, Vfx>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
        GD.Print("VEIKIA VFX");

        foreach (Node3D child in GetChildren())
        {
            vfxs.Add(child.Name, (Vfx)child);
            //GD.Print(child.Name);
        }
    }

    public override void _Process(double delta)
    {
    }

    /// <summary>
    /// Method for playing a vfx
    /// </summary>
    /// <param name="name">Vfx name in the scene</param>
    /// <param name="node">Parent node</param>
    /// <param name="location">Exact location for adding child</param>
    public void Play(string name, Node node, Vector3 location)
    {
        if (!vfxs.ContainsKey(name))
            return;

        Vfx duplicate = (Vfx)vfxs[name].Duplicate();
        duplicate.Position = location;
        node.AddChild(duplicate);
        duplicate.Activate();
    }

    /// <summary>
    /// Method for looping a vfx
    /// </summary>
    /// <param name="name">Vfx name in the scene</param>
    /// <param name="node">Parent node</param>
    /// <param name="location">Exact location for adding child</param>
    public void PlayLoop(string name, Node node, Vector3 location)
    {
        if (!vfxs.ContainsKey(name))
            return;

        Vfx duplicate = (Vfx)vfxs[name].Duplicate();
        duplicate.Position = location;
        node.AddChild(duplicate);
        duplicate.ActivateContinous();
    }

    /// <summary>
    /// Method for stoping a looped vfx
    /// </summary>
    /// <param name="name">Vfx name in the scene</param>
    /// <param name="node">Parent node</param>
    public void Stop(string name, Node node)
    {
        if (!vfxs.ContainsKey(name))
            return;

        var vfx = node.GetNode<Vfx>(name);
        vfx.Deactivate();
    }
}
