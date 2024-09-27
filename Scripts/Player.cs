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
	RayCast3D RayCast;
	public Timer SellTimer;
    public float SprintFOV = 80f;
    public float DefaultFOV = 75f;
	public float ADSFOV = 60f;

	Node3D Weapon;
	Node3D Build;

	PackedScene TowerScene;
	CustomSignals CSignals;

	Tower TargetTower;

	public PlayerMode currentMode {get; private set;}
    
	public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		currentMode = PlayerMode.Combat;
		Camera = GetNode<Camera3D>("Camera3D");
		RayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		SellTimer = GetNode<Timer>("SellTimer");
		Weapon = GetNode<Node3D>("Camera3D/Weapon");
        Build = GetNode<Node3D>("Build");
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");

        TowerScene = ResourceLoader.Load<PackedScene>("res://Scenes/tower.tscn");

		PositionC = Camera.Transform.Origin.Y;

		SellTimer.Timeout += FinishSellBuilding;
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

		//Handle Sprint.
		if (Input.IsActionPressed("sprint"))
		{
			Speed = SprintSpeed;
		}
		else if (Input.IsActionJustReleased("sprint"))
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
		Weapon.Hide();
		Build.Visible = true;
		GD.Print("BUILDAS");
	}

    void EnterCombatMode()
    {
        currentMode = PlayerMode.Combat;
		
		foreach (Node child in Build.GetChildren())
		{
			child.Call("Remove");

		}

		Build.Visible = false;
		Weapon.Visible = true;        
		
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
		Weapon.Call("_ProcessCombat", delta);
    }

    void _ProcessBuild(double delta)
    {
		// build a towewr
		if (Input.IsActionJustPressed("attack"))
		{
			if (Build.GetChildCount() != 0 && PlayerStats.Instance.Gold >= 100)
			{

				Tower placeTower = (Tower)Build.GetChild(0);
				placeTower.GlobalTransform = Build.GlobalTransform;

				

				placeTower.CanAttack = true;
				placeTower.Reparent(GetParent());

                foreach (Node child in Build.GetChildren())
                {
                    child.QueueFree();
                }

                PlayerStats.Instance.Gold -= 100;
                PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
                GD.Print("GOLD:" + PlayerStats.Instance.Gold);
            }
		}

		//sell a tower by starting timer
		if (Input.IsActionJustPressed("attack2"))
		{
			if (RayCast.IsColliding())
			{
				if (RayCast.GetCollider() is Tower)
				{
                    TargetTower = (Tower)RayCast.GetCollider();
                    
					if (TargetTower.CanAttack == true)
					{
						SellTimer.Start();
						GD.Print("PRADEJO SUDA");
					}
				}
			}
		}
		
		// cancel sell
		if (Input.IsActionJustReleased("attack2") && !SellTimer.IsStopped() && (RayCast.GetCollider() is not Tower))
		{
			SellTimer.Stop();
		}

        // if you look away mid sell
        if (Input.IsActionPressed("attack2"))
		{
            if (RayCast.GetCollider() is not Tower)
				SellTimer.Stop();

        }

		//pick first tower
        if (Input.IsActionJustPressed("one"))
        {
            GD.Print("TOWER1");
            TestTower();
        }

    }

	void TestTower()
	{
		Tower newTower = (Tower)TowerScene.Instantiate();
		newTower.AxisLockAngularZ = true;
		newTower.AxisLockLinearZ = true;
		Build.AddChild(newTower);
    }

	void StartSellBuilding()
	{
	}

	void FinishSellBuilding()
	{
		if (RayCast.IsColliding() && Input.IsActionPressed("attack2"))
		{
			if (RayCast.GetCollider() == TargetTower)
			{
				Tower collider = (Tower)RayCast.GetCollider().Call("Remove");

				PlayerStats.Instance.Gold += 100;
				PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
                GD.Print("GOLD:" + PlayerStats.Instance.Gold);
                GD.Print("BAIGE SUDA");
            }
        }
	}
}

public enum PlayerMode
{
	Combat,
	Build
}
