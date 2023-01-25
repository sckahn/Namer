using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Movable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;

    private float currentTime;
    private bool isRoll;
    private int movingSpeed = 1;
    Vector3 target;
    
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
        //Debug.Log("this is Movable");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        if (isRoll) return;

        CheckSurrounding check = GameManager.GetInstance.GetCheckSurrounding;
        
        Vector3 direction = (thisObject.transform.position - player.transform.position);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        {
            if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.right) !=null ) return;

            target = thisObject.transform.position + Vector3.right;

            thisObject.StartCoroutine(MoveObj(thisObject.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        {
            if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.left) != null) return;

            target = thisObject.transform.position + Vector3.left;

            thisObject.StartCoroutine(MoveObj(thisObject.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z > 0)
        {
            if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.forward) != null) return;

            target = thisObject.transform.position + Vector3.forward;

            thisObject.StartCoroutine(MoveObj(thisObject.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z < 0)
        {
            if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.back) != null) return;
                
            target = thisObject.transform.position + Vector3.back;

            thisObject.StartCoroutine(MoveObj(thisObject.gameObject));

            return;
        }
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Movable : this Object -> other Object");
    }

    IEnumerator MoveObj(GameObject obj)
    {
        currentTime = 0;
        Vector3 startPos = obj.transform.localPosition;
        isRoll = true;
        while (currentTime < movingSpeed)
        {
            currentTime += Time.deltaTime;
            obj.transform.localPosition = Vector3.Lerp(startPos, target, currentTime / movingSpeed);
            yield return null;
        }
        isRoll = false;
        //dt.SetNewPosOrSize();
    }
}

