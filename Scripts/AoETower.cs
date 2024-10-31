using Godot;
using System;

public partial class AoETower : Building
{
    AnimationPlayer TowerAnim;
    public override void _Ready()
    {
        base._Ready();
        TowerAnim = GetNode<AnimationPlayer>("ElectricTower/AnimationPlayer");

        AttackTimer.Start();
    }

    public override void Fire()
    {
        if (CanAttack == true)
        {
            Godot.Collections.Array<Node3D> bodies = DetectionArea.GetOverlappingBodies();

            if (bodies != null)
            {
                TowerAnim.Play("Shoot");
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
            node.Call("ReceiveDamage", 24); 
            Enemy enem = node as Enemy;
            GD.Print("AFTER AOE DAMAGE: " + enem.EnemyStats.Health);
        }
    }
}
