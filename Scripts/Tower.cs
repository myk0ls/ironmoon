using Godot;
using System;

public partial class Tower : Node3D
{
	Timer AttackTimer;
	Node3D Head;
	PackedScene Bullet;
	Area3D DetectionArea;

	Node3D TargetNode;
	Zombie TargetZombie;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
		Head = GetNode<Node3D>("Head");
		AttackTimer = GetNode<Timer>("AttackTimer");
		DetectionArea = GetNode<Area3D>("Area3D");

		TargetNode = null;
		TargetZombie = null;

		AttackTimer.Timeout += Fire;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node3D node)
	{
		if (TargetNode is null && node.IsInGroup("enemy"))
		{
			TargetNode = node;
			TargetZombie = (Zombie)TargetNode;
            TargetZombie.Death += RemoveTarget;
			TargetZombie.Death += ScanForTarget;
            Fire();
			AttackTimer.Start();
		}
	}

    public void OnBodyExited(Node3D node)
    {
		if (node == TargetNode)
		{
			RemoveTarget();
			TargetZombie = null;
        }
    }

	void RemoveTarget()
	{
		if (TargetNode != null)
		{
			TargetNode = null;
            TargetZombie.Death -= RemoveTarget;
            TargetZombie.Death -= ScanForTarget;
        }
    }

	public void Fire()
	{
		Bullet newBullet = (Bullet)Bullet.Instantiate();
		newBullet.SetTarget(TargetNode);
		Head.AddChild(newBullet);
	}

	void ScanForTarget()
	{
		Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

		if (bodies != null)
		{
			for (int i = 0;i < bodies.Count;i++)
			{
				GD.Print(bodies[i].ToString());
				if (bodies[i] is CharacterBody3D)
				{
                    TargetNode = (Node3D)bodies[i];
                    TargetZombie = (Zombie)TargetNode;
					break;
                }
			}
		}
		else
            AttackTimer.Stop();
    }
}
