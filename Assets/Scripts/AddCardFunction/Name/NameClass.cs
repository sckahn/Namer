using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameClass
{
    private string name;
    public string Name { get { return name; } }
    private IAdjective[] adjectives;
    public IAdjective[] Adjectives { get { return adjectives; } }

    public NameClass(string name, IAdjective[] adjectives)
    {
        this.name = name;
        this.adjectives = adjectives;
    }
}

public class Rock : NameClass
{
    public Rock() : base("Rock", new IAdjective[] { new HeavyAdj()}) { }
}

public class Cactus : NameClass
{
    public Cactus() : base("Cactus", new IAdjective[]{new SpikyAdj()}){}
}

public class Ball : NameClass
{
    public Ball() : base("Ball", new IAdjective[]{new PushAdj()}){}
}

public class GoalPoint : NameClass
{
    public GoalPoint() : base("GoalPoint", new IAdjective[]{}){}
}
