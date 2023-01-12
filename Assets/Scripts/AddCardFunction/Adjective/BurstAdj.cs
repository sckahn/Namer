using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstAdj : IAdjective
{
    private string name = "Burst";
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
        Debug.Log("this is Burst");
    }
    
    public void Function(ObjectClass thisObject, GameObject player, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Burst : this Object > Player");
        }
        else if (!isAffect)
        {
            Debug.Log("Burst : this Object < Player");
        }
    }
    
    public void Function(ObjectClass thisObject, ObjectClass otherObject, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Burst : this Object > other Object");
        }
        else if (!isAffect)
        {
            Debug.Log("Burst : this Object < other Object");
        }
    }
}
