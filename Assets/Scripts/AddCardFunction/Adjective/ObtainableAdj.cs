using UnityEngine;

public class ObtainableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Obtainable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    
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
        //Debug.Log("this is Obtainable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        if (thisObject.GetObjectName() != EName.Null)
        {
            ObtainNameCard(thisObject, thisObject.GetObjectName());
        }
        else
        {
            ObtainAdjectiveCard(thisObject, thisObject.Adjectives);
        }
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Obtainable : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
    
    // Method of Adjective
    public IAdjective DeepCopy()
    {
        return new ObtainableAdj();
    }
    
    private void ObtainNameCard(InteractiveObject thisObject, EName nameType)
    {
        GameObject cardPrefab = Resources.Load("Prefabs/Cards/01. NameCard/" + GameDataManager.GetInstance.Names[nameType].cardPrefabName) as GameObject;
        CardManager.GetInstance.AddCard(cardPrefab);
        
        GameObject.Destroy(thisObject.gameObject, 0.5f);
    }
    
    private void ObtainAdjectiveCard(InteractiveObject thisObject, IAdjective[] adjectives)
    {
        foreach (IAdjective adjective in adjectives)
        {
            if (adjective != null && adjective.GetAdjectiveName() != EAdjective.Obtainable)
            {
                GameObject cardPrefab = Resources.Load("Prefabs/Cards/02. AdjustCard/" + GameDataManager.GetInstance.Adjectives[adjective.GetAdjectiveName()].cardPrefabName) as GameObject;
                CardManager.GetInstance.AddCard(cardPrefab);
                break;
            }
        }
        
        GameObject.Destroy(thisObject.gameObject, 0.5f);
    }
}
