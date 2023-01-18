using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BouncyAdj : IAdjective
{
    private Adjective adjectiveName = Adjective.Bouncy;
    private AdjectiveType adjectiveType = AdjectiveType.Repeat;
    private int count = 0;

    #region 통통 꾸밈 카드 멤버변수
    private GameObject myObj;
    private Rigidbody rigid;
    [SerializeField] private float jumpPower = 4.528313f;
    #endregion

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
        if (myObj == null) myObj = thisObject.gameObject;
        if (rigid == null) rigid = myObj.GetComponent<Rigidbody>();
        Collider collider = thisObject.GetComponent<BoxCollider>();
        //Dictionary<Dir, GameObject[]> hits = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(thisObject.gameObject, 0.01f);
        //if (hits.Keys.Contains(Dir.down) && hits[Dir.down].Length != 0)
        //{
        //    Bounce();
        //}

        CheckSurrounding checkingMachine = GameManager.GetInstance.GetCheckSurrounding;
        List<Transform> hits = checkingMachine.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.down);
        Debug.Log("this", thisObject.transform);
        if (hits != null)
        {
            Debug.Log("hit!");
            Bounce();
        }
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Bouncy : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Bouncy : this Object -> other Object");
    }

    private void Bounce()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}