using Godot;
using System;
using System.Net.WebSockets;

public partial class WaveManager : Node
{
	[Export]
	Godot.Collections.Array<WaveStats> waves;

	Godot.Collections.Array<PathSpawner> lanes = new Godot.Collections.Array<PathSpawner>();

	CustomSignals CSignals;


		//= new Godot.Collections.Array<Wave>();

	public Timer WaveTimer;
	public Timer IntermissionTimer;

	int currentWave = 0;
	int enemiesLeftToSpawn = 0;
	int activeEnemies = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		WaveTimer = GetNode<Timer>("WaveTimer");
        IntermissionTimer = GetNode<Timer>("IntermissionTimer");
        CSignals = GetNode<CustomSignals>("/root/CustomSignals");

        foreach (PathSpawner lane in GetNode<Node3D>("/root/World/Lanes/").GetChildren())
		{
			lanes.Add(lane);
			//GD.Print(lane.ToString());
		}

		CSignals.ActiveEnemyKilled += OnEnemyDefeated;
		IntermissionTimer.Timeout += StartWave;
		WaveTimer.Timeout += SpawnEnemy;

		IntermissionTimer.Start();
		CSignals.EmitSignal(nameof(CSignals.IntermissionLabel));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void StartWave()
	{
		if (currentWave > waves.Count)
		{
			GD.Print("All waves complete");
			return;
		}

		GD.Print("Pradedamas wave: " + currentWave);

		CSignals.EmitSignal(nameof(CSignals.UpdateCurrentWave), currentWave);

		enemiesLeftToSpawn = CalculateTotalUnits(waves[currentWave]);
		activeEnemies = enemiesLeftToSpawn;

        CSignals.EmitSignal(nameof(CSignals.IntermissionLabel));
        WaveTimer.Start();
	}

	void SpawnEnemy()
	{
		if (enemiesLeftToSpawn <= 0)
		{
			WaveTimer.Stop();
			CheckWaveComplete();
			return;
		}
        var randomLane = lanes[(int)GD.RandRange(0,1)];
		randomLane.AddPathFollow(GetRandomEnemyType());
		enemiesLeftToSpawn--;

		GD.Print($"Enemy spawned! Enemies left to spawn: {enemiesLeftToSpawn}");
	}

	void OnEnemyDefeated()
	{
		activeEnemies--;

        GD.Print($"Enemy defeated! Active enemies: {activeEnemies}");
        CheckWaveComplete();
	}

    private void CheckWaveComplete()
    {
        if (enemiesLeftToSpawn <= 0 && activeEnemies <= 0)
        {
            GD.Print($"Wave {currentWave} complete!");
            currentWave++;

            if (currentWave < waves.Count)
            {
                IntermissionTimer.Start(); // Start the intermission timer for the next wave
                CSignals.EmitSignal(nameof(CSignals.IntermissionLabel));
                GD.Print($"INTERMISISJA");
            }
            else
            {
                GD.Print("All waves finished!");
            }

			CSignals.EmitSignal(nameof(CSignals.WaveEnded));
        }
    }

    public int CalculateTotalUnits(WaveStats wave)
	{
		return wave.SpiderSmall +
			   wave.SpiderMedium +
			   wave.SpiderBig +
			   wave.FlyingBot;
	}

    string GetRandomEnemyType()
    {
        int totalEnemies = waves[currentWave].SpiderSmall +
                           waves[currentWave].SpiderMedium +
                           waves[currentWave].SpiderBig +
                           waves[currentWave].FlyingBot;

        if (totalEnemies <= 0)
            return null; // No enemies left to spawn

        int randomValue = (int)GD.Randi() % totalEnemies;

        if (randomValue < waves[currentWave].SpiderSmall)
        {
            waves[currentWave].SpiderSmall--;
            return "SpiderSmall";
        }
        randomValue -= waves[currentWave].SpiderSmall;

        if (randomValue < waves[currentWave].SpiderMedium)
        {
            waves[currentWave].SpiderMedium--;
            return "SpiderMedium";
        }
        randomValue -= waves[currentWave].SpiderMedium;

        if (randomValue < waves[currentWave].SpiderBig)
        {
            waves[currentWave].SpiderBig--;
            return "SpiderBig";
        }
        randomValue -= waves[currentWave].SpiderBig;

        // Remaining is FlyingBot
        waves[currentWave].FlyingBot--;
        return "FlyingBot";
    }
}
