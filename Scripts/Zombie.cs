using Godot;
using System;

public partial class Zombie : CharacterBody3D
{
    public float Health = 100;
    public float WalkSpeed = 1.5f;

    [Signal] public delegate void DeathEventHandler();

    PathFollow3D pathToFollow;

    public override void _Ready()
    {
        pathToFollow = GetParent<PathFollow3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        var pathProgress = pathToFollow.Progress + WalkSpeed * (float)delta;

        if (pathToFollow.ProgressRatio == 1)
        {
            QueueFree();
            GetTree().ReloadCurrentScene();
        }

        pathToFollow.Progress = pathProgress;

        Velocity = velocity;
        MoveAndSlide();
    }

    public void ReceiveDamage(float Damage)
    {
        Health -= Damage;

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        QueueFree();
        EmitSignal(nameof(Death));
        PlayerStats.Instance.GainGold(5);
        PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
    }
}
