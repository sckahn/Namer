using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Obtainable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    
    public EAdjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public EAdjectiveType GetAdjectiveType()
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
        //Debug.Log("this is Obtainable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Obtainable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Obtainable : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
}
