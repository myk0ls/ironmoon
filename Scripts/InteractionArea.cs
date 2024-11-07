using Godot;
using System;

public partial class InteractionArea : Area3D
{
	Node interactObject;
	CustomSignals CSignals;
    Area3D InteractionArea3D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CSignals = GetNode<CustomSignals>("/root/CustomSignals");

        CSignals.InteractNodeUpdate += ScanForTarget;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("interact") && interactObject != null)
        {
            interactObject.Call("Interact");
        }
    }

	public void OnBodyEntered(Node node)
	{
        GD.Print(node.ToString());
        if (node.HasMethod("Interact"))
		{
			GD.Print(node.ToString());
			interactObject = node;
		}
	}

	public void OnBodyExited(Node node) 
	{
		interactObject = null;
	}

    void ScanForTarget()
    {
        Godot.Collections.Array<Node3D> bodies = this.GetOverlappingBodies();

        if (bodies != null)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                //GD.Print(bodies[i].ToString());
                if (bodies[i].HasMethod("Interact"))
                {
                    interactObject = bodies[i];
                    break;
                }
            }
        }
    }

}
