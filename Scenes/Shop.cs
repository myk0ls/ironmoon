using Godot;
using System;

public partial class Shop : StaticBody3D
{
	CustomSignals CSignals;
	Player PlayerNode;
	WeaponController _weaponController;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
		PlayerNode = GetNode<Player>("/root/World/Player");
		_weaponController = GetNode<WeaponController>("/root/World/Player/Camera3D/WeaponController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Interact()
	{
		if (PlayerNode.currentMode != PlayerMode.Shop)
		{
			CSignals.EmitSignal(nameof(CSignals.ShopView));
		}
		else
		{
            CSignals.EmitSignal(nameof(CSignals.ShopViewClose));
            GD.Print("EXITTTTT");
        }
    }

	public void _on_area_3d_body_exited(Node node)
	{
		if (node is Player)
		{
			var player = node as Player;
			if (player.currentMode == PlayerMode.Shop)
			{
				CSignals.EmitSignal(nameof(CSignals.ShopViewClose));
				GD.Print("EXITTTTT");
			}
        }
	}

	public void Purchase(string name)
	{
		switch(name)
		{
			case "PPSH":
				_weaponController.UnlockWeapon(2);
				break;
            case "Shotgun":
                _weaponController.UnlockWeapon(3);
                break;
        }
	}

	public void Upgrade(string name, int level)
	{
		switch (name)
		{
			case "HandCannon":
				_weaponController.UpgradeWeapon(1, GetNode<ArmamentStats>("res://Resources/ArmamentResources/Upgrades/handcannon" + level + ".tres"));
				break;
            case "PPSH":
                _weaponController.UpgradeWeapon(2, GetNode<ArmamentStats>("res://Resources/ArmamentResources/Upgrades/ppsh" + level + ".tres"));
                break;
            case "Shotgun":
                _weaponController.UpgradeWeapon(3, GetNode<ArmamentStats>("res://Resources/ArmamentResources/Upgrades/shotgun" + level + ".tres"));
                break;
        }
	}
}
