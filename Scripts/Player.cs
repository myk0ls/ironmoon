using Godot;
using System;

public partial class Player : CharacterBody3D
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

    Camera3D Camera;
    public float SprintFOV = 80f;
    public float DefaultFOV = 75f;
	public float ADSFOV = 60f;

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Camera = GetNode<Camera3D>("Camera3D");
        PositionC = Camera.Transform.Origin.Y;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		//Handle Sprint.
		if (Input.IsActionPressed("sprint"))
		{
			Speed = SprintSpeed;
		}
		else if (Input.IsActionJustReleased("sprint"))
			Speed = DefaultSpeed;

		// Aim down sights speed reduce
		if (Input.IsActionPressed("attack2") && !Input.IsActionPressed("sprint"))
		{
			Speed = ADSSpeed;
		}
		else if (Input.IsActionJustReleased("attack2"))
            Speed = DefaultSpeed;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = (@event as InputEventMouseMotion);
            Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X / 1000 * Sensitivity, Rotation.Z);
            Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X - motion.Relative.Y / 1000 * Sensitivity, -2, 2), Camera.Rotation.Y, Camera.Rotation.Z);
        }

        if (Input.IsActionJustPressed("esc"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }

        if (Input.IsActionJustPressed("crouch"))
		{
			GD.Print(Camera.Position.ToString());
            Camera.Position = Camera.Position.Lerp(CrouchingPostion, 1);
            GD.Print(Camera.Position.ToString());

			//if (IsOnFloor())
			Speed = CrouchSpeed;
        }

		if (Input.IsActionJustReleased("crouch"))
		{
            Camera.Position = Camera.Position.Lerp(StandingPostion, 1);
            GD.Print(Camera.Position.ToString() + " ENDING POSITION");
			Speed = DefaultSpeed;
		}
    }
}
