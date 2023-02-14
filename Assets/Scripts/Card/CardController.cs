using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    // [SerializeField] private CardData cardData;
    [SerializeField] private ECardType cardType;
    [SerializeField] private EName nameType;
    [SerializeField] private EAdjective adjectiveType;
    
    public PRS originPRS;
    GameObject cardHolder;
    [SerializeField] GameObject frontCover;
    [SerializeField] BoxCollider bc;
    [SerializeField] GameObject highlight;
    [SerializeField] GameObject Encyclopedia;
    [SerializeField] private Text UIText;
    [SerializeField] private Text NameAdjUIText;
    CardRotate cr;

    public EAdjective GetAdjectiveTypeOfCard()
    {
        return adjectiveType;
    }

    private void Start()
    {
        cr = this.gameObject.GetComponent<CardRotate>();
        cardHolder = FindObjectOfType<CardManager>().gameObject;

        SetUIText();
    }

    private void SetUIText()
    {
        if (UIText == null || (cardType == ECardType.Name && NameAdjUIText == null))
        {
            Debug.LogError("인스펙터창에서 UI 텍스트를 확인 후 넣어주세요!");
        }
        
        GameDataManager gameDataManager = GameDataManager.GetInstance;
        if (cardType == ECardType.Name)
        {
            UIText.text = gameDataManager.Names[nameType].uiText;
            NameAdjUIText.text = "";
            
            if (gameDataManager.Names[nameType].adjectives != null)
            {
                EAdjective[] adjectives = gameDataManager.Names[nameType].adjectives;
                string delimiter = ", ";
                for (int i = 0; i < adjectives.Length; i++)
                {
                    NameAdjUIText.text += gameDataManager.Adjectives[adjectives[i]].uiText + delimiter;
                }
                
                NameAdjUIText.text = NameAdjUIText.text.TrimEnd(',', ' ');
            }
            else
            {
                NameAdjUIText.text = "순수";
            }
        }
        else if (cardType == ECardType.Adjective)
        {
            UIText.text = gameDataManager.Adjectives[adjectiveType].uiText;
        }

        Text contentText = Encyclopedia.GetComponentInChildren<Text>();
        if (cardType == ECardType.Name)
        {
            contentText.text = gameDataManager.Names[nameType].contentText;
        }
        else if (cardType == ECardType.Adjective)
        {
            contentText.text = gameDataManager.Adjectives[adjectiveType].contentText;
        }
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOLocalMove(prs.pos, dotweenTime);
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


        if (CardManager.GetInstance.isEncyclopedia || GameManager.GetInstance.CurrentState == GameStates.Encyclopedia)
        {
            Encyclopedia.SetActive(true);
            return;
        }

        if (GameManager.GetInstance.CurrentState == GameStates.Encyclopedia)
            return;

        cr.enabled = true;
    }

    //마우스가 호버하다가 떠나면 하이라이트 표시를 끄고 카드 회전을 멈추고 처음 상태로 되돌린다
    private void OnMouseExit()
    {
        if (!CardManager.GetInstance.ableCardCtr) return;
        highlight.SetActive(false);
        if (CardManager.GetInstance.isEncyclopedia || GameManager.GetInstance.CurrentState == GameStates.Encyclopedia)
        {
            Encyclopedia.SetActive(false);
            return;
        }
        cr.enabled = false;
        transform.DORotateQuaternion(cardHolder.transform.rotation, 0.5f);
    }

    //카드 영역에서 마우스 누르면 카드 선택 커서로 변경, 카드를 숨김 
    private void OnMouseDown()
    {
        if (GameManager.GetInstance.CurrentState == GameStates.Pause) return;
        if (CardManager.GetInstance.isEncyclopedia || GameManager.GetInstance.CurrentState == GameStates.Encyclopedia) return;
        if (!CardManager.GetInstance.ableCardCtr) return;
        CardManager.GetInstance.isPickCard = true;
        //transform.DOMove(Input.mousePosition , 5f);
        //transform.DOScale(new Vector3(0, 0, 0), 5f);
        bc.enabled = false;
        frontCover.SetActive(false);

        CardManager.GetInstance.pickCard = gameObject;
    }

    //카드 선택 커서 상태에서 상호작용 오브젝트 위에서 마우스를 놓으면 속성 부여,
    //오브젝트 아닌곳에서는 기본 커서로 다시 변경하고 카드를 다시 보이게,
    private void OnMouseUp()
    {
        if (!CardManager.GetInstance.ableCardCtr) return;
        CardManager.GetInstance.isPickCard = false;
        if (CardManager.GetInstance.target != null && CardManager.GetInstance.ableAddCard)
        {
            CastCard(CardManager.GetInstance.target);
            CardManager.GetInstance.target = null;
            CardManager.GetInstance.myCards.Remove(this.gameObject.GetComponent<CardController>());
            CardManager.GetInstance.CardAlignment();
            Destroy(this.gameObject, 0.5f);
        }
        else if(bc != null)
        {
            bc.enabled = true;
            frontCover.SetActive(true);
            CardManager.GetInstance.ableAddCard = true;
        }
    }

    public void CastCard(GameObject target)
    {
        if (target)
        {
            if (cardType == ECardType.Name)
            {
                target.GetComponent<InteractiveObject>().AddName(nameType);
            }
            else if (cardType == ECardType.Adjective)
            {
                target.GetComponent<InteractiveObject>().AddAdjective(adjectiveType);
            }
        }
    }
}
