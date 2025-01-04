using Godot;
using System;

public partial class Base : Node3D
{
	public int Health = 1000;
	CustomSignals CSignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ReceiveDamage(int damage)
	{
		Health -= damage;

		//GD.Print($"BASE HEALTH:{Health}");

		CSignals.EmitSignal(nameof(CSignals.BaseHealthUpdate), Health);	

		if (Health <= 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
