using Godot;
using System;
using System.Net;

public partial class LaserTower : Tower
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void Fire()
	{
		base.Fire();
	}

}
