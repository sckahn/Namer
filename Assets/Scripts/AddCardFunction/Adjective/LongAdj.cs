using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LongAdj : MonoBehaviour, IAdjective
{
    private EAdjective adjectiveName = EAdjective.Long;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    
    private int growScale = 1;
    private float shrinkScale = 0.5f;
    private float goalScale;
    private Vector3 prevScale;
    [SerializeField]private float growingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;
    
    
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

    public void Abandon(InteractiveObject thisObject)
    {
        ShrinkObj(thisObject);
    }

    public void ShrinkObj(InteractiveObject thisObject)
    {
        var canShrink = SetShrinkScale(thisObject.gameObject);
        if (canShrink)
        {
            DetectManager.GetInstance.OnObjectScaleChanged(targetScale,thisObject.transform);
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(ScaleObj(thisObject.gameObject));
        }
    }
    public void ObjectScaling(InteractiveObject targetObj)
    {
        bool flag = CheckGrowable(targetObj.gameObject);
        if (flag)
        {
            SetGrowScale(targetObj.gameObject);
            DetectManager.GetInstance.OnObjectScaleChanged(targetScale,targetObj.transform);
          
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(ScaleObj(targetObj.gameObject));
            // targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
        }
        else
        {
            prevScale = targetObj.transform.lossyScale;
            InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(Twinkle(targetObj.gameObject));
            // targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
        }
    }
    private void SetGrowScale(GameObject targetObj)
    {
        prevScale = targetObj.transform.lossyScale;
        goalScale = targetObj.transform.localScale.y + growScale;
        targetScale = new Vector3(targetObj.transform.localScale.x, goalScale, targetObj.transform.localScale.z);
        // currentHeight = targetObj.transform.localScale.y;
    }

    private bool SetShrinkScale(GameObject targetObj)
    {


        if (Mathf.RoundToInt(targetObj.transform.localScale.y) == 1) return false;
        if (prevScale.y != targetObj.transform.lossyScale.y)
        {
            goalScale = targetObj.transform.localScale.y - growScale;
            prevScale = targetObj.transform.lossyScale;
            targetScale = new Vector3(targetObj.transform.localScale.x, goalScale, targetObj.transform.localScale.z);
            return true;
        }

        return false;
        // currentHeight = targetObj.transform.localScale.y;
    }

    private bool CheckGrowable(GameObject targetObj)
    {
        //1. check if it is growable
        //2. after grow update object arr 
        
        // var test = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(targetObj, Dir.up);
        // var upperNeighbor = DetectManager.GetInstance.GetAdjacentObjectWithDir(targetObj, Dir.up);
        var upperNeighbor=DetectManager.GetInstance.GetAdjacentObjectWithDir(targetObj, Dir.up, targetObj.transform.lossyScale);

        if (upperNeighbor != null)
        {
            if (upperNeighbor.tag == "Player")
                return true;
            else
            {
                return false;
            }
        }
        return true;
    }

   
    // 오브젝트의 y축 스케일을 조정
    IEnumerator ScaleObj(GameObject targetObj)
    {
        currentTime = 0;
        Vector3 startScale = targetObj.transform.localScale;
        while (targetObj != null && currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            targetObj.transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            yield return null;
           
        }
        //수정한 부분
        DetectManager.GetInstance.StartDetector(new List<GameObject>() { targetObj });
        //수정한 부분 
    }

    //빨간색으로 반짝이는 효과
    IEnumerator Twinkle(GameObject targetObj)
    {
        float minBrightness = 0.5f;
        float maxBrightness = 1.0f;
        float speed = 1.0f;
        float currentBrightness;
        var renderer = targetObj.GetComponentInChildren<Renderer>().material;
        Color originalcolor = renderer.color;
         
        currentTime = 0;
        var meshRenderer =  targetObj.GetComponentInChildren<MeshRenderer>();
        
        while (targetObj != null && currentTime < growingSpeed+3f)
        {
            currentTime += Time.deltaTime;
            // meshRenderer.enabled = !meshRenderer.enabled;
            float noise = Mathf.PerlinNoise(Time.time * speed, 0);
            currentBrightness = Mathf.Lerp(minBrightness, maxBrightness, noise);

            // 여기서 메터리얼의 색깔을 바꾸어서 메터리얼이 달라진다.-> 깃헙데스크톱에 바뀐게 계속 생성된다.
            renderer.color = new Color(1, currentBrightness, currentBrightness);
            yield return null;
        }

        renderer.color = originalcolor;
        meshRenderer.enabled = true;
    }

    #region TestCaseCode

    /*
     * Test Cases
     * 1. 그로우 안하고 슈링크 하기 v
     * 2. 그로우 실패후 슈링크 하기 v
     * 3. 그로우 후 이동해서 flame에 가다가 되기 v
     * 4. 그로우 -> 슈링크후 flame 가다가 되기 안되는게 정상 v  
     */
    [ContextMenu("Grow")]
    public void Grow()
    {
        ObjectScaling(gameObject.GetComponent<InteractiveObject>());
    }

    [ContextMenu("Shrink")]
    public void Shrink()
    {
        ShrinkObj(gameObject.GetComponent<InteractiveObject>());
    }
    

    #endregion

    
}
