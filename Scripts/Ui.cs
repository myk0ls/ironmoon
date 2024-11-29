using Godot;
using System;

public partial class Ui : Control
{
	Player PlayerNode;
	ProgressBar SellBar;
	Label GoldLabel;
	Label FPSLabel;
	Label AmmoLabel;
    Panel ShopPanel;
    ItemList TowerList;
	CustomSignals CSignals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CSignals = GetNode<CustomSignals>("/root/CustomSignals");
		PlayerNode = GetNode<Player>("/root/World/Player");
		SellBar = GetNode<ProgressBar>("SellBar");
        GoldLabel = GetNode<Label>("GoldLabel");
		FPSLabel = GetNode<Label>("FPSLabel");
		AmmoLabel = GetNode<Label>("AmmoLabel");
		ShopPanel = GetNode<Panel>("ShopPanel");

        SellBar.Value = PlayerNode.SellTimer.WaitTime - PlayerNode.SellTimer.TimeLeft;

		PlayerStats.Instance.UpdateGoldLabel += UpdateGold;
		CSignals.UpdateAmmoLabel += UpdateAmmoLabel;
		CSignals.ShopView += ShowShop;

        GoldLabel.Text = String.Format("GEARS: {0}", PlayerStats.Instance.Gold);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!PlayerNode.SellTimer.IsStopped())
		{
			SellBar.Visible = true;
			SellBar.Value = PlayerNode.SellTimer.WaitTime - PlayerNode.SellTimer.TimeLeft;
		}
		else
			SellBar.Visible = false;

		if (FPSLabel.Visible)
			FPSLabel.Text = String.Format("FPS:{0}", Engine.GetFramesPerSecond());
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("fpsdebug"))
		{
			if (!FPSLabel.Visible)
				FPSLabel.Visible = true;
			else
				FPSLabel.Visible = false;
		}
		/*
		if (Input.IsActionJustPressed("one") && PlayerNode.currentMode == PlayerMode.Build)
		{
			if (!TowerList.IsAnythingSelected())
			{
				TowerList.Select(0);
			}
			else
				TowerList.DeselectAll();
		}

        if (Input.IsActionJustPressed("attack") && PlayerNode.currentMode == PlayerMode.Build)
        {
			if (TowerList.IsAnythingSelected() == true)
			{
				TowerList.DeselectAll();
			}
        }
		*/
    }

    void UpdateGold()
	{
		GoldLabel.Text = String.Format("GEARS: {0}", PlayerStats.Instance.Gold);
	}

	void UpdateAmmoLabel()
	{
		var weapon = PlayerNode.WeaponController as WeaponController;
		AmmoLabel.Text = weapon.CurrentArm.ArmStats.ClipSize.ToString() + "/" +
			PlayerStats.Instance.GetAmmo(weapon.CurrentArm.Name);
	}

	void ShowShop()
	{
		if (ShopPanel.Visible == false)
		{
			ShopPanel.Visible = true;
		}
		else
		{
			ShopPanel.Visible = false;
		}
	}
}
