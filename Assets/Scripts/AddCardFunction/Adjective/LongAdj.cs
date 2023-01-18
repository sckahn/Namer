using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LongAdj : MonoBehaviour, IAdjective
{
    private Adjective adjectiveName = Adjective.Long;
    private AdjectiveType adjectiveType = AdjectiveType.Normal;
    private int count = 0;
    
    private int growScale = 1;
    private float shrinkScale = 0.5f;
    private float goalScale;
    private float currentHeight;
    [SerializeField]private float growingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;
    
    
    public Adjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public AdjectiveType GetAdjectiveType()
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
        ObjectScaling(thisObject);
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
    public void ObjectScaling(InteractiveObject targetObj)
    {
        bool flag = CheckGrowable(targetObj.gameObject);
        print(flag);
        if (flag)
        {
            targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
        }
        else
        {
            targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
        }
    }
    private void SetGrowScale(GameObject targetObj)
    {
        goalScale = targetObj.transform.localScale.y + growScale;
        targetScale = new Vector3(targetObj.transform.localScale.x, goalScale, targetObj.transform.localScale.z);
        currentHeight = targetObj.transform.localScale.y;
    }

    private bool CheckGrowable(GameObject targetObj)
    {
        // var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(targetObj, 1f);
        var test = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(targetObj, Dir.up);
        test = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(targetObj, Dir.up) == null
            ? new List<Transform>()
            : test;
        if (test.Count != 0) return false;
      
        
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



    IEnumerator WrapperCoroutine(bool isGrow,InteractiveObject targetObj)
    {
        if (isGrow)
        {
            SetGrowScale(targetObj.gameObject);
            yield return targetObj.StartCoroutine(ScaleObj(targetObj.gameObject));
        }
        else
        {
            // SetGrowScale(targetObj.gameObject);
            // yield return targetObj.StartCoroutine(ScaleObj(targetObj.gameObject));
            // SetShrinkScale(targetObj.gameObject);
            // yield return targetObj.StartCoroutine(ScaleObj(targetObj.gameObject));
            targetObj.StartCoroutine(Twinkle(targetObj.gameObject));
        }
    }

   

    IEnumerator ScaleObj(GameObject targetObj)
    {
        currentTime = 0;
        Vector3 startScale = targetObj.transform.localScale;
        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            targetObj.transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            yield return null;
           
        }
    }

    IEnumerator Twinkle(GameObject targetObj)
    {
        currentTime = 0;
        var meshRenderer =  targetObj.GetComponentInChildren<MeshRenderer>();
        
        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return null;
        }
        meshRenderer.enabled = true;
    }

    
}
