using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAdj : IAdjective
{
    private string name = "Long";
    private int count = 0;
    
    public string GetName()
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
    
    public void Function(ObjectClass thisObject)
    {
        Debug.Log("this is Long");
    }
    
    public void Function(ObjectClass thisObject, GameObject player, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Long : this Object > Player");
        }
        else if (!isAffect)
        {
            Debug.Log("Long : this Object < Player");
        }
    }
    
    public void Function(ObjectClass thisObject, ObjectClass otherObject, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Long : this Object > other Object");
        }
        else if (!isAffect)
        {
            Debug.Log("Long : this Object < other Object");
        }
    }
}
