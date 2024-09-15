using Godot;
using System;

public partial class Bullet : Node3D
{
	Vector3 Direction;
	Node3D Target;

	float Speed = 5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Target == null)
			QueueFree();

		Direction = (Target.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		Vector3 movement = Direction * Speed * (float)delta;

		this.Translate(movement);
	}
	public void OnBulletEntered(Node node)
	{
		if (node.IsInGroup("enemy"))
		{
			node.Call("ReceiveDamage", 34);
		}

		QueueFree();
	}

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    public void SetTarget(Node3D target)
	{
		Target = target;
	}
}
