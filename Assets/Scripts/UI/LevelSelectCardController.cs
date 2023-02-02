using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelSelectCardController : MonoBehaviour
{
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform cardHolderPoint;
    [SerializeField] Transform cardHolderLeft;
    [SerializeField] Transform cardHolderRight;
    public List<MainMeneCardController> mainCards;
    [SerializeField] GameObject[] startCards;
    [SerializeField] GameObject[] levelSelectTiles;

    [SerializeField] float levelSelectMovingTime = 1.5f;
    [SerializeField] float cardDealingSpeed = 0.2f;

    private void Start()
    {
        CardStart();
        StartCoroutine(LevelGroudsSetUp());
    }

    void CardStart()
    {
        StartCoroutine(DealCard());
    }

    float ranNum;
    IEnumerator LevelGroudsSetUp()
    {
        for (int i = 0; i < levelSelectTiles.Length; i++)
        {
            ranNum = Random.Range(0f, 2f);
            levelSelectTiles[i].SetActive(true);
            levelSelectTiles[i].transform.DOMove(levelSelectTiles[i].transform.position + new Vector3(0f, 15f, 0), levelSelectMovingTime + ranNum);
            yield return null;
        }

    }

    //시작 카드를 딜링해주는 메서드 
    IEnumerator DealCard()
    {
        var scene = SceneManager.GetActiveScene();
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < startCards.Length; i++)
        {
            MainMenuAddCard(startCards[i]);

            yield return new WaitForSeconds(cardDealingSpeed);
        }

    }



    //카드를 생성하는 메서드 
    void MainMenuAddCard(GameObject cardPrefab)
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
        var card = cardObject.GetComponent<MainMeneCardController>();
        mainCards.Add(card);
        MainCardAlignment();
    }


    //메인화면 카드를 정렬하는 메서드 
    void MainCardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(cardHolderLeft, cardHolderRight, mainCards.Count, new Vector3(1f, 1f, 1f));

        for (int i = 0; i < mainCards.Count; i++)
        {
            var targetCard = mainCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.originPRS.rot = cardHolderPoint.transform.rotation;
            targetCard.MoveTransform(targetCard.originPRS, true, 2f);
        }
    }

    //카드를 둘글게 정렬하는 메서드
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, Vector3 scale)
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

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (objCount >= 4)
            {
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

}
