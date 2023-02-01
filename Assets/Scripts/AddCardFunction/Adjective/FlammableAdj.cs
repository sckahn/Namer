using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FlammableAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Flammable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;

    #region 잘타는 꾸밈 카드 맴버변수
    private GameObject fireEffect;
    private ParticleSystem fire;
    private bool isOnFire;
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
        ObjectOnFire(thisObject.gameObject);
    }

    private void ObjectOnFire(GameObject targetObj)
    {
        if (isContact)
        {
            isContact = false;
            
            targetObj.GetComponent<InteractiveObject>().StartCoroutine(OnFire(targetObj.gameObject));
        }
    }

    GameObject FindEffect(String prefabName)
    {
        string path = "Assets/Prefabs/Effect/" + prefabName + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found: " + prefabName);
        }

        return prefab;
    }

    IEnumerator OnFire(GameObject thisObj)
    {
        fire.Play();

        yield return new WaitForSeconds(2.5f);

        var ObjectMesh = thisObj.GetComponentInChildren<MeshRenderer>();
        ObjectMesh.enabled = false;
        fire.Stop();

        Vector3 pos = thisObj.transform.position;

        DetectManager.GetInstance.ChangeValueInMap(pos,null);

        yield return new WaitForSeconds(1.5f);

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