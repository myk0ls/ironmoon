using Godot;
using System;

public partial class Zombie : CharacterBody3D
{
    public float Health = 100;
    public float WalkSpeed = 1.5f;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

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
    }
}
