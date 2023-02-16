using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMeneCardController : MonoBehaviour
{
    public PRS originPRS;
    GameObject cardHolder;
    [SerializeField] GameObject frontCover;
    [SerializeField] BoxCollider bc;
    [SerializeField] GameObject highlight;
    MainUIController mainUIController;
    CardRotate cr;
    GameObject levelSelectCardHolder;

    private void Start()
    {
        cr = this.gameObject.GetComponent<CardRotate>();
        mainUIController = FindObjectOfType<MainUIController>();
        levelSelectCardHolder = GameObject.Find("CardHolders").transform.Find("LevelSelectCardHolder").gameObject;
        cardHolderPicker();
    }

    void cardHolderPicker()
    {
        if (this.gameObject.transform.parent?.name == "MainMenuCards")
        {
            cardHolder = FindObjectOfType<CardManager>().gameObject;
        }
        else
        {
            cardHolder = FindObjectOfType<LevelSelectCardController>().gameObject;
        }

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

    //마우스가 호버중이면 하이라이트 표시를하고 카트를 회전시킨다
    private void OnMouseOver()
    {
        if (GameManager.GetInstance.CurrentState == GameStates.Pause) return;
        if (!CardManager.GetInstance.ableCardCtr) return;
        highlight.SetActive(true);
        cr.enabled = true;
    }

    //마우스가 호버하다가 떠나면 하이라이트 표시를 끄고 카드 회전을 멈추고 처음 상태로 되돌린다
    private void OnMouseExit()
    {
        if (!CardManager.GetInstance.ableCardCtr) return;
        highlight.SetActive(false);
        cr.enabled = false;
        transform.DORotateQuaternion(cardHolder.transform.rotation, 0.5f);
    }

    //카드 영역에서 마우스 누르면 카드 선택 커서로 변경, 카드를 숨김 
    private void OnMouseDown()
    {
        if (GameManager.GetInstance.CurrentState == GameStates.Pause) return;
        if (!CardManager.GetInstance.ableCardCtr) return;
        CardManager.GetInstance.isPickCard = true;
        bc.enabled = false;
        frontCover.SetActive(false);
    }

    //카드 선택 커서 상태에서 상호작용 오브젝트 위에서 마우스를 놓으면 속성 부여,
    //오브젝트 아닌곳에서는 기본 커서로 다시 변경하고 카드를 다시 보이게,
    private void OnMouseUp()
    {
        if (!CardManager.GetInstance.ableCardCtr) return;
        CardManager.GetInstance.isPickCard = false;
        if (CardManager.GetInstance.target != null)
        {
            MainCastCard(this.gameObject.name);
            CardManager.GetInstance.target = null;

            if (this.name != "OptionCard(Clone)")
            {
                Invoke("CardReturn", 1f);
            }
        }
        else if (bc != null)
        {
            CardReturn();
        }
    }

    public void CardReturn()
    {
        bc.enabled = true;
        frontCover.SetActive(true);
    }



    public void MainCastCard(string cardName)
    {
        switch (cardName)
        {
            case "StartCard(Clone)":
                mainUIController.LevelSelectScene();
                break;
            case "EncyclopediaCard(Clone)":
                mainUIController.EncyclopediaScene();
                break;
            case "OptionCard(Clone)":
                mainUIController.OptionPanelOpen();
                break;
            case "CreditCard(Clone)":
                mainUIController.CreditScene();
                break;
            case "MainCard(Clone)":
                mainUIController.MainMenuScene();
                break;
            case "1StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
            case "2StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
            case "3StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
            case "4StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
            case "5StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
            case "6StageCard(Clone)":
                GameManager.GetInstance.SetLevelFromCard(cardName);
                LoadingSceneController.LoadScene("DemoPlay");
                break;
                // LoadingSceneController.LoadScene("JSTESTER");
                //이부분 살짝 수정함
                // GameManager.GetInstance.SetLevelFromCard(cardName);              
            default:
                break;
        }
    }



}
