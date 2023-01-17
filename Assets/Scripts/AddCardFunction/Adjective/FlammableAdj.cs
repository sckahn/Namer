using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableAdj : IAdjective
{
    private Adjective name = Adjective.Flammable;
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
        // Debug.Log("this is Flammable");

        if (!thisObject.gameObject.AddComponent<Flameable>())
        {
            thisObject.gameObject.AddComponent<Flameable>();
        }
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flammable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Flammable : this Object -> other Object");
    }
}