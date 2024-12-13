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
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		//PathFollow = GetNode<PathFollow3D>("PathFollow3D");
		PathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawn.tscn");
		BigPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawnBigSpider.tscn");
		SmallPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawnSmallSpider.tscn");
		FlyingPathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowFlying.tscn");

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
		AddChild(newPath);
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
	
    PackedScene GetPathFollow(char a)
	{
		return FlyingPathFollow;
    }

}
