using Godot;
using System;

public partial class PathSpawner : Path3D
{
	PackedScene PathFollow;
	Timer PathTimer;
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		//PathFollow = GetNode<PathFollow3D>("PathFollow3D");
		PathFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowSpawn.tscn");

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
		PathFollow3D newPath = (PathFollow3D)PathFollow.Instantiate();
		AddChild(newPath);
	}
}
