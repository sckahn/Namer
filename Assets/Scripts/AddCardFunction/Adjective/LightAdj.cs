using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Light;
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
        //Debug.Log("this is Light");
        GameObject gameObject = new GameObject("Light");
        gameObject.transform.SetParent(thisObject.transform);
        gameObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        gameObject.AddComponent<Light>();
        gameObject.GetComponent<Light>().intensity = 5;
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
