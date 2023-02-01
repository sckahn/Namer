using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Hardware;
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
        DetectManager detectManager = DetectManager.GetInstance;
        var neihbors =detectManager.GetAdjacentsDictionary(thisObject.gameObject,thisObject.transform.lossyScale);

        var prevLocatio = thisObject.gameObject.transform.position;
            Vector3 direction = (thisObject.transform.position - player.transform.position);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        {
            // if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.right) !=null ) return;
            if (neihbors.ContainsKey(Dir.right)) return;
            target = thisObject.transform.position + Vector3.right;
            detectManager.SwapBlockInMap(prevLocatio, target);
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(MoveObj(thisObject.gameObject));
            // thisObject.StartCoroutine(MoveObj(thisObject.gameObject));
        
            return;
        }
        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        {
            // if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.left) != null) return;
            if (neihbors.ContainsKey(Dir.left)) return;
        
            target = thisObject.transform.position + Vector3.left;
            detectManager.SwapBlockInMap(prevLocatio, target);
            // thisObject.StartCoroutine(MoveObj(thisObject.gameObject));
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(MoveObj(thisObject.gameObject));
        
            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z > 0)
        {
            // if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.forward) != null) return;
            if (neihbors.ContainsKey(Dir.forward)) return;
            target = thisObject.transform.position + Vector3.forward;
            detectManager.SwapBlockInMap(prevLocatio, target);
            // thisObject.StartCoroutine(MoveObj(thisObject.gameObject));
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(MoveObj(thisObject.gameObject));
            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z < 0)
        {
            // if (check.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.back) != null) return;
            if (neihbors.ContainsKey(Dir.back))return;
            target = thisObject.transform.position + Vector3.back;
            detectManager.SwapBlockInMap(prevLocatio, target);
        
            // thisObject.StartCoroutine(MoveObj(thisObject.gameObject));
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(MoveObj(thisObject.gameObject));
            return;
        }
        
        
        
        

    }



    //현제 물체 위치 찍는 메소드 test용
    void CheckArrChange(DetectManager detectManager, GameObject go)
    {
        var foreTestPurpos = detectManager.GetObjectsData();
        for (int i = 0; i < foreTestPurpos.GetLength(0); i++)
        {
            for (int j = 0; j < foreTestPurpos.GetLength(1); j++)
            {
                for (int k = 0; k < foreTestPurpos.GetLength(2); k++)
                {
                    if (foreTestPurpos[i, j, k] == null) continue;
                    if (go == foreTestPurpos[i, j, k])
                    {
                        Debug.Log("----------------------------------");
                
                        Debug.Log($"{i} {j} {k}");
                        Debug.Log(go, go.transform);       
                        Debug.Log(foreTestPurpos[i,j,k], foreTestPurpos[i,j,k].transform);
                    }
                }
            }
            
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
        //수정한 부분
        DetectManager.GetInstance.StartDetector(new List<GameObject>() { obj });
        //수정한 부분 
    }
}

