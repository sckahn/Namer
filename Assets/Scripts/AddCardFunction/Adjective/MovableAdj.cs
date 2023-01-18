using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Movable;
    private AdjectiveType adjectiveType = AdjectiveType.Normal;
    private int count = 0;
    
    public Adjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public AdjectiveType GetAdjectiveType()
    {
        return adjectiveType;
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
        //Debug.Log("this is Movable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        player.GetComponent<PlayerMovement>().PlayPushAnimation1();


        //Vector3 direction = (thisObject.transform.position - player.transform.position);
        //if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        //{ 
        //    thisObject.transform.position += Vector3.right;
        //    return;
        //}
        //else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        //{
        //    thisObject.transform.position += Vector3.left;
        //    return;
        //}
        //else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z>0)
        //{
        //    thisObject.transform.position += Vector3.forward;
        //    return;
        //}
        //else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z<0)
        //{
        //    thisObject.transform.position += Vector3.back;
        //    return;
        //}
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Movable : this Object -> other Object");
    }
}

