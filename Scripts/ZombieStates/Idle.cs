using Godot;
using System;

public partial class Idle : State
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Enter();
    }

    public void OnEnterAgroBox(Node node)
    {
        if (node is Player)
        {
            machine.TransitionTo("Agro");
        }
    }
}
