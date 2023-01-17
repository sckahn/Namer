using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAdj : IAdjective
{
    private Adjective name = Adjective.Flame;
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
        //Debug.Log("this is Flame");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flame : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherInteractiveObjec)
    {
        //Debug.Log("Flame : this Object -> other Object");
    }
}
