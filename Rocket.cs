using Godot;
using System;

public partial class Rocket : Node3D
{
    Vector3 Direction;
    Node3D Target;
    Vector3 TargetOrigin;

    bool TargetAcquired = false;

    float Speed = 6f;
    public override void _Ready()
    {
        if (Target != null && IsInstanceValid(Target))
        {
            TargetOrigin = Target.GlobalTransform.Origin;
            TargetOrigin = new Vector3(TargetOrigin.X, TargetOrigin.Y - (float)0.5, TargetOrigin.Z);
        }

    }

    public override void _Process(double delta)
    {
        if (Target == null
            && IsInstanceValid(Target) == false)
        {
            QueueFree();
        }


        if (!TargetAcquired && IsInstanceValid(Target))
        {
            // Calculate and store the initial direction to the target
            Direction = (TargetOrigin - GlobalPosition).Normalized();
            TargetAcquired = true;
        }

        if (TargetAcquired)
        {
            // Continue moving in the stored direction
            Vector3 movement = Direction * Speed * (float)delta;
            GlobalTranslate(movement);
        }
    }

    public void SetTarget(Node3D target)
    {
        Target = target;
    }

    public void OnBodyEntered(Node3D node)
    {
        if (node is Player)
        {
            PlayerStats.Instance.ReceiveDamage(10);
        }

        if (node is Building)
        {
            var building = (Building)node;
            building.ReceiveDamage(10);
        }

        VfxManager.Instance.Play("Explosion", GetParent(), GlobalPosition);
        SfxManager.Instance.Play("ExplosionSound", this);
        GetTree().CreateTimer(2).Timeout += QueueFree;
    }

    public void ReceiveDamage(int damage)
    {
        GD.Print("PATAIKEI");
        VfxManager.Instance.Play("Explosion", GetParent(), GlobalPosition);
        SfxManager.Instance.Play("ExplosionSound", this);
        GetTree().CreateTimer(2).Timeout += QueueFree;
    }
}

