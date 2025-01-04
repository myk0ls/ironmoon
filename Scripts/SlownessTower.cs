using Godot;
using System;

public partial class SlownessTower : Building
{
    AnimationPlayer TowerAnim;
    public override void _Ready()
    {
        base._Ready();
        //TowerAnim = GetNode<AnimationPlayer>("ElectricTower/AnimationPlayer");

        //AttackTimer.Start();
    }

    public override void Fire()
    {
        if (CanAttack == true)
        {
            Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

            if (bodies != null)
            {
                //TowerAnim.Play("Shoot");
                for (int i = 0; i < bodies.Count; i++)
                {
                    ApplyEffect(bodies[i]);
                }
            }
        }
    }

    void ApplyEffect(Node node)
    {
        if (node.IsInGroup("enemy"))
        {
           // node.Call("ReceiveDamage", 24);
            Enemy enem = node as Enemy;
            //GD.Print("AFTER AOE DAMAGE: " + enem.EnemyStats.Health);
            enem.EnemyStats.WalkSpeed = enem.EnemyStats.WalkSpeed / 2;
        }
    }


    void OnEntered(Node node)
    {
        if (node.IsInGroup("enemy"))
        {
            // node.Call("ReceiveDamage", 24);
            Enemy enem = node as Enemy;
            //GD.Print("AFTER AOE DAMAGE: " + enem.EnemyStats.Health);
            enem.EnemyStats.WalkSpeed = enem.EnemyStats.WalkSpeed / 2;
        }
    }

    void OnExited(Node node)
    {
        if (node.IsInGroup("enemy"))
        {
            Enemy enem = node as Enemy;
            enem.EnemyStats.WalkSpeed = enem.EnemyStats.WalkSpeed * 2;
        }
    }

    void OnDeath()
    {
        Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

        if (bodies != null)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].IsInGroup("enemy"))
                {
                    Enemy enem = bodies[i] as Enemy;
                    enem.EnemyStats.WalkSpeed = enem.EnemyStats.WalkSpeed * 2;
                }
            }
        }
    }
}
