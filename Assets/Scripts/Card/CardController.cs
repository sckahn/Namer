using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardController : MonoBehaviour
{
    public PRS originPRS;
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject frontCover;
    [SerializeField] BoxCollider bc;
    [SerializeField] GameObject highlight;
    CardComponent cc;
    CardRotate cr;

    private void Start()
    {
        cc = this.gameObject.GetComponent<CardComponent>();
        cr = this.gameObject.GetComponent<CardRotate>();
        cardHolder = FindObjectOfType<CardManager>().gameObject;
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }

    }

    private void Update()
    {
    }

    private void OnMouseOver()
    {
        highlight.SetActive(true);
        cr.enabled = true;
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
        cr.enabled = false;
        transform.DORotateQuaternion(cardHolder.transform.rotation, 0.5f);
    }

    //카드 영역에서 마우스 누르면 카드 선택 커서로 변경, 카드를 숨김 
    private void OnMouseDown()
    {
        CardManager.GetInstance.isPickCard = true;
        //transform.DOMove(Input.mousePosition , 5f);
        //transform.DOScale(new Vector3(0, 0, 0), 5f);
        bc.enabled = false;
        frontCover.SetActive(false);
    }

    //카드 선택 커서 상태에서 상호작용 오브젝트 위에서 마우스를 놓으면 속성 부여,
    //오브젝트 아닌곳에서는 기본 커서로 다시 변경하고 카드를 다시 보이게,
    private void OnMouseUp()
    {
        CardManager.GetInstance.isPickCard = false;
        if (CardManager.GetInstance.target != null)
        {
            cc.AddCard();
            CardManager.GetInstance.target = null;
            Destroy(this.gameObject, 0.5f);
        }
        else if(bc != null)
        {
            bc.enabled = true;
            frontCover.SetActive(true);
        }
    }
}
