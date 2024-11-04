using Godot;
using System;
using System.Security;
using System.Collections.Generic;
using static Godot.WebSocketPeer;

public partial class StateMachine : Node
{
    [Export] public NodePath initialNode;

    private Dictionary<string, State> states;
    private State currentState;



    public override void _Ready()
    {
        states = new Dictionary<string, State>();
        foreach (Node node in GetChildren())
        {
            if (node is State s)
            {
                states[node.Name] = s;
                s.machine = this;
                s._Ready();
                s.Exit();
            }
        }

        currentState = GetNode<State>(initialNode);
        currentState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        currentState._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
            currentState.Update();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        currentState._UnhandledInput(@event);
    }

    public void TransitionTo(string key)
    {
        if (!states.ContainsKey(key) || currentState == states[key])
            return;

        currentState.Exit();
        currentState = states[key];
        currentState.Enter();
    }
}
