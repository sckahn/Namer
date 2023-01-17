using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAdj : IAdjective
{
    private Adjective name = Adjective.Win;
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
        //Debug.Log("this is Win");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Win : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherInteractiveObjec)
    {
        //Debug.Log("Win : this Object -> other Object");
    }
}
