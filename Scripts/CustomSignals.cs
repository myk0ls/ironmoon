using Godot;
using System;

public partial class CustomSignals : Node
{
	[Signal] public delegate void GameModeChangedEventHandler();

    [Signal] public delegate void RepairModeEventHandler();

    [Signal] public delegate void UpdateAmmoLabelEventHandler();

    [Signal] public delegate void InteractNodeUpdateEventHandler();
    
    [Signal] public delegate void ShopViewEventHandler();

    [Signal] public delegate void ShopViewCloseEventHandler();

    [Signal] public delegate void UpdateCurrentWaveEventHandler(int wave);

    [Signal] public delegate void WaveEndedEventHandler();

    [Signal] public delegate void ActiveEnemyKilledEventHandler();

    [Signal] public delegate void IntermissionLabelEventHandler();

    [Signal] public delegate void PurchaseEventHandler(string item);

    [Signal] public delegate void UpgradeItemEventHandler(string item, int level);

    [Signal] public delegate void BaseHealthUpdateEventHandler(int health);
}
