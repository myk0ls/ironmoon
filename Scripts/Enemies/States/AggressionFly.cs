using Godot;
using System;

public partial class AggressionFly : State
{
    Enemy _enemy;
    Timer AttackTimer;

    Building Target = null;

    MeshInstance3D Barrel;
    PackedScene RocketScene;
    Node3D WorldNode;

    bool IsAttacking = false;

    public override void Enter()
    {
        GD.Print("pasikeite i agro");
        _enemy = machine.GetParent<Enemy>();
        AttackTimer = machine.GetParent<Enemy>().GetNode<Timer>("AttackTimer");
        Barrel = _enemy.Model.GetNode<MeshInstance3D>("Armature/Skeleton3D/Barrel/Barrel2");
        RocketScene = ResourceLoader.Load<PackedScene>("res://Scenes/rocket.tscn");
        WorldNode = GetNode<Node3D>("/root/World");

        AttackTimer.Timeout += Shoot;
        _enemy.Death += AttackTimer.Stop;
    }

    public override void Update()
    {
        //base.Enter();
        if (!IsInstanceValid(_enemy) || !_enemy.IsAlive)
            return;

        var pathProgress = _enemy.PathToFollow.Progress + _enemy.EnemyStats.WalkSpeed * (float)GetProcessDeltaTime();

        if (_enemy.PathToFollow.ProgressRatio == 1)
        {
            machine.TransitionTo("ProgressEnd");
        }

        _enemy.PathToFollow.Progress = pathProgress;
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

    public void Shoot()
    {
        Barrel.LookAt(PlayerNode.GlobalPosition, Vector3.Up, true);
        GD.Print("jis ziuri i tave");
        var Rocket = RocketScene.Instantiate() as Rocket;
        Rocket.SetTarget(PlayerNode);
        WorldNode.AddChild(Rocket);
        Rocket.GlobalPosition = _enemy.GlobalPosition;
    }
}