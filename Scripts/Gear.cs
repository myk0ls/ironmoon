using Godot;
using System;

public partial class Gear : RigidBody3D
{
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

	public void Interact()
	{
		//QueueFree();
		PlayerStats.Instance.GainGold(1);
		PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
		SfxManager.Instance.Play("GearPickup", GetNode<Player>("/root/World/Player"));
		CSignals.EmitSignal(nameof(CSignals.InteractNodeUpdate));
		QueueFree();
		//var audio = GetNode<AudioStreamPlayer3D>("GearPickup");
		//audio.Finished += QueueFree;
	}
}
