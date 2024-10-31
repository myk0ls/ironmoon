using Godot;
using System;

[GlobalClass]
public partial class EnemyStats : Resource
{
    [Export]
    public float Health { get; set; }

    [Export]
    public float WalkSpeed { get; set; }

    [Export]
    public float CriticalHitChance { get; set; }

    [Export]
    public float DropChance { get; set; }

    [Export]
    public float AttackDamage { get; set; }

    [Export]
    public float AttackSpeed { get; set; }

    public EnemyStats(): this(0, 0, 0, 0, 0, 0) { }

    public EnemyStats(float Health, float WalkSpeed, float CriticalHitChance, 
        float DropChance, float AttackDamage, float AttackSpeed)
    {
        this.Health = Health;
        this.WalkSpeed = WalkSpeed;
        this.CriticalHitChance = CriticalHitChance;
        this.DropChance = DropChance;
        this.AttackDamage = AttackDamage;
        this.AttackSpeed = AttackSpeed;
    }
}
