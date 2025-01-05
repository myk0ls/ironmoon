using Godot;
using System;

public partial class TowerModel : Node3D
{
    ShaderMaterial Green;
    ShaderMaterial Red;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //Green = GetNode<ShaderMaterial>("res://Shaders/TowerBuild/Green.tres");
        Green = ResourceLoader.Load<ShaderMaterial>("res://Shaders/TowerBuild/GreenShaderMaterial.tres");
        Red = ResourceLoader.Load<ShaderMaterial>("res://Shaders/TowerBuild/RedShaderMaterial.tres");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ChangeShader(string name)
	{
        ShaderMaterial material;
        switch(name)
        {
            case "Green":
                material = Green;
                break;
            case "Red":
                material = Red; 
                break;
            default:
                material = null;
                break;
        }


        foreach (Node node in GetChildren())
        {
            if (node is MeshInstance3D)
            {
                var mesh = (MeshInstance3D)node;

                for(int i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); i++)
                {
                    mesh.SetSurfaceOverrideMaterial(i, material);
                }

                if (node.GetChildren().Count > 0)
                {
                    ChangeShader(name);
                }
            }
        }
    }

    public void ChangeShader(Node currentNode, ShaderMaterial material)
    {
        // Check if the current node is a MeshInstance3D
        if (currentNode is MeshInstance3D mesh)
        {
            for (int i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); i++)
            {
                mesh.SetSurfaceOverrideMaterial(i, material);
            }
        }

        // Recursively process all child nodes
        foreach (Node child in currentNode.GetChildren())
        {
            ChangeShader(child, material);
        }
    }

    public void UpdateShader(string name)
    {
        ShaderMaterial material;
        switch (name)
        {
            case "Green":
                material = Green;
                break;
            case "Red":
                material = Red;
                break;
            default:
                material = null;
                break;
        }

        ChangeShader(this, material);
    }


}
