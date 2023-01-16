using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private Name nameType;
    
    private Card card;
    private void OnEnable()
    {
        CardFactory cardFactory = null;
        switch (nameType)
        {
            case Name.Rock :
                cardFactory = new RockFactory();
                break;
            case Name.Cactus :
                cardFactory = new CactusFactory();
                break;
            case Name.Ball :
                cardFactory = new BallFactory();
                break;
            case Name.GoalPoint :
                cardFactory = new GoalPointFactory();
                break;
        }

        if (cardFactory != null)
        {
            card = cardFactory.CreateCard();
        }
    }
}

public class Card
{
    public NameClass name { get; set; }
    public IAdjective[] adjectives { get; set; }
}

public abstract class CardFactory 
{
    public Card CreateCard()
    {
        Card card = new Card { name = CreateName(), adjectives = CreateAdjectives() };
        
        return card;
    }

    public abstract NameClass CreateName();
    public abstract IAdjective[] CreateAdjectives();
}

public class RockFactory : CardFactory
{
    public override NameClass CreateName()
    {
        NameClass name = new Rock();
        return name;
    }

    public override IAdjective[] CreateAdjectives()
    {
        IAdjective[] adjectives = new IAdjective[10];
        adjectives[(int)eAdjective.Heavy] = new HeavyAdj();
        
        return adjectives;
    }
}

public class CactusFactory : CardFactory
{
    public override NameClass CreateName()
    {
        NameClass name = new Cactus();
        return name;
    }

    public override IAdjective[] CreateAdjectives()
    {
        IAdjective[] adjectives = new IAdjective[10];
        adjectives[(int)eAdjective.Spiky] = new SpikyAdj();
        
        return adjectives;
    }
}

public class BallFactory : CardFactory
{
    public override NameClass CreateName()
    {
        NameClass name = new Ball();
        return name;
    }

    public override IAdjective[] CreateAdjectives()
    {
        IAdjective[] adjectives = new IAdjective[10];
        adjectives[(int)eAdjective.Push] = new PushAdj();
        adjectives[(int)eAdjective.Pop] = new PopAdj();
        
        return adjectives;
    }
}

public class GoalPointFactory : CardFactory
{
    public override NameClass CreateName()
    {
        NameClass name = new GoalPoint();
        return name;
    }

    public override IAdjective[] CreateAdjectives()
    {
        IAdjective[] adjectives = new IAdjective[10];
        return adjectives;
    }
}
