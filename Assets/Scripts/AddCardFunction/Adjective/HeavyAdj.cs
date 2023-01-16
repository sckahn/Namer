using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAdj : IAdjective
{
    private string name = "Heavy";
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
    
    public void Execute(ObjectClass thisObject)
    {
        //Debug.Log("this is Heavy");
    }

    public void Execute(ObjectClass thisObject, GameObject player, bool isAffect)
    {
        if (isAffect)
        {
            //Debug.Log("Heavy : this Object > Player");
        }
        else if (!isAffect)
        {
            //Debug.Log("Heavy : this Object < Player");
        }
    }
    
    public void Execute(ObjectClass thisObject, ObjectClass otherObject, bool isAffect)
    {
        if (isAffect)
        {
            //Debug.Log("Heavy : this Object > other Object");
        }
        else if (!isAffect)
        {
            //Debug.Log("Heavy : this Object < other Object");
        }
    }
}
