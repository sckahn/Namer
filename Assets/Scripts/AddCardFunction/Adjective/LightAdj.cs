using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAdj : IAdjective
{
    private Adjective name = Adjective.Light;
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
        //Debug.Log("this is Light");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Light : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Light : this Object -> other Object");
    }
}
