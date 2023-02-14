using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Flow;
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
        //Debug.Log("this is Null");
        //thisObject.gameObject.layer = 4;
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(FlowObj(thisObject));
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Null : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Null : this Object -> other Object");
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(AbandonFlow(thisObject));
    }
    
    public void Abandon(InteractiveObject thisObject)
    {
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(AbandonFlow(thisObject));
        //thisObject.gameObject.layer = 0;
    }

    IEnumerator FlowObj(InteractiveObject obj)
    {
        yield return null;
        obj.gameObject.layer = 4;
    }

    IEnumerator AbandonFlow(InteractiveObject obj)
    {
        yield return null;
        obj.gameObject.layer = 0;
        obj.SubtractAdjective(EAdjective.Flow);
    }
    
    public IAdjective DeepCopy()
    {
        return new FlowAdj();
    }
}
