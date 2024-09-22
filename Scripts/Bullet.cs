using Godot;
using System;

public partial class Bullet : Node3D
{
	Vector3 Direction;
	Node3D Target;
	Vector3 TargetOrigin; 

	float Speed = 5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		if (Target != null && IsInstanceValid(Target))
		{
			TargetOrigin = Target.GlobalTransform.Origin;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Target == null &&
            IsInstanceValid(Target) == false)
		{
			QueueFree();
		}

		
		if (IsInstanceValid(Target))
		{
			Direction = (Target.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
			Vector3 movement = Direction * Speed * (float)delta;

			//GlobalPosition = movement;

			//Vector3 LookDir = GlobalPosition.DirectionTo(Target.GlobalPosition);
			//Rotation = LookDir;

			LookAt(GlobalTransform.Origin + movement, Vector3.Up);

			Translate(movement);
		}
		else
			QueueFree();
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
