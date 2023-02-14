using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CardManager : Singleton<CardManager>
{
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform cardHolderPoint;
    [SerializeField] Transform cardHolderLeft;
    [SerializeField] Transform cardHolderRight;
    public List<CardController> myCards;
    public List<MainMeneCardController> mainCards;
    [SerializeField] GameObject[] startCards;
    private Coroutine dealCardCoroutine;
    //타겟 상호작용 오브젝트 
    //[HideInInspector]
    public GameObject target;
    GameObject buttons;
    
    public bool isPickCard = false; 
    public bool ableCardCtr = true;
    public bool isEncyclopedia = false;
    public bool isCardDealingDone = false;

    // 마우스로 선택한 카드
    public GameObject pickCard;
    public bool ableAddCard = true;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name=="MainScene")
        {
            CardStart();
        } else
        {
            CardStart();
            buttons = GameObject.Find("IngameCanvas").transform.GetChild(1).gameObject;
        }
    }

    public void CardStart()
    {
        if(dealCardCoroutine !=null)
            StopCoroutine(dealCardCoroutine);
        dealCardCoroutine=StartCoroutine(DealCard());
    }

    //시작 카드를 딜링해주는 메서드 
    IEnumerator DealCard()
    {
        var scene = SceneManager.GetActiveScene();
        yield return new WaitForSeconds(0.1f);
        
        if(scene.name == "MainScene")
        {
            for (int i = 0; i < startCards.Length; i++)
            {
                MainMenuAddCard(startCards[i]);
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            isCardDealingDone = false;
            GameDataManager gameData = GameDataManager.GetInstance;
            int level = GameManager.GetInstance.Level;
            GameObject[] cards = gameData.GetCardPrefabs(gameData.LevelDataDic[level].cardView);
            
            for (int i = 0; i < cards.Length; i++)
            {
                AddCard(cards[i]);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1f);
            isCardDealingDone = true;
            buttons.SetActive(true);
            // 테스트 시 위의 코드를 주석처리하고, 아래의 함수를 사용해주세요.
            // for (int i = 0; i < startCards.Length; i++)
            // {
            //     AddCard(startCards[i]);
            //     yield return new WaitForSeconds(0.5f);
            // }


        }
    }

    //카드를 생성하는 메서드 
    [ContextMenu("AddCard")]
    public void AddCard(GameObject cardPrefab)
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
        var card = cardObject.GetComponent<CardController>();
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "MainScene")
        {
            cardObject.transform.parent = Camera.main.transform;
        }
        myCards.Add(card);
        CardAlignment();
    }

    void MainMenuAddCard(GameObject cardPrefab)
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
        var card = cardObject.GetComponent<MainMeneCardController>();
        var scene = SceneManager.GetActiveScene();
        cardObject.transform.parent = GameObject.Find("MainMenuCards").transform;
        mainCards.Add(card);
        MainCardAlignment();
    }

    //카드를 정렬하는 메서드 
    public void CardAlignment(float time = 2f)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(cardHolderLeft, cardHolderRight, myCards.Count, new Vector3(1f, 1f, 1f));

        for (int i = 0; i < myCards.Count; i++)
        {
            var targetCard = myCards[i];
            
            targetCard.originPRS = originCardPRSs[i];
            targetCard.originPRS.rot = cardHolderPoint.transform.rotation;
            targetCard.MoveTransform(targetCard.originPRS, true, time);
        }
    }

    public void SetInputTrue()
    {
        GameManager.GetInstance.isPlayerCanInput = true;
    }

    //메인화면 카드를 정렬하는 메서드 
    public void MainCardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(cardHolderLeft, cardHolderRight, mainCards.Count, new Vector3(1f, 1f, 1f), true);

        for (int i = 0; i < mainCards.Count; i++)
        {
            var targetCard = mainCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.originPRS.rot = cardHolderPoint.transform.rotation;
            targetCard.MoveTransform(targetCard.originPRS, true, 2f);
        }
    }

    //카드를 둘글게 정렬하는 메서드
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, Vector3 scale, bool isMain = false)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.24f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            case 4: objLerps = new float[] { 0.1f, 0.37f, 0.64f, 0.9f }; break;
            case 5: objLerps = new float[] { 0.05f, 0.275f, 0.5f, 0.725f, 0.95f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                {
                    objLerps[i] = interval * i;
                }
                break;
        }

        Vector3 cardHolderPos = leftTr.parent.localPosition;
        Vector3 leftPos = new Vector3(-1.2f, 0, 0) + cardHolderPos;
        Vector3 rightPos = new Vector3(1.2f, 0, 0) + cardHolderPos;

        if (isMain)
        {
            leftPos = leftTr.position;
            rightPos = rightTr.position;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftPos, rightPos, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (objCount >= 4)
            {
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

    public void CardsHide()
    {
        for (int i = 0; i< myCards.Count; i++)
        {
            myCards[i].gameObject.SetActive(false);
        }
    }

    public void CardsDown()
    {
        for (int i = 0; i < myCards.Count; i++)
        {
            myCards[i].gameObject.transform.
                DOLocalMove(new Vector3(myCards[i].transform.localPosition.x, -1.5f, 1.496f), 1f);
        }
    }

    public void CardsUp()
    {
        for (int i = 0; i < myCards.Count; i++)
        {
            myCards[i].gameObject.transform.
                DOLocalMove(new Vector3(myCards[i].transform.localPosition.x, -0.5f, 1.496f), 1f);
        }
    }

    public void CardsReveal()
    {
        for (int i = 0; i < myCards.Count; i++)
        {
            myCards[i].gameObject.SetActive(true);
        }
    }


}
public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}
