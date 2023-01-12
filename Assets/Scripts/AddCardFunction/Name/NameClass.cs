using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameClass
{
    private string name;
    public string Name { get {return name;}}
    private IAdjective[] specificities;
    public IAdjective[] Specificities { get {return specificities;} }

    public NameClass(string name, IAdjective[] specificities)
    {
        this.name = name;
        this.specificities = specificities;
    }
}

public class Rock : NameClass
{
    public Rock() : base("Rock", new IAdjective[] { new HeavyAdj() }) { }
}

public class Ball : NameClass
{
    public Ball() : base("Ball", new IAdjective[]{new PushAdj(), new BurstAdj()}){}
}

public class Cactus : NameClass
{
    public Cactus() : base("Cactus", new IAdjective[]{}){}
}

public class Balloon : NameClass
{
    public Balloon() : base("Balloon", new IAdjective[]{new LightAdj(), new FlyAdj()}){}
}