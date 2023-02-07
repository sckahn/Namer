using UnityEngine;

public class FlameAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Flame;
    private EAdjectiveType adjectiveType = EAdjectiveType.Contradict;
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
        //Debug.Log("this is Flame");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flame : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Flame : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
}
