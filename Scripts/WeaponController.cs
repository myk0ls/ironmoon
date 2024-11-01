using Godot;
using System;

public partial class WeaponController : Node3D
{
    const float ADSLerp = 20;

    /// <summary>
    /// weapon bob and sway parameters
    /// </summary>
	Vector3 MouseMov;
    float SwayThreshold = 20;
    float SwaySens = 2f;
    float SwayAmount = 0.15f;
    float SwaySpeed = 5;

    float StartY;
    float BobSpeed = 0.002f;
    float BobAmount = 0.01f;
    Vector3 DefaultRotation;
    //

    private float shakeAmount = 0.02f; // Amount of shake
    private float shakeDuration = 0.05f; // Duration of shake in seconds
    Vector3 _originalPosition;

    public float CurrentRecoil = 0f;
    public float _shakeTimer = 0f;


    //arm array
    Godot.Collections.Array<Armament> ArmArray = new Godot.Collections.Array<Armament>();
    int CurrentWeaponIndex { get; set; } = -1;
    public Armament CurrentArm = null;

    Player player;
    RayCast3D rayCast;
    Camera3D camera3D;



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

        //CurrentArm = ArmArray[CurrentWeaponIndex];
        SwitchWeapon(0);
        


        _originalPosition = Position;
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

        

        if (Input.IsActionJustPressed("one"))
        {
            SwitchWeapon(0);
        }

        if (Input.IsActionJustPressed("two"))
        {
            SwitchWeapon(1);
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
        if (rayCast.IsColliding() && rayCast != null 
            && animName == "shoot" 
            && rayCast.GetCollider() != null
            && CurrentArm.ArmStats.ClipSize > 0
            )
        {
            CurrentArm.ArmStats.ClipSize -= 1;
            GD.Print(CurrentArm.ArmStats.ClipSize);

            SfxManager.Instance.Play(CurrentArm.Name + "Shot",this);

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
            Rotation = Rotation.Lerp(DefaultRotation, SwaySpeed * (float)delta);

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
            Position = Position.Lerp(new Vector3(Position.X, (float)StartY, Position.Z), BobSpeed);
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
            Vector3 shakeOffset = new Vector3(
                (float)GD.RandRange(-shakeAmount, shakeAmount),
                (float)GD.RandRange(-shakeAmount, shakeAmount),
                (float)GD.RandRange(-shakeAmount, shakeAmount)
            );

            camera3D.Position = _originalPosition + shakeOffset;
        } 
        else
        {
            // Reset position when shake ends
            camera3D.Position =  Position.Lerp(_originalPosition, shakeAmount);
        }
    }

    void TriggerShake()
    {
        _shakeTimer = shakeDuration;
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
    }
}
