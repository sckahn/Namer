using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Bouncy;
    private int count = 0;
    
    public Adjective GetName()
    {
        return adjectiveName;
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
        //Debug.Log("this is Bouncy");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Bouncy : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherInteractiveObjec)
    {
        //Debug.Log("Bouncy : this Object -> other Object");
    }
}
