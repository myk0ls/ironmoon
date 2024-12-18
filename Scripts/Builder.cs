using Godot;
using System;

public partial class Builder : Node3D
{
    Player PlayerNode;
    PackedScene TowerScene;
    PackedScene SingleTargetTowerScene;
    PackedScene AoeTowerScene;
    CustomSignals CSignals;
    Building TargetTower;
    Timer SellTimer;

    bool isInSelection = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PlayerNode = GetParent<Player>();
        CSignals = GetNode<CustomSignals>("/root/CustomSignals");
        TowerScene = ResourceLoader.Load<PackedScene>("res://Scenes/tower.tscn");
        SingleTargetTowerScene = ResourceLoader.Load<PackedScene>("res://Scenes/single_target_tower.tscn");
        AoeTowerScene = ResourceLoader.Load<PackedScene>("res://Scenes/aoe_tower.tscn");
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
                Building placeTower = (Building)GetChild(0);
                
                if (!CheckAreaIfAbleToBuild(placeTower))
                    return;
                if (!CheckGroundIfAbleToBuild(placeTower))
                    return;


                placeTower.GlobalTransform = GlobalTransform;

                isInSelection = false;

                placeTower.CanAttack = true;
                placeTower.Reparent(GetParent().GetParent());
                //placeTower.CollisionLayer = 1;


                foreach (Node child in GetChildren())
                {
                    child.QueueFree();
                }

                VfxManager.Instance.Play("Dust", placeTower, placeTower.TowerShape.Position);

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
                if (PlayerNode.RayCast.GetCollider() is Building)
                {
                    TargetTower = (Building)PlayerNode.RayCast.GetCollider();

                    if (TargetTower.CanAttack == true)
                    {
                        SellTimer.Start();
                        GD.Print("PRADEJO SUDA");
                    }
                }
            }
        }

        // cancel sell
        if (Input.IsActionJustReleased("attack2") && !SellTimer.IsStopped() && (PlayerNode.RayCast.GetCollider() is not Building))
        {
            SellTimer.Stop();
        }

        // if you look away mid sell
        if (Input.IsActionPressed("attack2"))
        {
            if (PlayerNode.RayCast.GetCollider() is not Building)
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

        if (Input.IsActionJustPressed("two"))
        {
            if (!isInSelection)
            {
                TestAOETower();
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
        SingleTargetTower newTower = (SingleTargetTower)SingleTargetTowerScene.Instantiate();
        //newTower.AxisLockAngularZ = true;
        //newTower.AxisLockLinearZ = true;
        //newTower.CollisionLayer = 2;
        AddChild(newTower);
    }

    void TestAOETower()
    {
        AoETower newTower = (AoETower)AoeTowerScene.Instantiate();
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
                Building collider = (Building)PlayerNode.RayCast.GetCollider().Call("Remove");

                PlayerStats.Instance.Gold += 100;
                PlayerStats.Instance.EmitSignal(nameof(PlayerStats.Instance.UpdateGoldLabel));
                GD.Print("GOLD:" + PlayerStats.Instance.Gold);
                GD.Print("BAIGE SUDA");
            }
        }
    }

    bool CheckAreaIfAbleToBuild(Building placeTower)
    {
        foreach(Node node in placeTower.DetectionArea.GetOverlappingAreas())
        {
            if (node.GetParent() is Building)
            {
                return false;
            }    
        }
        return true;
        
    }

    bool CheckGroundIfAbleToBuild(Building placeTower)
    {
        if (placeTower.BuildRayCast.IsColliding())
        {
            //if (placeTower.BuildRayCast.GetCollider().IsInGroup)
            return true;
        }
        return false;
    }
}
