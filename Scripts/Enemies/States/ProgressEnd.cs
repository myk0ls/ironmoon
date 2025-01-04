using Godot;
using System;

public partial class ProgressEnd : State
{
    Enemy _enemy;
    Timer timer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy = machine.GetParent<Enemy>();
        //GetTree().CreateTimer(1).Timeout += DamageBase;
        timer = _enemy.GetNode<Timer>("BaseAttackTimer");
        timer.Timeout += DamageBase;
        timer.Start();
    }

    public override void Update()
    {
        //base.Enter();
        if (!IsInstanceValid(_enemy) || !_enemy.IsAlive)
        {
            timer.Stop();
            return;
        }

    }

    public override void Exit()
    {
        base.Enter();
    }

    void DamageBase()
    {
        _enemy._base.ReceiveDamage(1);
    }
}
