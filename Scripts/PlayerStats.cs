using Godot;
using System;

public partial class PlayerStats : Node
{
	public int Health = 100;
	public int Mana = 100;
	public int Gold = 350;
	public static PlayerStats Instance { get; private set; }


    [Signal] public delegate void UpdateGoldLabelEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GainGold(int Amount)
	{
		Gold += Amount;
	}
}
