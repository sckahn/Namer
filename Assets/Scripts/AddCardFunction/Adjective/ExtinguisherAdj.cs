using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Extinguisher;
    private EAdjectiveType adjectiveType = EAdjectiveType.Contradict;
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
        //Debug.Log("this is Null");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Null : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Null : this Object -> other Object");
    }
    
    public void Abandon(InteractiveObject thisObject)
    {
        
    }
    
    public IAdjective DeepCopy()
    {
        return new NullAdj();
    }
}
