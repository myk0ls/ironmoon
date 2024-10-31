using Godot;
using System;
using static Godot.TextServer;

public partial class SingleTargetTower : Building
{
    Node3D Head;
    PackedScene Bullet;
    MeshInstance3D TowerHead;
    AnimationPlayer TowerAnim;

    Node3D TargetNode;
    Enemy TargetEnemy;

    public override void _Ready()
    {
        base._Ready();
        Head = GetNode<Node3D>("MachineGunTower/Head/BulletSpawn");
        TowerHead = GetNode<MeshInstance3D>("MachineGunTower/Head");
        Bullet = ResourceLoader.Load<PackedScene>("res://Scenes/bullet.tscn");
        TowerAnim = GetNode<AnimationPlayer>("MachineGunTower/AnimationPlayer");

        TargetNode = null;
        TargetEnemy = null;
    }

    void SetTargetNode(Node3D node)
    {
        TargetNode = node;
        TargetEnemy = (Enemy)TargetNode;
        TargetEnemy.Death += RemoveTarget;
        TargetEnemy.Death += ScanForTarget;
    }

    void RemoveTarget()
    {
        if (TargetNode != null)
        {
            TargetNode = null;
            TargetEnemy.Death -= RemoveTarget;
            TargetEnemy.Death -= ScanForTarget;
        }
    }

    public void OnBodyEntered(Node3D node)
    {
        if (TargetNode is null && node.IsInGroup("enemy"))
        {
            SetTargetNode(node);
            Fire();
            AttackTimer.Start();
        }
    }

    public void OnBodyExited(Node3D node)
    {
        if (node == TargetNode)
        {
            RemoveTarget();
            TargetEnemy = null;
        }
    }


    public override void Fire()
    {
        if (TargetNode != null && CanAttack == true)
        {
            Bullet newBullet = (Bullet)Bullet.Instantiate();
            newBullet.SetTarget(TargetNode);
            Head.AddChild(newBullet);

            Vector3 Direction = (TargetNode.GlobalPosition - GlobalPosition).Normalized();
            //TowerHead.LookAt(-TargetNode.GlobalPosition, Vector3.Up);
            //TowerHead.LookAt(TargetNode.Position, Vector3.Up);

            Vector3 direction = (TargetNode.GlobalPosition - TowerHead.GlobalPosition).Normalized();
            TowerHead.LookAt(TargetNode.GlobalPosition, Vector3.Up);
            TowerHead.RotateObjectLocal(Vector3.Up, (float)3.14);


            TowerAnim.Play("Shoot");
        }
    }

    void ScanForTarget()
    {
        Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

        if (bodies != null)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                GD.Print(bodies[i].ToString());
                if (bodies[i] is CharacterBody3D &&
                    bodies[i].IsInGroup("enemy"))
                {
                    SetTargetNode(bodies[i]);
                    break;
                }
            }
        }
        else
            AttackTimer.Stop();
    }
}
