using Godot;
using System;

public partial class DamageNumber : Node3D
{
    public Label3D DamageLabel;
	AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		DamageLabel = GetNode<Label3D>("DamageLabel");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		animationPlayer.AnimationFinished += (rise_and_fade) => Remove();

        animationPlayer.Play("rise_and_fade");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Remove()
	{
		QueueFree();
	}
}
