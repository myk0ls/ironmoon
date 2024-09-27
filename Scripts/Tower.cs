using Godot;
using System;

public partial class Tower : StaticBody3D
{
	Timer AttackTimer;
	Node3D Head;
	PackedScene Bullet;
	Area3D DetectionArea;
	Area3D CollisionArea;
	MeshInstance3D DistanceRadius;

	Player PlayerNode;

	Node3D TargetNode;
	Zombie TargetZombie;

	CustomSignals CSignals;

	public bool CanAttack;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerNode = GetNode<Player>("/root/World/Player");
		Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
		Head = GetNode<Node3D>("Head");
		AttackTimer = GetNode<Timer>("AttackTimer");
		DetectionArea = GetNode<Area3D>("Area3D");
		DistanceRadius = GetNode<MeshInstance3D>("DistanceRadius");
		//CollisionArea = GetNode<Area3D>("");

		CSignals = GetNode<CustomSignals>("/root/CustomSignals");

		TargetNode = null;
		TargetZombie = null;
		CanAttack = false;

		AttackTimer.Timeout += Fire;
		CSignals.GameModeChanged += ToggleDistanceRadius;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void SetTargetNode(Node3D node)
	{
        TargetNode = node;
        TargetZombie = (Zombie)TargetNode;
        TargetZombie.Death += RemoveTarget;
        TargetZombie.Death += ScanForTarget;
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

	public void OnBodyEntered(Node3D node)
	{
		if (TargetNode is null && node.IsInGroup("enemy"))
		{
			SetTargetNode(node);
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


	public virtual void Fire()
	{
		if (TargetNode != null && CanAttack == true)
		{
			Bullet newBullet = (Bullet)Bullet.Instantiate();
			newBullet.SetTarget(TargetNode);
			Head.AddChild(newBullet);
		}
	}

	void ScanForTarget()
	{
		Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

		if (bodies != null)
		{
			for (int i = 0;i < bodies.Count;i++)
			{
				GD.Print(bodies[i].ToString());
				if (bodies[i] is CharacterBody3D &&
					bodies[i].IsInGroup("enemy"))
				{
					SetTargetNode(bodies[i]);
					break;
                }
			}
		}
		else
            AttackTimer.Stop();
    }

	public void Remove()
	{
		QueueFree();
        CSignals.GameModeChanged -= ToggleDistanceRadius;
		GD.Print("REMOVED");
    }

	void ToggleDistanceRadius()
	{
		if (IsInstanceValid(DistanceRadius))
		{
			if (PlayerNode.currentMode == PlayerMode.Build)
			{
				DistanceRadius.Visible = true;
			}
			else
				DistanceRadius.Visible = false;
		}
    }
}
