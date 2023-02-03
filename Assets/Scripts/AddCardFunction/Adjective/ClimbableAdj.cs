using UnityEngine;

public class ClimbableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Climbable;
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
        //Debug.Log("this is Climbable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Climbable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Climbable : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
}
