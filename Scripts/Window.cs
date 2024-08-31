using Godot;
using System;
using System.Collections.Generic;

public partial class Window : StaticBody3D
{
	List<Node3D> Planks = new List<Node3D>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node3D PlankArray = GetNode<Node3D>("Planks");

		for (int i = 0; i < PlankArray.GetChildCount(); i++)
		{
			Planks.Add(PlankArray.GetChild<Node3D>(i));
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Interact()
	{
		GD.Print("veikiaaa");

		for (int i = 0; i < Planks.Count; i++)
		{
			if (Planks[i].Visible == false)
			{
				Planks[i].Visible = true;
				break;
			}

        }
	}
}
