using Godot;
using System;

public partial class Ui : Control
{
	Player PlayerNode;
	ProgressBar SellBar;
	Label GoldLabel;
	Label FPSLabel;
	Label AmmoLabel;
	Label HealthLabel;
	Panel ShopPanel;
    ItemList TowerList;
	CustomSignals CSignals;
	Label WaveLabel;
    Label IntermissionLabel;
	Timer IntermissionTimer;
	AnimationPlayer animationPlayer;
	ProgressBar BaseHealthBar;

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
        WaveLabel = GetNode<Label>("WaveLabel");
		HealthLabel = GetNode<Label>("HealthBar/HealthLabel");
		IntermissionLabel = GetNode<Label>("IntermissionLabel");
		IntermissionTimer = GetNode<Timer>("/root/World/WaveManager/IntermissionTimer");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		BaseHealthBar = GetNode<ProgressBar>("BaseHealthBar");

        SellBar.Value = PlayerNode.SellTimer.WaitTime - PlayerNode.SellTimer.TimeLeft;

		PlayerStats.Instance.UpdateGoldLabel += UpdateGold;
		CSignals.UpdateAmmoLabel += UpdateAmmoLabel;
		PlayerStats.Instance.UpdateHPLabel += UpdateHp;
		CSignals.ShopView += ShowShop;
		CSignals.ShopViewClose += CloseShop;
		CSignals.IntermissionLabel += ToggleIntermissionLabel;
		CSignals.UpdateCurrentWave += (currentWave) => ShowWaveLabel(currentWave);
		CSignals.WaveEnded += ShowWaveEndedLabel;
		CSignals.BaseHealthUpdate += (health) => UpdateBaseHealthBar(health);

        GoldLabel.Text = String.Format("GEARS: {0}", PlayerStats.Instance.Gold);
		UpdateHp();
		UpdateBaseHealthBar(1000);
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

		if (!IntermissionTimer.IsStopped())
		{
            //IntermissionLabel.Text = String.Format("{0:0.#}",IntermissionTimer.TimeLeft.ToString());
            IntermissionLabel.Text = IntermissionTimer.TimeLeft.ToString("0");
           /*
			if (IntermissionLabel.Text == "5" || IntermissionLabel.Text == "4" || IntermissionLabel.Text == "3" ||
				IntermissionLabel.Text == "2" || IntermissionLabel.Text == "1" || IntermissionLabel.Text == "0")
            {
                SfxManager.Instance.Play("ClockTick", PlayerNode);
            }
		   */
        }
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

	void UpdateHp()
	{
		HealthLabel.Text = String.Format("HP: {0}", PlayerStats.Instance.GetHealth());
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
			GD.Print("SHOPAS");
		}
		else
			ShopPanel.Visible = false;
		
	}

	void CloseShop()
	{
		if (ShopPanel.Visible == true)
		{
			ShopPanel.Visible = false;
		}
	}

	void ToggleIntermissionLabel()
	{
		if (!IntermissionLabel.Visible)
		{
			IntermissionLabel.Visible = true;
		}
		else
			IntermissionLabel.Visible = false;

    }

	void ShowWaveLabel(int currentWave)
	{
		if (WaveLabel.Visible == false)
		{
			WaveLabel.Visible = true;
		}
		WaveLabel.Text = "Wave " + (currentWave + 1);
		animationPlayer.Play("ShowWave");

		//WaveLabel.Visible = false;
	}

	void ShowWaveEndedLabel()
	{
        if (WaveLabel.Visible == false)
        {
            WaveLabel.Visible = true;
        }
        WaveLabel.Text = "Wave Completed";
        animationPlayer.Play("ShowWave");
    }

	void UpdateBaseHealthBar(int health)
	{
		BaseHealthBar.Value = health;
	}
}

