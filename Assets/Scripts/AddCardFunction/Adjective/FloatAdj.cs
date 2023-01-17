using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAdj : IAdjective
{
    private Adjective name = Adjective.Float;
    private int count = 0;
    
    public Adjective GetName()
    {
        return name;
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
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherInteractiveObjec)
    {
        //Debug.Log("Float : this Object -> other Object");
    }
}
