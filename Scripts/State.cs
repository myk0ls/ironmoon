using Godot;
using System;

[GlobalClass]
public partial class State : Node
{
    // Called when the node enters the scene tree for the first time.
    public StateMachine machine;
    public Player PlayerNode;
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        PlayerNode = GetNode<Player>("/root/World/Player");
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {

    }
    public virtual void Enter()
    {

    }
}