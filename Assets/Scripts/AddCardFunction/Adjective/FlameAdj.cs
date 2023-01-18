using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Flame;
    private AdjectiveType adjectiveType = AdjectiveType.Contradict;
    private int count = 0;
    
    public Adjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public AdjectiveType GetAdjectiveType()
    {
        return adjectiveType;
    }

    public int GetCount()
    {
        return count;
    }

    public void SetCount(int addCount)
    {
        this.count += addCount;
    }
    
    public void Execute(InteractiveObject thisObject)
    {
        //Debug.Log("this is Flame");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flame : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Flame : this Object -> other Object");
    }
}
