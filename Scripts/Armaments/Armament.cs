using Godot;
using System;

public partial class Armament : Node3D
{
	[Export]
	public ArmamentStats ArmStats { get; set; }
	[Export]
	public Node3D ArmModel { get; set; }
	public AnimationPlayer ArmPlayer { get; set; }

	public AnimationTree ArmTree { get; set; }

    public AnimationNodeStateMachinePlayback AnimStateMachine { get; set; }

    [Export] 
	public Vector3 DefaultPosition;
    
	[Export]
	public Vector3 ADSPosition;

	[Export]
	public bool IsAuto;
	

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		ArmPlayer = ArmModel.GetNode<AnimationPlayer>("AnimationPlayer");
		ArmTree = ArmModel.GetNode<AnimationTree>("AnimationTree");
		AnimStateMachine = (AnimationNodeStateMachinePlayback)ArmModel.GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
