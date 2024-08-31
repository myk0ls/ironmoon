using Godot;
using System;

public partial class Dummy : CharacterBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
		QueueFree();
	}
}
