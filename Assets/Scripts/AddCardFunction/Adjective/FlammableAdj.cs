using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableAdj : MonoBehaviour, IAdjective
{
    private Adjective adjectiveName = Adjective.Flammable;
    private AdjectiveType adjectiveType = AdjectiveType.Contradict;
    private int count = 0;
    
    private bool isContact;
    [Range(0,1)]public float sweepDistance = .5f;

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
        ObjectOnFire();
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Flammable : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Flammable : this Object -> other Object");
    }
    
    [ContextMenu("Flammable Testing")]
    public void ObjectOnFire()
    {
        LookUpFlame();
        if (isContact )
        {
            isContact = false;
            gameObject.SetActive(false);
            // print("Boom");
        }
    }

    
    private void LookUpFlame()
    {
        var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(gameObject,sweepDistance);

        for (int i = 0; i < neighbors.Count; i++)
        {
            if ((Dir)i != Dir.up && (Dir)i != Dir.down)
            {
                foreach (var item in neighbors[(Dir)i])
                {
                    // item object의 오브젝트 클레스 안에 불값으로 특성이 있는지 없는지 가져오기
                    //bool flag =CheckAdj(item) 이거 구현 해달라고 요청
                    if (item.gameObject.name == "Flame")
                    {
                        isContact = true;
                        return;
                    }
                }
            }
        }
    }
}