using Godot;
using System;

public partial class WeaponController : Node3D
{
    const float ADSLerp = 20;

    /// <summary>
    /// weapon bob and sway parameters
    /// </summary>
	Vector3 MouseMov;
    float SwayThreshold = 10;
    float SwaySens = 2f;
    float SwayAmount = 0.15f;
    float SwaySpeed = 5;

    float StartY;
    float BobSpeed = 0.002f;
    float BobAmount = 0.01f;
    Vector3 DefaultRotation;
    //

    private bool _shakeTriggered = false;
    private float shakeAmount = 0.004f; // Amount of shake
    private float shakeDuration = 0.02f; // Duration of shake in seconds
    Vector3 _originalRotation;

    public float CurrentRecoil = 0f;
    public float _shakeTimer = 0f;
    private Vector3 _shakeOffset;


    //arm array
    Godot.Collections.Array<Armament> ArmArray = new Godot.Collections.Array<Armament>();
    int CurrentWeaponIndex { get; set; } = -1;
    public Armament CurrentArm = null;

    PackedScene BulletDecal;

    Player player;
    RayCast3D rayCast;
    Camera3D camera3D;
    CustomSignals CSignals;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (Node node in GetChildren())
        {
            if (node is Armament)
            {
                ArmArray.Add((Armament)node);
                Armament arm = node as Armament;
                //arm.ArmPlayer.AnimationStarted += (shoot) => Attack(shoot);
                //arm.ArmPlayer.AnimationStarted += (shoot) => ApplyRecoil(shoot);
            }
        }

        camera3D = (Camera3D)GetParent();
        player = GetNode<Player>("/root/World/Player");
        rayCast = GetParent().GetNode<RayCast3D>("RayCast3D");
        CSignals = GetNode<CustomSignals>("/root/CustomSignals");
        BulletDecal = ResourceLoader.Load<PackedScene>("res://Scenes/bullet_hole.tscn");

        //CurrentArm = ArmArray[CurrentWeaponIndex];
        SwitchWeapon(0);



        _originalRotation = camera3D.Transform.Origin;
        DefaultRotation = Rotation;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void _ProcessCombat(double delta)
    {
        CurrentRecoil = (float)Mathf.Lerp(CurrentRecoil, 0, CurrentArm.ArmStats.RecoilSpeed * delta);

        ApplyCameraRecoil();


        if (Input.IsActionJustPressed("one")
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
            && ArmArray[0].IsOwned
            )
        {
            SwitchWeapon(0);
            CSignals.EmitSignal(nameof(CSignals.RepairMode));
        }

        if (Input.IsActionJustPressed("two")
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
            && ArmArray[1].IsOwned
            )
        {
            SwitchWeapon(1);
            CSignals.EmitSignal(nameof(CSignals.RepairMode));
        }

        if (Input.IsActionJustPressed("three")
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
            && ArmArray[2].IsOwned
            )
        {
            SwitchWeapon(2);
            CSignals.EmitSignal(nameof(CSignals.RepairMode));
        }

        if (Input.IsActionJustPressed("four")
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
            && ArmArray[3].IsOwned
            )
        {
            SwitchWeapon(3);
            CSignals.EmitSignal(nameof(CSignals.RepairMode));
        }


        if (Input.IsActionJustPressed("sprint") && CurrentArm.ArmPlayer.IsPlaying() == false 
            && !Input.IsActionPressed("attack2"))
        {
            //CurrentArm.ArmPlayer.Play("sprintStart");
            CurrentArm.AnimStateMachine.Travel("sprint");
        }

        if (Input.IsActionJustReleased("sprint") && !Input.IsActionPressed("attack2"))
        {
            CurrentArm.AnimStateMachine.Travel("idle");
        }

        if (Input.IsActionJustPressed("reload")
            )
        {
            if (CurrentArm.ArmStats.ClipSize < CurrentArm.ArmStats.TotalClipSize
                && PlayerStats.Instance.GetAmmo(CurrentArm.Name) > 0
                && CurrentArm.AnimStateMachine.GetCurrentNode() != "shoot"
                && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
                )
            {
                SfxManager.Instance.Play(CurrentArm.Name + "Reload", this);
                CurrentArm.AnimStateMachine.Travel("reload");
                //Reload();
            }
        }


        if (CurrentArm.IsAuto == false
            && CurrentArm.ArmStats.ClipSize > 0)
        {
            if (Input.IsActionJustPressed("attack"))
            {
                CurrentArm.AnimStateMachine.Travel("shoot");
            }
        }
        else if (CurrentArm.IsAuto == true
            && CurrentArm.ArmStats.ClipSize > 0)
        {
            if (Input.IsActionPressed("attack"))
            {
                CurrentArm.AnimStateMachine.Travel("shoot");
            }
        }
        else if (CurrentArm.ArmStats.ClipSize == 0
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload")
        {
            if (Input.IsActionJustPressed("attack"))
            {
                SfxManager.Instance.Play(CurrentArm.Name + "Reload", this);
                CurrentArm.AnimStateMachine.Travel("reload");
            }
        }
        
        
        //Aim down sights
        if (Input.IsActionPressed("attack2") 
            && !Input.IsActionPressed("sprint") 
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "sprint"
            && CurrentArm.AnimStateMachine.GetCurrentNode() != "reload"
            )
        {
            Transform3D newTransform = CurrentArm.Transform;
            newTransform.Origin = newTransform.Origin.Lerp(CurrentArm.ADSPosition, ADSLerp * (float)delta);
            CurrentArm.Transform = newTransform;
            SwayAmount = 0.05f;

            camera3D.Fov = Mathf.Lerp(camera3D.Fov, player.ADSFOV, ADSLerp * (float)delta);
        }
        else if (CurrentArm.AnimStateMachine.GetCurrentNode() == "idle"
            || CurrentArm.AnimStateMachine.GetCurrentNode() == "reload"
            )
        {
            Transform3D newTransform = CurrentArm.Transform;
            newTransform.Origin = newTransform.Origin.Lerp(CurrentArm.DefaultPosition, ADSLerp * (float)delta);
            CurrentArm.Transform = newTransform;

            SwayAmount = 0.15f;

            camera3D.Fov = Mathf.Lerp(camera3D.Fov, player.DefaultFOV, ADSLerp * (float)delta); 
        }
        

        // Sway and bob weapon
        WeaponSway(delta);
        WeaponBob(delta);

        //CameraShake(delta);

    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion motion = @event as InputEventMouseMotion;
            MouseMov = new Vector3(-motion.Relative.Y * SwaySens, -motion.Relative.X * SwaySens, 0);
        }
    }

    public void Attack(StringName animName)
    {
        if (animName == "shoot" 
            && CurrentArm.ArmStats.ClipSize > 0
            )
        {
            CurrentArm.ArmStats.ClipSize -= 1;
            GD.Print(CurrentArm.ArmStats.ClipSize);

            SfxManager.Instance.Play(CurrentArm.Name + "Shot",this);
            CSignals.EmitSignal(nameof(CSignals.UpdateAmmoLabel));

            CameraShakeAsync();

            if (rayCast.IsColliding() && rayCast != null
                && rayCast.GetCollider() != null
                )
            {
                var collider = rayCast.GetCollider() as Node3D;
                if (collider.IsInGroup("enemy"))
                {
                    var shapeIndex = rayCast.GetColliderShape();
                    GD.Print(shapeIndex);

                    switch (shapeIndex)
                    {
                        case 0:
                            collider.Call("ReceiveDamage", CurrentArm.ArmStats.BaseDamage);
                            DamageNumbers.Instance.DisplayNumber((int)CurrentArm.ArmStats.BaseDamage, GetAdjustedCollisionPoint(), false);
                            break;

                        case 1:
                            collider.Call("ReceiveDamage", CurrentArm.ArmStats.BaseDamage + 20);
                            DamageNumbers.Instance.DisplayNumber((int)CurrentArm.ArmStats.BaseDamage + 20, GetAdjustedCollisionPoint(), true);
                            break;
                    }
                }

                AddBulletDecal();
            }

        }
            

    }

    void WeaponSway(double delta)
    {
        // sway
        var mouseMovLength = MouseMov.Length();

        if (mouseMovLength > SwayThreshold || mouseMovLength < -SwayThreshold)
        {
            Rotation = Rotation.Lerp(MouseMov.Normalized() * SwayAmount + DefaultRotation, SwaySpeed * (float)delta);
        }
        else
            Rotation = Rotation.Lerp(DefaultRotation, SwaySpeed * (float)delta * 2);

    }

    void WeaponBob(double delta)
    {
        if (Input.IsActionPressed("attack2"))
        {
            Position = new Vector3(Position.X, (float)StartY, Position.Z);
        }

        // bob
        if (player.IsOnFloor() && player.Velocity.Length() != 0 && !Input.IsActionPressed("attack2"))
        {
            var time = (float)Time.GetTicksMsec();

            float velocityLength = player.Velocity.Length();
            float angle = time * BobSpeed * velocityLength;
            float bobOffset = (float)Math.Sin(angle) * BobAmount + StartY;

            Position = new Vector3(Position.X, bobOffset, Position.Z);
        }
        else
            Position = Position.Lerp(new Vector3(Position.X, (float)StartY, Position.Z), BobSpeed * (float)delta * 10000);
    }

    Vector3 GetAdjustedCollisionPoint()
    {
        Vector3 collisionPoint = rayCast.GetCollisionPoint();
        Vector3 directionToPlayer = (player.GlobalPosition - collisionPoint).Normalized();
        Vector3 adjustedCollisionPoint = collisionPoint + directionToPlayer * (float)1;

        return adjustedCollisionPoint;
    }

    void SwitchWeapon(int newIndex)
    {
        if (CurrentWeaponIndex != newIndex)
        {
            if (CurrentArm != null)
            {
                CurrentArm.Visible = false;
            }

            
            CurrentArm = ArmArray[newIndex];
            CurrentWeaponIndex = newIndex;
            CurrentArm.Visible = true;
            CurrentArm.AnimStateMachine.Travel("idle");
            CSignals.EmitSignal(nameof(CSignals.UpdateAmmoLabel));

            rayCast.TargetPosition = new Vector3(0, 0, -CurrentArm.ArmStats.Range);
        }
    }

    void ApplyRecoil(StringName animName)
    {
        if (animName == "shoot")
        {
            CurrentRecoil = Mathf.Clamp(CurrentRecoil + CurrentArm.ArmStats.RecoilAmount,
                0, CurrentArm.ArmStats.MaxRecoil);
        }
    }

    void ApplyCameraRecoil()
    {
        Vector3 rotation = camera3D.RotationDegrees;
        rotation.X = Mathf.Lerp(rotation.X, rotation.X + CurrentRecoil, 0.5f);

        camera3D.RotationDegrees = rotation;
    }
    
    void CameraShake(double delta)
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= (float)delta;
            _shakeTriggered = true;

            shakeAmount = CurrentArm.ArmStats.ShakeAmount;

            // Generate small random offsets for shake
            _shakeOffset = new Vector3(
                (float)GD.RandRange(-shakeAmount, shakeAmount),
                (float)GD.RandRange(-shakeAmount, shakeAmount),
                //(float)GD.RandRange(-shakeAmount, shakeAmount)
                0
            );

            // Apply the shake effect as an offset
            //camera3D.Rotation = camera3D.Rotation + _shakeOffset;
            camera3D.RotateObjectLocal(_shakeOffset.Normalized(), 0);
        }
        else if (_shakeTriggered)
        {
            // Gradually reduce the shake offset when shake ends
            _shakeOffset = _shakeOffset.Lerp(Vector3.Zero, (float)delta * 5f);
            //camera3D.Rotation = camera3D.Rotation + _shakeOffset;
            camera3D.RotateObjectLocal(_shakeOffset.Normalized(), 0);

            // Stop applying shake adjustments once the offset is close to zero
            if (_shakeOffset.Length() < 0.01f)
            {
                _shakeOffset = Vector3.Zero;
                _shakeTriggered = false;
            }
        }
    }

    void TriggerShake()
    {
        _shakeTimer = CurrentArm.ArmStats.ShakeDuration;
        _shakeTriggered = true;
    }

    public async void CameraShakeAsync()
    {
        Transform3D initial_transform = camera3D.Transform;
        float elapsed_time = 0.0f;

        while (elapsed_time < CurrentArm.ArmStats.ShakeDuration)
        {
            var offset = new Vector3(
                (float)GD.RandRange(-CurrentArm.ArmStats.ShakeAmount, CurrentArm.ArmStats.ShakeAmount),
                (float)GD.RandRange(-CurrentArm.ArmStats.ShakeAmount, CurrentArm.ArmStats.ShakeAmount),
                //(float)GD.RandRange(-shakeAmount, shakeAmount)
                0.0f
            );

            Transform3D new_transform = initial_transform;
            new_transform.Origin += offset;
            camera3D.Transform = new_transform;

            elapsed_time += (float)GetProcessDeltaTime();

            await ToSignal(GetTree(), "process_frame");
        }

        camera3D.Transform = initial_transform;
    }


    void Reload()
    {
        for (int i = 0; i <= CurrentArm.ArmStats.TotalClipSize; i++)
        {
            if (PlayerStats.Instance.GetAmmo(CurrentArm.Name) > 0 
                && CurrentArm.ArmStats.ClipSize != CurrentArm.ArmStats.TotalClipSize
                )
            {
                CurrentArm.ArmStats.ClipSize += 1;
                PlayerStats.Instance.RemoveAmmo(CurrentArm.Name);
            }
        }
        CSignals.EmitSignal(nameof(CSignals.UpdateAmmoLabel));
    }

    void Repair()
    {
        //CameraShakeAsync();


        if (rayCast.IsColliding())
        {
            if (rayCast.GetCollider() is Building
                && PlayerStats.Instance.Gold >= PlayerStats.Instance.RepairCost
                )
            {
                var building = (Building)rayCast.GetCollider();

                if (building.Health == 100)
                    return;

                building.ReceiveRepair(10);
                PlayerStats.Instance.RemoveGears(PlayerStats.Instance.RepairCost);
                PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
            }
        }
    }

    void AddBulletDecal()
    {
        var decal = BulletDecal.Instantiate() as Decal;
        var collisionPoint = rayCast.GetCollisionPoint();
        decal.Position = collisionPoint;
        GetTree().CreateTimer(3).Timeout += decal.QueueFree;
        GetNode("/root/World").AddChild(decal);
    }

    public void UnlockWeapon(int index)
    {
        ArmArray[index].IsOwned = true;
    }

    public void UpgradeWeapon(int index, ArmamentStats armamentStats)
    {
        ArmArray[index].ArmStats = armamentStats;
    }
}
