using System;
using UnityEditor;
using UnityEngine;

public class LightAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Light;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;

    #region LightAdjClassMem

    private bool isLighting;
    private GameObject lightSource;

    #endregion
    
    public EAdjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public EAdjectiveType GetAdjectiveType()
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
        // GameObject gameObject = new GameObject("Light");
        // gameObject.transform.SetParent(thisObject.transform);
        // gameObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        // gameObject.AddComponent<Light>();
        // gameObject.GetComponent<Light>().intensity = 5;
    
        if (!isLighting)
        {
            LetThereBeLight(thisObject.gameObject);
        }
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Light : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Light : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        if (isLighting)
        {
            LeteThereNoLight(thisObject.gameObject);
        }
    }

    public IAdjective DeepCopy()
    {
        return new LightAdj();
    }

    private void LetThereBeLight(GameObject targetObj)
    {
        isLighting = true;
        var lightSource =FindEffect("LightEffect");
        GameObject.Instantiate(lightSource, targetObj.transform);
    }
    
    void LeteThereNoLight(GameObject targetObj)
    {
        if (isLighting)
        {
            isLighting = false;
            // lightSource.GetComponent<Light>().enabled = false;
            Debug.Log(targetObj.GetComponentInChildren<Light>().gameObject.name,targetObj.GetComponentInChildren<Light>().gameObject.transform);
            GameObject.Destroy(targetObj.GetComponentInChildren<Light>().gameObject);
            // GameObject.Destroy(lightSource);

        }
    }
    GameObject FindEffect(String prefabName)
    {
        string path = "Prefabs/Interaction/Effect/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path); 
        if (prefab == null)
        {
            Debug.LogError("Prefab not found: " + prefabName);
        }

        return prefab;
    }
}
