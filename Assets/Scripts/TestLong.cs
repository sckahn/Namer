using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TestLong : MonoBehaviour
{
    private EAdjective adjectiveName = EAdjective.Long;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    
    private int growScale = 1;
    private float shrinkScale = 0.5f;
    private float goalScale;
    private float currentHeight;
    [SerializeField] private float growingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;

    public void Start()
    {
        bool flag = DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.transform, Dir.up);

        if (!flag)
        {
            SetGrowScale(this.gameObject);
            this.StartCoroutine(ScaleObj(this.gameObject));
        }
        else
        {
            this.StartCoroutine(Twinkle(this.gameObject));
        }
    }

    public void AddcardLong()
    {
        bool flag = DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.transform, Dir.up);
        if (!flag)
        {
            SetGrowScale(this.gameObject);
            StartCoroutine(ScaleObjAddCard(this.gameObject));
        }
        else
        {
            this.StartCoroutine(Twinkle(this.gameObject));
        }
    }


    [ContextMenu("objScaling")]
    public void ObjectScaling(InteractiveObject targetObj)
    {
        bool flag = CheckGrowable(targetObj.gameObject);
        if (flag)
        {
            SetGrowScale(targetObj.gameObject);
            targetObj.StartCoroutine(ScaleObj(targetObj.gameObject));
            // targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
        }
        else
        {
            targetObj.StartCoroutine(Twinkle(targetObj.gameObject));
            // targetObj.StartCoroutine(WrapperCoroutine(flag,targetObj));
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
        var test = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(targetObj, Dir.up);
        if (test != null)
        {
            foreach (var item in test)
            {
                if (item.gameObject.tag == "Player")
                    return true;
            }
            if (test.Count != 0) return false;
            
        }
        return true;
    }

    IEnumerator WrapperCoroutine(bool isGrow,InteractiveObject targetObj)
    {
        if (isGrow)
        {
            SetGrowScale(targetObj.gameObject);
            yield return targetObj.StartCoroutine(ScaleObj(targetObj.gameObject));
        }
        else
        {
            targetObj.StartCoroutine(Twinkle(targetObj.gameObject));
        }
    }

    // 오브젝트의 y축 스케일을 조정
    IEnumerator ScaleObj(GameObject targetObj)
    {
        currentTime = 0;
        Vector3 startScale = targetObj.transform.localScale;

        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            targetObj.transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            
            while (InteractionMaster.GetInstance.isPause == true)
            {
                yield return null;
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    IEnumerator ScaleObjAddCard(GameObject targetObj)
    {
        currentTime = 0;
        Vector3 startScale = targetObj.transform.localScale;
        InteractionMaster.GetInstance.isPause = true;
        
        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            targetObj.transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            
            yield return new WaitForFixedUpdate();
        }

        InteractionMaster.GetInstance.isPause = false;
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
        
        while (currentTime < growingSpeed+3f)
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
}

