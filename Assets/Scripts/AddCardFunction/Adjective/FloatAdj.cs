using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Float;
    private AdjectiveType adjectiveType = AdjectiveType.Repeat;
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
        //Debug.Log("this is Float");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Float : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Float : this Object -> other Object");
    }
}
