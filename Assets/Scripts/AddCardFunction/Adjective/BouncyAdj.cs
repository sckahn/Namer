using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Bouncy;
    private EAdjectiveType adjectiveType = EAdjectiveType.Repeat;
    private int count = 0;

    #region 통통 꾸밈 카드 멤버변수
    private GameObject myObj;
    private Rigidbody rigid;
    [SerializeField] private float jumpPower = 4.528313f;

    private float currentTime;
    private float movingSpeed = 1f;
    private Vector3 targetPos;
    // private Vector3 startPos;

    private bool isBouncy;
    float bounceHeight = 2f;
    float bounceSpeed = 3f;
    float bounciness = 4f;

    private float time;
    
    
    #endregion

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

        Vector3 startPos = thisObject.gameObject.transform.position;
        var rb = thisObject.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        CheckBouncible(thisObject.gameObject);
        thisObject.StartCoroutine(BounceCoroutine(thisObject.gameObject,startPos));
        // InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(BounceCoroutine(thisObject.gameObject));
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Bouncy : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Bouncy : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
    
    public IAdjective DeepCopy()
    {
        return new BouncyAdj();
    }
    
    private void Bounce()
    {
        rigid.velocity = Vector3.zero;
        // 플레이어가 위에 있으면, 힘을 더 주도록 해야함 
        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    Vector3 SetTargetPos(GameObject targetObj)
    {
        Vector3 startPos = targetObj.transform.position;
        return startPos;
    }

    void CheckBouncible(GameObject targetObj)
    {
        var underObject = DetectManager.GetInstance.GetAdjacentObjectWithDir(targetObj, Dir.down, targetObj.transform.lossyScale);
        var upperObject =
            DetectManager.GetInstance.GetAdjacentObjectWithDir(targetObj, Dir.up, targetObj.transform.lossyScale);
        if (underObject != null && upperObject == null)
        {
            isBouncy = true;
        }
    }
    /*
     * -------TestCase---------
     * 1. 캐릭터가 아래로 들어갔을때 -> 캐릭터를 박아버리네 
     * 2. 뛰면서 2단계위에 뭐가 있을때
     * 3. 뛰면서 위에 캐릭터 있고 그 위에 물체가 있을때
     * 4. 
     */
  
    private IEnumerator BounceCoroutine(GameObject obj,Vector3 startPos)
    {
        float time = 0;
        Debug.Log(obj,obj.transform);
        while (isBouncy)
        {
            time += Time.deltaTime * bounceSpeed;
            float y = Mathf.Clamp(Mathf.Sin(time) * bounceHeight,0,1.1f);
            // float y = Mathf.Sin(time) * bounceHeight;
            Debug.DrawLine(obj.transform.position,new Vector3(obj.transform.position.x,obj.transform.position.y -1f,obj.transform.position.z),Color.red);
            obj.transform.position = Vector3.Lerp(obj.transform.position, startPos + new Vector3(0, y, 0), Time.deltaTime * bounciness);
            
            yield return new WaitForEndOfFrame();
        }
    }
    
    /*
     * 1. 크기만큼 y축으로 올라가따가 내려가따가
     * 1. check if it can go up
     * 2. make it go up and down continuously
     * 
     * 2. 캐릭터가  위에 올라갈수 있음 
     */
    
}