using Godot;
using System;

public partial class Progress : State
{
    Enemy _enemy;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy = machine.GetParent<Enemy>();
    }

    public override void Update()
    {
        //base.Enter();
        if (!IsInstanceValid(_enemy) || !_enemy.IsAlive)
            return;
        
        var pathProgress = _enemy.PathToFollow.Progress + _enemy.EnemyStats.WalkSpeed * (float)GetProcessDeltaTime();

        if (_enemy.PathToFollow.ProgressRatio == 1)
        {
            _enemy.QueueFree();
        }

        _enemy.PathToFollow.Progress = pathProgress;
    }

    public override void Exit()
    {
        base.Enter();
    }

    public void OnEnterAgroBox(Node node)
    {
        if (node is Building)
        {
            GD.Print("RADO BUILDINGAAA");
            _enemy.TargetNode = (Node3D)node;
            machine.TransitionTo("Aggression");
        }
    }
}
