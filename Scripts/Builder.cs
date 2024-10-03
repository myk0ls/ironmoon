using Godot;
using System;

public partial class Builder : Node3D
{
    Player PlayerNode;
    PackedScene TowerScene;
    CustomSignals CSignals;
    Tower TargetTower;
    Timer SellTimer;

    bool isInSelection = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PlayerNode = GetParent<Player>();
        CSignals = GetNode<CustomSignals>("/root/CustomSignals");
        TowerScene = ResourceLoader.Load<PackedScene>("res://Scenes/tower.tscn");
        SellTimer = GetParent().GetNode<Timer>("SellTimer");

        SellTimer.Timeout += FinishSellBuilding;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void _ProcessBuild(double delta)
    {
        // build a towewr
        if (Input.IsActionJustPressed("attack"))
        {
            if (GetChildCount() != 0 && PlayerStats.Instance.Gold >= 100)
            {
                Tower placeTower = (Tower)GetChild(0);
                
                if (!CheckAreaIfAbleToBuild(placeTower))
                    return;
                if (!CheckGroundIfAbleToBuild(placeTower))
                    return;


                placeTower.GlobalTransform = GlobalTransform;

                isInSelection = false;

                placeTower.CanAttack = true;
                placeTower.Reparent(GetParent().GetParent());
                placeTower.CollisionLayer = 1;

                foreach (Node child in GetChildren())
                {
                    child.QueueFree();
                }

                PlayerStats.Instance.Gold -= 100;
                PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
                GD.Print("GOLD:" + PlayerStats.Instance.Gold);
            }
        }

        //sell a tower by starting timer
        if (Input.IsActionJustPressed("attack2"))
        {
            if (PlayerNode.RayCast.IsColliding())
            {
                if (PlayerNode.RayCast.GetCollider() is Tower)
                {
                    TargetTower = (Tower)PlayerNode.RayCast.GetCollider();

                    if (TargetTower.CanAttack == true)
                    {
                        SellTimer.Start();
                        GD.Print("PRADEJO SUDA");
                    }
                }
            }
        }

        // cancel sell
        if (Input.IsActionJustReleased("attack2") && !SellTimer.IsStopped() && (PlayerNode.RayCast.GetCollider() is not Tower))
        {
            SellTimer.Stop();
        }

        // if you look away mid sell
        if (Input.IsActionPressed("attack2"))
        {
            if (PlayerNode.RayCast.GetCollider() is not Tower)
                SellTimer.Stop();

        }

        //pick first tower
        if (Input.IsActionJustPressed("one"))
        {
            if (!isInSelection)
            {
                TestTower();
                isInSelection = true;
            }
            else if (isInSelection)
            {
                foreach (Node child in GetChildren())
                {
                    child.QueueFree();
                }
                isInSelection = false;
            }
        }

    }

    void TestTower()
    {
        Tower newTower = (Tower)TowerScene.Instantiate();
        newTower.AxisLockAngularZ = true;
        newTower.AxisLockLinearZ = true;
        newTower.CollisionLayer = 2;
        AddChild(newTower);
    }



    void StartSellBuilding()
    {
    }

    void FinishSellBuilding()
    {
        if (PlayerNode.RayCast.IsColliding() && Input.IsActionPressed("attack2"))
        {
            if (PlayerNode.RayCast.GetCollider() == TargetTower)
            {
                Tower collider = (Tower)PlayerNode.RayCast.GetCollider().Call("Remove");

                PlayerStats.Instance.Gold += 100;
                PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
                GD.Print("GOLD:" + PlayerStats.Instance.Gold);
                GD.Print("BAIGE SUDA");
            }
        }
    }

    bool CheckAreaIfAbleToBuild(Tower placeTower)
    {
        foreach(Node node in placeTower.DetectionArea.GetOverlappingAreas())
        {
            GD.Print(node.Name.ToString());
            if (node.GetParent() is Tower)
            {
                return false;
            }    
        }
        return true;
    }

    bool CheckGroundIfAbleToBuild(Tower placeTower)
    {
        if (placeTower.RayCast.IsColliding())
        {
            // if (placeTower.RayCast.GetCollider())
            return true;
        }
        return false;
    }
}
