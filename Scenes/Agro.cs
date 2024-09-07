using Godot;
using System;

public partial class Agro : State
{
    Zombie zombie;

    public override void Enter()
    {
        GD.Print("pasikeite i agro");
        zombie = machine.GetParent<Zombie>();
    }

    public override void Update()
    {
        Vector3 velocity = zombie.Velocity;

        Vector3 dir = PlayerNode.GlobalPosition - zombie.GlobalPosition;
        dir.Y = 0;
        dir = dir.Normalized();

        velocity.X = dir.X * zombie.WalkSpeed;
        velocity.Z = dir.Z * zombie.WalkSpeed;

        zombie.Velocity = velocity;
    }

    public override void Exit() 
    { 
        base.Exit();
    }

}
