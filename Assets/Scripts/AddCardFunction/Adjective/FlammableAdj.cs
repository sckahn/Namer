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
        //ParticleSetting(thisObject);
        //ObjectOnFire(thisObject);
        //Debug.Log("execute1");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flammable : this Object -> Player");
        // ObjectOnFire(thisObject);
    }

    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        Debug.Log("Flammable : this Object -> other Object");
        //if (otherObject.Adjectives[(int)EAdjective.Flame].GetAdjectiveName() == EAdjective.Flame)
        //{
        //    isContact = true;
        //    ObjectOnFire(thisObject.gameObject);
        //}
        ParticleSetting(thisObject);
        isContact = true;
        ObjectOnFire(thisObject.gameObject);
    }

    [ContextMenu("Flammable Testing")]
    private void ObjectOnFire(InteractiveObject targetObj)
    {
        LookUpFlame(targetObj.gameObject);
        if (isContact)
        {
            isContact = false;
            isOnFire = true;
            targetObj.StartCoroutine(OnFire(targetObj.gameObject));
            isOnFire = false;
            // targetObj.gameObject.SetActive(false);
            // print("Boom");
        }
    }

    private void ObjectOnFire(GameObject targetObj)
    {
        // LookUpFlame(targetObj.gameObject); // 이거 이중 체크 인데..
        if (isContact)
        {
            isContact = false;
            isOnFire = true;
            targetObj.GetComponent<InteractiveObject>().StartCoroutine(OnFire(targetObj.gameObject));
            isOnFire = false;
            // targetObj.gameObject.SetActive(false);
            // print("Boom");
        }
    }



    private void LookUpFlame(GameObject targetObj)
    {

        Dictionary<Dir, List<Transform>> crossDirDict = new Dictionary<Dir, List<Transform>>();
        for (int i = 0; i < Enum.GetNames(typeof(Dir)).Length; i++)
        {
            if ((Dir)i != Dir.down && (Dir)i != Dir.up && (Dir)i != Dir.Null)
            {
                var objList = GameManager.GetInstance.GetCheckSurrounding.GetTransformsAtDirOrNull(targetObj, (Dir)i);
                if (objList != null)
                    crossDirDict.Add((Dir)i, objList);
            }
        }
        foreach (var ol in crossDirDict)
        {
            foreach (var item in ol.Value)
            {
                Debug.Log(item.name + ol.Key);
                Debug.Log(item.gameObject.name);
                //interactiveobject component가 없을 경우 에러뜸 임시로 예외처리 해줌
                if (item.gameObject.GetComponent<InteractiveObject>() == null) continue;

                if (item.gameObject.GetComponent<InteractiveObject>().Adjectives[(int)EAdjective.Flame]
                        .GetAdjectiveName() == EAdjective.Flame)
                {
                    isContact = true;
                }
            }
        }
        Debug.Log(isContact);
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

        Debug.Log("Pos : " + pos.x + pos.y + pos.z);

        DetectManager.GetInstance.ChangeValueInMap(pos,null);

        yield return new WaitForSeconds(1.5f);
        GameObject.Destroy(thisObj);
        //while (isOnFire)
        //{
        //    fire.Play();
        //    yield return new WaitForSeconds(5f);
        //    //waith until phase has changed
        //    // yield return new WaitUntil();
        //    // yield return new DOTweenCYInstruction.WaitForCompletion();
        //}
    }

    private void ParticleSetting(InteractiveObject thisObject)
    {
        fireEffect = FindEffect("Fire effect2");
        GameObject.Instantiate(fireEffect, thisObject.transform);
        fire = thisObject.gameObject.GetComponentInChildren<ParticleSystem>();
        fire.Stop();
    }

}