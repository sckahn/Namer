using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAdj : IAdjective
{
    private string name = "Light";
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
        Debug.Log("this is Light");
    }

    public void Function(ObjectClass thisObject, GameObject player, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Light : this Object > Player");
        }
        else if (!isAffect)
        {
            Debug.Log("Light : this Object < Player");
        }
    }
    
    public void Function(ObjectClass thisObject, ObjectClass otherObject, bool isAffect)
    {
        if (isAffect)
        {
            Debug.Log("Light : this Object > other Object");
        }
        else if (!isAffect)
        {
            Debug.Log("Light : this Object < other Object");
        }
    }
}
