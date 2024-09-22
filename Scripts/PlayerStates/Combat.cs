using Godot;
using System;

public partial class Combat : State
{
    public float Speed { get; set; } = 5f;
    public const float DefaultSpeed = 5.0f;
    public const float SprintSpeed = 8f;
    public const float CrouchSpeed = 3.5f;
    public const float ADSSpeed = 3.5f;
    public const float JumpVelocity = 4.5f;
    public const float Sensitivity = 1.1f;
    float PositionC;
    Vector3 StandingPostion = new Vector3(0, (float)0.5, 0);
    Vector3 CrouchingPostion = new Vector3(0, (float)0.15, 0);

    public Camera3D Camera;
    public float SprintFOV = 80f;
    public float DefaultFOV = 75f;
    public float ADSFOV = 60f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        // Aim down sights speed reduce
        if (Input.IsActionPressed("attack2") && !Input.IsActionPressed("sprint"))
        {
            PlayerNode.Speed = ADSSpeed;
        }
        else if (Input.IsActionJustReleased("attack2"))
            PlayerNode.Speed = DefaultSpeed;
    }

    public override void Exit()
    {
        base.Exit(); 
    }
}
