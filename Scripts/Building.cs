using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

[GlobalClass]
public partial class Building : Node3D
{
    public int Health { get; set; } = 100;
	protected Timer AttackTimer { get; set; }
	public Area3D DetectionArea;
	Area3D CollisionArea;
	MeshInstance3D DistanceRadius;
	public RayCast3D BuildRayCast;
    ProgressBar HealthBar;
    Sprite3D HealthBarSprite;
    public CollisionShape3D TowerShape;

	Player PlayerNode;
    CustomSignals CSignals;
    PackedScene ExplosionVfx;
    PackedScene SmokeVfx;
    public bool CanAttack;
    bool IsCriticalCondition;


    [Signal] public delegate void HealthBarUpdateEventHandler();
    [Signal] public delegate void CriticalConditionEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PlayerNode = GetNode<Player>("/root/World/Player");
        AttackTimer = GetNode<Timer>("AttackTimer");
        DetectionArea = GetNode<Area3D>("Area3D");
        DistanceRadius = GetNode<MeshInstance3D>("DistanceRadius");
        BuildRayCast = GetNode<RayCast3D>("BuildRayCast");
        HealthBarSprite = GetNode<Sprite3D>("HealthBarSprite");
        HealthBar = GetNode<ProgressBar>("SubViewport/HealthBar");
        TowerShape = GetNode<CollisionShape3D>("CollisionShape3D");

        CSignals = GetNode<CustomSignals>("/root/CustomSignals");
        ExplosionVfx = ResourceLoader.Load<PackedScene>("res://Shaders/Explosion/explosion.tscn");
        SmokeVfx = ResourceLoader.Load<PackedScene>("res://Shaders/Smoke/smoke.tscn");

        HealthBar.Value = Health;
        DetectionArea.Monitoring = true;
        CanAttack = false;
        IsCriticalCondition = false;

        ToggleHealthBar();

        AttackTimer.Timeout += Fire;
        CSignals.GameModeChanged += ToggleDistanceRadius;
        CSignals.GameModeChanged += ToggleHealthBar;
        CSignals.RepairMode += ToggleHealthBar;
        HealthBarUpdate += UpdateHealth;
        CriticalCondition += ToggleCriticalCondition;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{


	}

    public virtual void Fire()
    {
    }

    public virtual void ApplyEffect()
    {
    }

    void ToggleDistanceRadius()
    {
        if (IsInstanceValid(DistanceRadius))
        {
            if (PlayerNode.currentMode == PlayerMode.Build)
            {
                DistanceRadius.Visible = true;
            }
            else
                DistanceRadius.Visible = false;
        }
    }

    void ToggleHealthBar()
    {
        var weapon = PlayerNode.WeaponController as WeaponController;
        if (IsInstanceValid(HealthBarSprite))
        {
            if (PlayerNode.currentMode == PlayerMode.Build ||
                weapon.CurrentArm.Name == "Wrench"
                )
            {
                HealthBarSprite.Visible = true;
            }
            else
                HealthBarSprite.Visible = false;
        }
    }

    public void Remove()
    {
        QueueFree();
        CSignals.GameModeChanged -= ToggleDistanceRadius;
        GD.Print("REMOVED");
    }

    public void ReceiveDamage(int Damage)
    {
        Health -= Damage;
        EmitSignal(nameof(HealthBarUpdate));

        if (Health <= 50 && !IsCriticalCondition)
        {
            EmitSignal(nameof(CriticalCondition));
        }

        if (Health <= 0)
        {
            VfxManager.Instance.Play("Explotion", this, TowerShape.Position);
            GetTree().CreateTimer(1).Timeout += QueueFree;
        }
    }

    public void ReceiveRepair(int RepairAmount)
    {
        Health += RepairAmount;
        if (Health > 100)
        {
            Health = 100;
        }

        if (Health > 50 && IsCriticalCondition)
            ToggleCriticalCondition();

        EmitSignal(nameof(HealthBarUpdate));
        SfxManager.Instance.Play("WrenchRepair", this);
    }

    void UpdateHealth()
    {
        HealthBar.Value = Health;
    }

    void ToggleCriticalCondition()
    {
        if (!IsCriticalCondition)
        {
            IsCriticalCondition = true;
            VfxManager.Instance.PlayLoop("Smoke", this, TowerShape.Position);
        }
        else
        {
            IsCriticalCondition = false;
            VfxManager.Instance.Stop("Smoke", this);
        }

    }
}
