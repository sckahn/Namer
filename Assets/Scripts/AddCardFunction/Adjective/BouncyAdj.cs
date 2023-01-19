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

        CheckSurrounding checkingMachine = GameManager.GetInstance.GetCheckSurrounding;
        List<Transform> hits = checkingMachine.GetTransformsAtDirOrNull(thisObject.gameObject, Dir.down);

        if (hits != null)
        {
            if (rigid.velocity.y < 0 && (thisObject.transform.position.y - hits[0].transform.position.y) <= 1.2f)
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
        // 플레이어가 위에 있으면, 힘을 더 주도록 해야함 
        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}