using Godot;
using System;

public partial class Aggression : State
{
    Enemy _enemy;
    Timer AttackTimer;

    Building Target = null;

    bool IsAttacking = false;

    public override void Enter()
    {
        GD.Print("pasikeite i agro");
        _enemy = machine.GetParent<Enemy>();
        AttackTimer = machine.GetParent<Enemy>().GetNode<Timer>("AttackTimer");

        AttackTimer.Timeout += Dead;
        _enemy.Death += AttackTimer.Stop;
    }

    public override void Update()
    {
        /*
        if (!IsInstanceValid(Target))
        {
            //machine.TransitionTo("Progress");
            //_enemy.animationStateMachine.Travel("Walk");
            //_enemy.EmitSignal(nameof(_enemy.Death));
            //return;
            IsAttacking = false;
            machine.TransitionTo("Progress");
        }
        
        */

        if (IsAttacking)
            return;

        if (!IsInstanceValid(_enemy.TargetNode))
        {
            machine.TransitionTo("Progress");
        }

        Vector3 velocity = _enemy.Velocity;

        Vector3 dir = _enemy.TargetNode.GlobalPosition - _enemy.GlobalPosition;
        dir.Y = 0;
        dir = dir.Normalized();

        _enemy.LookAt(_enemy.TargetNode.GlobalPosition, Vector3.Up);

        velocity.X = dir.X * _enemy.EnemyStats.WalkSpeed;
        velocity.Z = dir.Z * _enemy.EnemyStats.WalkSpeed;

        _enemy.Velocity = velocity;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void OnBuildingBoxEntered(Node node)
    {
        if (node is Building)
        {
            var building = node as Building;
            _enemy.animationStateMachine.Travel("Die");
            //building.ReceiveDamage(100);
            // _enemy.TargetNode = null;
            // _enemy.EmitSignal(nameof(_enemy.Death));

            IsAttacking = true;

            AttackTimer.Timeout += () => building.ReceiveDamage(25);
            AttackTimer.Start();
        }
    }

    void Dead()
    {
        _enemy.EmitSignal(nameof(_enemy.Death));
        var BuildingBox = _enemy.GetNode<CollisionShape3D>("BuildingBox/CollisionShape3D");
        BuildingBox.Disabled = true;   
    }
}
