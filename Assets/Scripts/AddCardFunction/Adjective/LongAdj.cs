using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAdj : MonoBehaviour, IAdjective
{
    private Adjective name = Adjective.Long;
    private int count = 0;
    
    private int growScale = 1;
    private float shrinkScale = 0.5f;
    private float goalScale;
    private float currentHeight;
    [SerializeField]private float growingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;
    
    
    public Adjective GetName()
    {
        return name;
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
       ObjectScaling();
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Long : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Long : this Object -> other Object");
    }
    [ContextMenu("objScaling")]
    public void ObjectScaling()
    {
        bool flag = CheckGrowable();
        if (flag)
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
        else
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
    }
    private void SetGrowScale()
    {
        goalScale = transform.localScale.y + growScale;
        targetScale = new Vector3(transform.localScale.x, goalScale, transform.localScale.z);
        currentHeight = transform.localScale.y;
    }

    private void SetShrinkScale()
    {
        goalScale = transform.localScale.y * shrinkScale;
        targetScale = new Vector3(transform.localScale.x, goalScale, transform.localScale.z);
        currentHeight = transform.localScale.y;
    }

  
    private bool CheckGrowable()
    {
        var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(gameObject, 1f);
        
       
        var gameObjects = neighbors[Dir.up];
        foreach (var neighborObj in gameObjects)
        {
            if (neighborObj.tag == "InteractObj")
            {
                return false;
            }
        }
        
        return true;
    }

    
    
    // 호성님 껄로 체크하는 메소드 내일 오면 물어보기
    // private bool CheckGrowable()
    // {
    //     print(GameManager.GetInstance.GetCheckSurrounding.name);
    //     var gameObjects = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(Dir.up);
    //     print(gameObjects.Count);
    //     if (gameObjects.Count == 0)
    //         return false;
    //     return true;
    // }



    IEnumerator WrapperCoroutine(bool isGrow)
    {
        if (isGrow)
        {
            SetGrowScale();
            yield return StartCoroutine(ScaleObj());
        }
        else
        {
            SetGrowScale();
            yield return StartCoroutine(ScaleObj());
            SetShrinkScale();
            yield return StartCoroutine(ScaleObj());
        }
    }

   

    IEnumerator ScaleObj()
    {
        currentTime = 0;
        Vector3 startScale = transform.localScale;
        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            yield return null;
           
        }
    }
    
}
