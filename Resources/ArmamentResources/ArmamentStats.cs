using Godot;
using System;

[GlobalClass]
public partial class ArmamentStats : Resource
{
    [Export] 
    public float BaseDamage { get; set; }

    [Export]
    public float Range { get; set; }

    [Export]
    public float ClipSize { get; set; }

    [Export]
    public float TotalClipSize { get; set; }

    [Export]
    public float KnockBack { get; set; }

    [Export]
    public float RecoilAmount { get; set; }

    [Export]
    public float RecoilSpeed { get; set; }

    [Export]
    public float MaxRecoil { get; set; }

    public ArmamentStats() : this(0, 0, 0, 0, 0) { }

    public ArmamentStats(float BaseDamage, float Range, float ClipSize, float TotalClipSize,
        float KnockBack)
    {
        this.BaseDamage = BaseDamage;
        this.Range = Range;
        this.ClipSize = ClipSize;
        this.TotalClipSize = TotalClipSize;
        this.KnockBack = KnockBack;
    }
}
