using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public float Speed { get; set; } = 5f;
	public const float DefaultSpeed = 5.0f;
	public const float SprintSpeed = 8f;
    public const float CrouchSpeed = 2.5f;
    public const float ADSSpeed = 3.5f;
    public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 1.1f;
	float PositionC;
	Vector3 StandingPostion = new Vector3(0, (float)0.5, 0);
    Vector3 CrouchingPostion = new Vector3(0, (float)0.15, 0);

    public Camera3D Camera;
	public RayCast3D RayCast;
	public Timer SellTimer;
    public float SprintFOV = 85.0f;
    public float DefaultFOV = 75.0f;
	public float ADSFOV = 60.0f;
	const float SprintLerp = 20f;

	public Node3D WeaponController;
	Node3D Builder;

	CustomSignals CSignals;

	
	public PlayerMode currentMode {get; private set;}
    

	public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		currentMode = PlayerMode.Combat;
		Camera = GetNode<Camera3D>("Camera3D");
		RayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		SellTimer = GetNode<Timer>("SellTimer");
		WeaponController = GetNode<Node3D>("Camera3D/WeaponController");
        Builder = GetNode<Node3D>("Builder");
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
		
		PositionC = Camera.Transform.Origin.Y;

		CSignals.ShopView += OnShopMode;
        CSignals.ShopViewClose += OffShopMode;
    }

    public override void _Process(double delta)
    {
		if (currentMode == PlayerMode.Combat)
			_ProcessCombat(delta);
		else if (currentMode == PlayerMode.Build)
			_ProcessBuild(delta);
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

		
        if (Input.IsActionPressed("sprint") && inputDir != Vector2.Zero && !Input.IsActionPressed("attack2"))
        {
            Speed = SprintSpeed;
            Camera.Fov = Mathf.Lerp(Camera.Fov, SprintFOV, SprintLerp * (float)delta);
			//GD.Print(Camera.Fov);

		}
        else if (Input.IsActionJustReleased("sprint"))
        {
            Speed = DefaultSpeed;
            Camera.Fov = Mathf.Lerp(Camera.Fov, DefaultFOV, SprintLerp * (float)delta);
            //GD.Print(Camera.Fov);
        }
		

        Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && currentMode != PlayerMode.Shop)
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

		if (Input.IsActionJustPressed("switch_mode"))
		{
			ToggleMode();
		}
    }

	void ToggleMode()
	{
		if (currentMode == PlayerMode.Combat)
		{
			EnterBuildMode();
		}
		else
		{
			EnterCombatMode();
		}
		
		CSignals.EmitSignal(nameof(CSignals.GameModeChanged));
	}

	void EnterBuildMode()
	{
		currentMode = PlayerMode.Build;
		WeaponController.Hide();
		Builder.Visible = true;
		GD.Print("BUILDAS");
	}

    void EnterCombatMode()
    {
        currentMode = PlayerMode.Combat;
		
		foreach (Node child in Builder.GetChildren())
		{
			child.Call("Remove");

		}

		Builder.Visible = false;
		WeaponController.Visible = true;        
		
		GD.Print("COMBATAS");
    }

	void _ProcessCombat(double delta)
	{
        // Aim down sights speed reduce
        if (Input.IsActionPressed("attack2") && !Input.IsActionPressed("sprint"))
        {
            Speed = ADSSpeed;
        }
        else if (Input.IsActionJustReleased("attack2"))
            Speed = DefaultSpeed;

		//All Weapon calls
		WeaponController.Call("_ProcessCombat", delta);
    }

    void _ProcessBuild(double delta)
    {
		Builder.Call("_ProcessBuild", delta);
    }

	void OnShopMode()
	{
		if (currentMode != PlayerMode.Shop)
		{
			currentMode = PlayerMode.Shop;
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

	void OffShopMode()
	{
        if (currentMode == PlayerMode.Shop)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            EnterCombatMode();
        }
    }
}

public enum PlayerMode
{
	Combat,
	Build,
	Shop
}
