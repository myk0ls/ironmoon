using Godot;
using System;

public partial class Vfx : Node3D
{
	Godot.Collections.Array<GpuParticles3D> Particles3Ds = new Godot.Collections.Array<GpuParticles3D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node node in GetChildren())
		{
			var particle = node as GpuParticles3D;
			Particles3Ds.Add(particle);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public async void Activate()
	{
		foreach (GpuParticles3D particle in Particles3Ds)
		{
			particle.Emitting = true;
		}
		await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
		QueueFree();
		
    }

	public void ActivateContinous()
	{
        foreach (GpuParticles3D particle in Particles3Ds)
        {
            particle.Emitting = true;
        }
    }

	public void Deactivate()
	{
        foreach (GpuParticles3D particle in Particles3Ds)
        {
            particle.Emitting = false;
        }
    }
}