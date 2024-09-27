using Godot;
using System;

public partial class DamageNumbers : Node
{
	public static DamageNumbers Instance;
	Node3D World;
	Node3D Number;
	PackedScene NumberScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		World = GetNode<Node3D>("/root/World");
		NumberScene = ResourceLoader.Load<PackedScene>("res://Scenes/damage_number.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	/*
	public void DisplayNumber(int Value, Vector3 Position, bool IsCritical = false)
	{
		Label3D Number = new Label3D();
		Number.GlobalPosition = Position;
		Number.Text = String.Format("{0}", Value);


		Color color = new Color(1, (float)0.98, (float)0.94, 1); // whitey
		if (IsCritical)
		{
			color = new Color(1, 0, 0, 1); // red
		}
		
		if (Value == 0)
		{
			color = new Color(1, 1, 1, 0); // transparent
		}

		Number.Modulate = color;
		Number.FontSize = 100;
		Number.OutlineModulate = new Color(0, 0, 0, 1); // black
		Number.OutlineSize = 1;
		Number.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;

		//GetTree().CurrentScene.Call("AddChild", Number);
		World.AddChild(Number);
	}
	*/
	public void DisplayNumber(int Value, Vector3 Position, bool IsCritical = false)
	{
		DamageNumber Number = NumberScene.Instantiate() as DamageNumber;
		World.AddChild(Number);

		Number.GlobalPosition = Position;
		Number.DamageLabel.Text = Value.ToString();

        Color color = new Color(1, (float)0.98, (float)0.94, 1); // whitey
        if (IsCritical)
        {
            color = new Color(1, 0, 0, 1); // red
        }

        if (Value == 0)
        {
            color = new Color(1, 1, 1, 0); // transparent
        }

        Number.DamageLabel.Modulate = color;

    }
}