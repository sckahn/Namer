using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAdj : IAdjective
{
    private Adjective name = Adjective.Long;
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
        if (!thisObject.gameObject.GetComponent<Longer>())
        {
            thisObject.gameObject.AddComponent<Longer>();
        }
        
        thisObject.gameObject.GetComponent<Longer>().ObjectScaling();
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Long : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Long : this Object -> other Object");
    }
}
