using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export]
	public EnemyStats EnemyStats { get; set; }

    [Export]
    public Node3D Model { get; set; }

    public CollisionShape3D HitBox { get; set; }

	public AnimationPlayer Animations { get; set; }

	public AnimationTree AnimationsTree { get; set; }

    Timer RemoveTimer { get; set; }

    public AnimationNodeStateMachinePlayback animationStateMachine { get; set; }

    PhysicalBoneSimulator3D simulator { get; set; }

    Skeleton3D skeleton {  get; set; }

    public PathFollow3D PathToFollow;

    bool IsAlive = true;

    PackedScene GearScene;

    public Node3D TargetNode { get; set; } = null;

    Node3D World;
	
	[Signal]
	public delegate void DeathEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PathToFollow = GetParent<PathFollow3D>();
        RemoveTimer = GetNode<Timer>("RemoveTimer");
        Animations = Model.GetNode<AnimationPlayer>("AnimationPlayer");
        AnimationsTree = Model.GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = (AnimationNodeStateMachinePlayback)Model.GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");
        skeleton = Model.GetNode<Skeleton3D>("Armature/Skeleton3D");
        HitBox = GetNode<CollisionShape3D>("CollisionShape3D");
        simulator = skeleton.GetNode<PhysicalBoneSimulator3D>("PhysicalBoneSimulator3D");

        World = GetNode<Node3D>("/root/World");
        GearScene = ResourceLoader.Load<PackedScene>("res://Scenes/gear.tscn");

        //skeleton.PhysicalBonesStopSimulation();

        EnemyStats duplicate = (EnemyStats)EnemyStats.Duplicate();
        EnemyStats = duplicate;



        Death += Die;
        RemoveTimer.Timeout += QueueFree;

        //Animations.AnimationFinished += (animation) => Die(animation);

        animationStateMachine.Travel("Walk");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        
        if (!IsAlive)
        {
            return;
        }
        


        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }
        /*
        var pathProgress = PathToFollow.Progress + EnemyStats.WalkSpeed * (float)delta;

        if (PathToFollow.ProgressRatio == 1)
        {
            QueueFree();
        }

        PathToFollow.Progress = pathProgress;
        */
        Velocity = velocity;
        MoveAndSlide();
    }

    public void ReceiveDamage(float Damage)
    {
        EnemyStats.Health -= Damage;
        SfxManager.Instance.Play("RobotHit", this);

        if (EnemyStats.Health <= 0)
        {
            //Die();
            //animationStateMachine.Travel("Die");
            IsAlive = false;
            //SetCollisionLayerValue(1, false);
            //SetCollisionLayerValue(2, true);
            SetCollisionLayerValue(2, false);
            SetCollisionLayerValue(3, true);
            HitBox.Disabled = true;
            //skeleton.PhysicalBonesStartSimulation();

            /*
            HitBox.Disabled = true;
            AnimationsTree.Active = false;

            simulator.Active = true;
            //skeleton.PhysicalBonesStartSimulation();
            */
            animationStateMachine.Travel("Die");

            EmitSignal(nameof(Death));
        }
    }

    public void Die()
    {
        
            //QueueFree();
            //EmitSignal(nameof(Death));
            PlayerStats.Instance.GainGold(3);
            PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
        RemoveTimer.Start();
        SfxManager.Instance.Play("RobotDeath", this);

        Gear gear = (Gear)GearScene.Instantiate();
        //gear.GlobalPosition = this.GlobalPosition;
        //GetParent().GetParent().GetParent().AddChild(gear);
        gear.Position = GlobalPosition;
        World.AddChild(gear);

    }

    void Dying()
    {
        //animationStateMachine.Travel("Die");

    }
}
