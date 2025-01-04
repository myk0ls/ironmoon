using Godot;
using System;

public partial class Bullet : Node3D
{
	Vector3 Direction;
	Node3D Target;
	Zombie Zomb;
	Vector3 TargetOrigin; 

	float Speed = 9f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		if (Target != null && IsInstanceValid(Target))
		{
			TargetOrigin = Target.GlobalTransform.Origin;
			//Zomb = Target as Zombie;
		}

		//Zomb.Death += QueueFree;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Target == null 
			&& IsInstanceValid(Target) == false)
		{
			QueueFree();
		}

		
		if (IsInstanceValid(Target))
		{
			//Direction = (Target.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
            Direction = (Target.GlobalPosition - GlobalPosition).Normalized();
            Vector3 movement = Direction * Speed * (float)delta;

            //GlobalPosition = movement;

            //Vector3 LookDir = GlobalPosition.DirectionTo(Target.GlobalPosition);
            //Rotation = LookDir;

            //LookAt(GlobalPosition + movement, Vector3.Up);
            //LookAt(Target.GlobalPosition, Vector3.Up);

			//Translate(movement);

			//GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, (float)0.03);
			//Translate(Target.GlobalPosition * Speed * (float)delta);
			GlobalTranslate(movement);
		}
		else
			QueueFree();
	}
	public void OnBulletEntered(Node node)
	{
		if (node.IsInGroup("enemy"))
		{
			node.Call("ReceiveDamage", 24);
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

	public void Remove()
	{
		QueueFree();
		//Zomb.Death -= Remove;
	}
}
