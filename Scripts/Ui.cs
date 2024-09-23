using Godot;
using System;

public partial class Ui : Control
{
	Player PlayerNode;
	ProgressBar SellBar;
	Label GoldLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerNode = GetNode<Player>("/root/World/Player");
		SellBar = GetNode<ProgressBar>("SellBar");
        GoldLabel = GetNode<Label>("GoldLabel");

        SellBar.Value = PlayerNode.SellTimer.WaitTime - PlayerNode.SellTimer.TimeLeft;

		PlayerStats.Instance.UpdateGoldLabel += UpdateGold;
        GoldLabel.Text = String.Format("GOLD: {0}", PlayerStats.Instance.Gold);
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
	}

	void UpdateGold()
	{
		GoldLabel.Text = String.Format("GOLD: {0}", PlayerStats.Instance.Gold);
	}
}
