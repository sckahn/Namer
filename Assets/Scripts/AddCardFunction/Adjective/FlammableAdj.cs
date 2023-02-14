using System;
using System.Collections;
using UnityEngine;

public class FlammableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Flammable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;

    #region 잘타는 꾸밈 카드 맴버변수
    private GameObject fireEffect;
    private ParticleSystem fire;
    private bool isContact;
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
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
    }

    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        ParticleSetting(thisObject);
        isContact = true;
        ObjectOnFire(thisObject.gameObject, otherObject.gameObject);
    }

    public void Abandon(InteractiveObject thisObject)
    {
        
    }
    
    public IAdjective DeepCopy() 
    {
        return new FlammableAdj();
    }

    private void ObjectOnFire(GameObject targetObj,GameObject otherObject)
    {
        if (isContact)
        {
            isContact = false;
            
            // targetObj.GetComponent<InteractiveObject>().StartCoroutine(OnFire(targetObj.gameObject));
            //코루튼 InteractionSequncer로 변경
            // InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(OnFire(targetObj.gameObject));
            InteractionSequencer.GetInstance.SequentialQueue.Enqueue(OnFire(targetObj, otherObject));
        }
    }

    GameObject FindEffect(String prefabName)
    {
        string path = "Prefabs/Interaction/Effect/" + prefabName;
        GameObject prefab = Resources.Load<GameObject>(path); 
        if (prefab == null)
        {
            Debug.LogError("Prefab not found: " + prefabName);
        }

        return prefab;
    }

    IEnumerator OnFire(GameObject thisObj, GameObject otherObject)
    {
        if (thisObj == null || otherObject == null) yield break;

        int flameIdx = (int)EAdjective.Flame;
        if (otherObject.GetComponent<InteractiveObject>().Adjectives[flameIdx] == null) yield break;
        fire.Play();
        SoundManager.GetInstance.Play(SoundManager.GetInstance.effectClips[0]);
        yield return new WaitForSeconds(2.5f);

        if (thisObj == null || otherObject == null) yield break;

        var ObjectMesh = thisObj.GetComponentInChildren<MeshRenderer>();
        ObjectMesh.enabled = false;
        fire.Stop();

        Vector3 pos = thisObj.transform.position;

        DetectManager.GetInstance.ChangeValueInMap(pos,null);

        yield return new WaitForSeconds(1.5f);

        if (thisObj == null || otherObject == null) yield break;
        GameObject.Destroy(thisObj);
    }

    private void ParticleSetting(InteractiveObject thisObject)
    {
        if (thisObject.transform.Find("FireEffect")) return;

        fireEffect = FindEffect("Fire effect2");
        GameObject effect = GameObject.Instantiate(fireEffect, thisObject.transform);
        effect.name = "FireEffect";
        fire = thisObject.gameObject.GetComponentInChildren<ParticleSystem>();
        fire.Stop();
    }

}