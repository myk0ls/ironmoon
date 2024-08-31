using Godot;
using System;

public partial class InteractionArea : Area3D
{
	Node interactObject;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
}
