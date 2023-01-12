using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAdj : IAdjective
{
    private string name = "Push";
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
        //Debug.Log("this is Push");
    }
    
    public void Function(ObjectClass thisObject, GameObject player, bool isAffect)
    {
        Vector3 direction = (thisObject.transform.position - player.transform.position);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        { 
            thisObject.transform.position += Vector3.right;
            return;
        }
        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        {
            thisObject.transform.position += Vector3.left;
            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z>0)
        {
            thisObject.transform.position += Vector3.forward;
            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z<0)
        {
            thisObject.transform.position += Vector3.back;
            return;
        }
    }
    
    public void Function(ObjectClass thisObject, ObjectClass otherObject, bool isAffect)
    {
        if (isAffect)
        {
            //Debug.Log("Push : this Object > other Object");
        }
        else if (!isAffect)
        {
            //Debug.Log("Push : this Object < other Object");
        }
    }
}

