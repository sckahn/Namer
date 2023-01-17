using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainableAdj : IAdjective
{
    private Adjective name = Adjective.Obtainable;
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
        //Debug.Log("this is Obtainable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Obtainable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherInteractiveObjec)
    {
        //Debug.Log("Obtainable : this Object -> other Object");
    }
}
