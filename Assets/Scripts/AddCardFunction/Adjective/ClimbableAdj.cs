using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableAdj : IAdjective
{
    private Adjective name = Adjective.Climbable;
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
        //Debug.Log("this is Climbable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Climbable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Climbable : this Object -> other Object");
    }
}
