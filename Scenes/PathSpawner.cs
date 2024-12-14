using Godot;
using System;

public partial class PathSpawner : Path3D
{
	PackedScene PathFollow;
	PackedScene BigPathFollow;
    PackedScene SmallPathFollow;
    PackedScene FlyingPathFollow;
    Timer PathTimer;
	Random random = new Random();

	Vector3 DefaultPosition;

	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		//PathFollow = GetNode<PathFollow3D>("PathFollow3D");
		PathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawn.tscn");
		BigPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawnBigSpider.tscn");
		SmallPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawnSmallSpider.tscn");
		FlyingPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowFlying.tscn");

		DefaultPosition = GlobalPosition;

        PathTimer = GetNode<Timer>("PathTimer");

		PathTimer.Timeout += AddPathFollow;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void AddPathFollow()
	{
		GD.Print("spawning");
		PathFollow3D newPath = (PathFollow3D)GetPathFollow().Instantiate();
		//MovePathToSide();
		AddChild(newPath);
		//GlobalPosition = DefaultPosition;
	}

    
	PackedScene GetPathFollow()
	{
		float value = random.Next(1, 11);
		if (value < 8)
			return PathFollow;
		else if (value == 8)
			return SmallPathFollow;
		else if (value == 9)
			return BigPathFollow;
		else return FlyingPathFollow;
	}
	
	void MovePathToSide()
	{
		int distance = random.Next(-1,1);
		GlobalPosition = new Vector3(GlobalPosition.X + distance, GlobalPosition.Y, GlobalPosition.Z);
	}
	
	/*
    PackedScene GetPathFollow()
	{
		return FlyingPathFollow;
    }
	*/
}
