using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Float;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    #region 둥둥 멤버변수
    private float currentTime;
    private float movingSpeed = 1f;
    private float length = 0.08f;
    private float speed = 0.8f;
    GameObject startObj;
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
        //Debug.Log("this is Float");
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(FloatObj(thisObject.gameObject));
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Float : this Object -> Player");
    }

    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Float : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(GravityOn(thisObject.gameObject));
    }

    public IAdjective DeepCopy()
    {
        return new FloatAdj();
    }

    IEnumerator FloatObj(GameObject obj)
    {

        if (DetectManager.GetInstance.GetAdjacentObjectWithDir(obj, Dir.up, (int)obj.transform.lossyScale.y) == null)
        {
            //바로 밑에 있는 타일 검사해서 있으면 전 과정 돌리기
            //바로 밑에 타일이 없으면 올라가는 코루틴 pass 둥둥 이펙트만 살리기
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            currentTime = 0;
            if (obj != null) yield return null;
            Vector3 startPos = obj.transform.position;
            startObj = DetectManager.GetInstance.GetAdjacentObjectWithDir(obj, Dir.down, 1);

            if (startObj != null)
            {
                DetectManager.GetInstance.SwapBlockInMap(startObj.transform.position + Vector3.up, startObj.transform.position + Vector3.up + Vector3.up);

                while (obj != null && obj.GetComponent<InteractiveObject>().CheckAdjective(adjectiveName) && currentTime < movingSpeed)
                {
                    currentTime += Time.deltaTime;
                    obj.transform.localPosition = Vector3.Lerp(startPos, startObj.transform.position + Vector3.up + Vector3.up, currentTime / movingSpeed);
                    yield return InteractionSequencer.GetInstance.WaitUntilPlayerInteractionEnd(this);
                }

                DetectManager.GetInstance.StartDetector();

                yield return new WaitForSeconds(0.2f);
                if (obj != null) yield return null;
            }

            obj.transform.localPosition = new Vector3(startPos.x, Mathf.CeilToInt(startPos.y), startPos.z);

            Vector3 currentPos = obj.transform.GetChild(0).localPosition;

            while (obj != null && obj.GetComponent<InteractiveObject>().CheckAdjective(adjectiveName))
            {
                currentTime += Time.deltaTime * speed;

                obj.transform.GetChild(0).
                    localPosition = new Vector3(obj.transform.GetChild(0).localPosition.x, currentPos.y + Mathf.Sin(currentTime) * length, obj.transform.GetChild(0).localPosition.z);
                yield return InteractionSequencer.GetInstance.WaitUntilPlayerInteractionEnd(this);
            }
            if (obj != null)
                Abandon(obj.GetComponent<InteractiveObject>());
        }


        else if (DetectManager.GetInstance.GetAdjacentObjectWithDir(obj, Dir.up).GetComponent<InteractiveObject>())
        {
            yield break;
        }
    }

    IEnumerator GravityOn(GameObject gameObject)
    {
        yield return null;
        if (gameObject != null)
        {
            //abandon 시 mesh의 위치를 되돌리는 코드
            gameObject.transform.GetChild(0).localPosition = new Vector3(0.194f, 0.817f, -0.476f);

            if (!gameObject.GetComponent<InteractiveObject>().CheckAdjective(EAdjective.Bouncy))
            {
                var rb = gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}
