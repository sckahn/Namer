using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Float;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    private float currentTime;
    private float movingSpeed = 1f;
    private float length = 0.08f;
    private float speed = 0.8f;

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

        if(DetectManager.GetInstance.GetAdjacentObjectWithDir(obj, Dir.up) == null)
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            currentTime = 0;
            if (obj != null) yield return null;
            Vector3 startPos = obj.transform.position;
            //Debug.Log(startPos);
            DetectManager.GetInstance.SwapBlockInMap(startPos, startPos + Vector3.up);

            while (obj != null && obj.GetComponent<InteractiveObject>().CheckAdjective(adjectiveName) && currentTime < movingSpeed)
            {
                currentTime += Time.deltaTime;
                obj.transform.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up, currentTime / movingSpeed);
                yield return InteractionSequencer.GetInstance.WaitUntilPlayerInteractionEnd(this);
            }

            //---------수정한부분
            DetectManager.GetInstance.StartDetector();
            //------------
            //Debug.Log(obj.transform.position);

            yield return new WaitForSeconds(0.2f);
            if (obj != null) yield return null;
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

    IEnumerator GravityOn(GameObject thisObject)
    {
        yield return null;
        if (thisObject != null)
        {
            if (!thisObject.GetComponent<InteractiveObject>().CheckAdjective(EAdjective.Bouncy))
            {
                var rb = thisObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}
