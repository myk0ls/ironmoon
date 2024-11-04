using Godot;
using System;

public partial class CustomSignals : Node
{
	[Signal] public delegate void GameModeChangedEventHandler();

    [Signal] public delegate void RepairModeEventHandler();

    [Signal] public delegate void UpdateAmmoLabelEventHandler();
}
