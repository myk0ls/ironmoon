using Godot;
using System;

public partial class ShopPanel : Panel
{
	Shop shop;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shop = GetNode<Shop>("/root/World/Shop");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_button_pressed_ppsh()
	{
		if (PlayerStats.Instance.Gold >= 100)
		{
			shop.Purchase("PPSH");
			PlayerStats.Instance.Gold -= 100;
			PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
			GetNode<Button>("/root/World/UI/ShopPanel/WeaponPurchase/Button").Disabled = true;

            SfxManager.Instance.Play("BoughtCash", this);
        }
	}

	public void _on_button_2_pressed_shotgun()
	{
        if (PlayerStats.Instance.Gold >= 100)
        {
            shop.Purchase("Shotgun");
            PlayerStats.Instance.Gold -= 100;
            PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
            GetNode<Button>("/root/World/UI/ShopPanel/WeaponPurchase/Button2").Disabled = true;

            SfxManager.Instance.Play("BoughtCash", this);
        }
    }

	public void _on_button_pressed_wrench()
	{
        if (PlayerStats.Instance.Gold >= 50)
        {
			PlayerStats.Instance.RepairCost = 10;
            PlayerStats.Instance.Gold -= 50;
            PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
            GetNode<Button>("/root/World/UI/ShopPanel/Upgrades/Button").Disabled = true;

			SfxManager.Instance.Play("BoughtCash", this);
        }
    }
}
