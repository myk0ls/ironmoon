using Godot;
using System;

public partial class PathSpawner : Path3D
{
	PackedScene PathFollow;
	PackedScene BigPathFollow;
    PackedScene SmallPathFollow;
    PackedScene FlyingPathFollow;
	PackedScene WardenFollow;
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
		WardenFollow = ResourceLoader.Load<PackedScene>("res://Scenes/pathFollowWarden.tscn");

		DefaultPosition = GlobalPosition;

        PathTimer = GetNode<Timer>("PathTimer");

		//PathTimer.Timeout += AddPathFollow;
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

    public void AddPathFollow(string enemyType)
    {
        PackedScene selectedScene;

        switch (enemyType)
        {
            case "SpiderSmall":
                selectedScene = SmallPathFollow;
                break;
            case "SpiderMedium":
                selectedScene = PathFollow;
                break;
            case "SpiderBig":
                selectedScene = BigPathFollow;
                break;
            case "FlyingBot":
                selectedScene = FlyingPathFollow;
                break;
			case "Warden":
				selectedScene = WardenFollow;
				break;
            default:
                GD.PrintErr($"Unknown enemy type: {enemyType}");
                return;
        }

        PathFollow3D newPath = (PathFollow3D)selectedScene.Instantiate();
		AddChild(newPath);
		//MovePathToSide(newPath);
        GD.Print($"Spawned {enemyType} on {Name}");
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
	
	/*
	void MovePathToSide(PathFollow3D newPath)
	{
		int distance = random.Next(-100,100);
		GD.Print($"distance: {distance}");
        newPath.Position = new Vector3(newPath.Position.X + distance, newPath.Position.Y, newPath.Position.Z);
	}
	*/

	/*
    PackedScene GetPathFollow()
	{
		return SmallPathFollow;
    }
	*/
}
