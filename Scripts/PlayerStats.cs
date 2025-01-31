using Godot;
using System;
using System.Reflection;

public partial class PlayerStats : Node
{
	public int Health = 100;
	public int Mana = 100;
	public int Gold = 350;

	public int ShotgunAmmo = 1000;
    public int HandCannonAmmo = 1000;
	public int PPSHAmmo = 1000;

	CustomSignals CSignals;

	public int RepairCost = 20;

	bool IsAlive = true;

	public static PlayerStats Instance { get; private set; }


    [Signal] public delegate void UpdateGoldLabelEventHandler();
	[Signal] public delegate void UpdateHPLabelEventHandler();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Instance = this;
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GainGold(int Amount)
	{
		Gold += Amount;
	}

	public void RemoveGears(int Amount)
	{
		Gold -= Amount;
	}
	public void GainAmmo(string AmmoName, int amount)
	{
		switch (AmmoName)
		{
			case "Shotgun":
				ShotgunAmmo += amount;
				break;
			case "HandCannon":
				HandCannonAmmo += amount; 
				break;
			case "PPSH":
				PPSHAmmo += amount;
				break;
		}
	}

	public void RemoveAmmo(string GunName)
	{
        switch (GunName)
        {
            case "Shotgun":
                ShotgunAmmo -= 1;
                break;
            case "HandCannon":
                HandCannonAmmo -= 1;
                break;
            case "PPSH":
                PPSHAmmo -= 1;
                break;
			default:
				break;
        }
    }

	public float GetAmmo(string GunName)
	{
        switch (GunName)
        {
            case "Shotgun":
				return ShotgunAmmo;
            case "HandCannon":
				return HandCannonAmmo;
            case "PPSH":
				return PPSHAmmo;
			default:
				return 1000f;

        }
    }

	public int GetHealth()
	{
		return Health;
	}

	public void ReceiveDamage(int damage)
	{
		if (!IsAlive) return;

		Health -= damage;
		EmitSignal(nameof(UpdateHPLabel));

		if (Health <= 0)
		{
			CSignals.EmitSignal(nameof(CSignals.GameOver));
			IsAlive = false;
		}
	}
}
