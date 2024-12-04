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

}
