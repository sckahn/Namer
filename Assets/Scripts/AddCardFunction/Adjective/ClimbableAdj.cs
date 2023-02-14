using System.Collections;
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
        InteractionSequencer.GetInstance.PlayerActionQueue.Enqueue(Climb(thisObject.gameObject, player));
    }

    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Climbable : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
    
    public IAdjective DeepCopy()
    {
        return new ClimbableAdj();
    }

    IEnumerator Climb(GameObject thisGameObj, GameObject player)
    {
        if (DetectManager.GetInstance.GetAdjacentObjectWithDir(thisGameObj, Dir.up, thisGameObj.transform.localScale))
        {
            yield break;
        }
        
        GameManager.GetInstance.isPlayerDoAction = true;
        player.GetComponent<PlayerMovement>().playerEntity.ChangeState(PlayerStates.Climb);
        yield return null;
    }
}
