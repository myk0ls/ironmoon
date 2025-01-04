using Godot;
using System;

[GlobalClass]
public partial class WaveStats : Resource
{
    [Export]
    public int SpiderSmall;

    [Export]
    public int SpiderMedium;

    [Export]
    public int SpiderBig;

    [Export]
    public int FlyingBot;

    [Export]
    public int Warden;

    public WaveStats() : this(0, 0, 0, 0, 0) { }

    public WaveStats(int spiderSmall, int spiderMedium, int spiderBig, int flyingBot, int warden)
    {
        SpiderSmall = spiderSmall;
        SpiderMedium = spiderMedium;
        SpiderBig = spiderBig;
        FlyingBot = flyingBot;
        Warden = warden;
    }

}
