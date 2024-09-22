using Godot;
using System;
using static Godot.WebSocketPeer;

public partial class Weapon : Node3D
{
    [Export] Vector3 DefaultPosition;
    [Export] Vector3 ADSPosition;
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


    Player player;
	RayCast3D rayCast;
    Node3D WeaponNode;
    Camera3D camera3D;
    AnimationPlayer weaponAnim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        camera3D = (Camera3D)GetParent();
        player = GetNode<Player>("/root/World/Player");
		rayCast = GetParent().GetNode<RayCast3D>("RayCast3D");
        WeaponNode = (Node3D)GetChild(0);
        weaponAnim = WeaponNode.GetNode<AnimationPlayer>("AnimationPlayer");

        weaponAnim.AnimationStarted += (shoot) => Attack(shoot);

        DefaultRotation = Rotation;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
    }

    public void _ProcessCombat(double delta)
    {
        if (Input.IsActionJustPressed("sprint") && weaponAnim.IsPlaying() == false && !Input.IsActionPressed("attack2"))
        {
            weaponAnim.Play("sprintStart");
        }

        if (Input.IsActionPressed("sprint") && weaponAnim.CurrentAnimation != "sprintStart" && !Input.IsActionPressed("attack2"))
        {
            weaponAnim.Play("sprint");
        }

        if (Input.IsActionJustReleased("sprint") && !Input.IsActionPressed("attack2"))
        {
            weaponAnim.PlayBackwards("sprintStart");
        }

        //Shoot
		if (Input.IsActionJustPressed("attack"))
		{
			//Attack();
            weaponAnim.Play("shoot");
		}

        //Aim down sights
        if (Input.IsActionPressed("attack2") && !Input.IsActionPressed("sprint") && weaponAnim.CurrentAnimation != "sprintStart")
        {
            Transform3D newTransform = WeaponNode.Transform;
            newTransform.Origin = newTransform.Origin.Lerp(ADSPosition, ADSLerp * (float)delta);
            WeaponNode.Transform = newTransform;
            SwayAmount = 0.05f;
            
            camera3D.Fov = Mathf.Lerp(camera3D.Fov, player.ADSFOV, ADSLerp * (float)delta);
        }
        else
        {
            Transform3D newTransform = WeaponNode.Transform;
            newTransform.Origin = newTransform.Origin.Lerp(DefaultPosition, ADSLerp * (float)delta);
            WeaponNode.Transform = newTransform;

            SwayAmount = 0.15f;

            camera3D.Fov = Mathf.Lerp(camera3D.Fov, player.DefaultFOV, ADSLerp * (float)delta);
        }

        // Sway and bob weapon
        WeaponSway(delta);
        WeaponBob(delta);

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
        if (rayCast.IsColliding() && rayCast != null && animName == "shoot")
        {
            var collider = rayCast.GetCollider() as Node3D;
            if (collider.IsInGroup("enemy"))
            {
                var shapeIndex = rayCast.GetColliderShape();
                GD.Print(shapeIndex);

                switch(shapeIndex)
                {
                    case 0:
                        collider.Call("ReceiveDamage", 50);
                    break;

                    case 1:
                        collider.Call("ReceiveDamage", 100);
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
}
