using UnityEngine;

public interface IAdjective
{
    public EAdjective GetAdjectiveName();
    public EAdjectiveType GetAdjectiveType();
    public int GetCount();
    public void SetCount(int addCount);
    
    public void Execute(InteractiveObject thisObject);
    public void Execute(InteractiveObject thisObject, GameObject player);
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject);

    public void Abandon(InteractiveObject thisObject);
}
