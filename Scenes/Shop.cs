using Godot;
using System;

public partial class Shop : StaticBody3D
{
	CustomSignals CSignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Interact()
	{
		CSignals.EmitSignal(nameof(CSignals.ShopView));
	}

	public void _on_area_3d_body_exited(Node node)
	{
		if (node is Player)
		{
            CSignals.EmitSignal(nameof(CSignals.ShopViewClose));
        }
	}
}
