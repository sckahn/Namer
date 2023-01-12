using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform cardHolderPoint;
    [SerializeField] Transform cardHolderLeft;
    [SerializeField] Transform cardHolderRight;
    [SerializeField] List<CardController> myCards;

    //카드를 생성하는 메서드 
    [ContextMenu("AddCard")]
    void AddCard()
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
        var card = cardObject.GetComponent<CardController>();
        myCards.Add(card);
        CardAlignment();
    }

    //카드를 정렬하는 메서드 
    void CardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(cardHolderLeft, cardHolderRight, myCards.Count, 0.5f, new Vector3(0.4f, 0.6f, 0.01f));

        for (int i = 0; i < myCards.Count; i++)
        {
            var targetCard = myCards[i];

            targetCard.originPRS = originCardPRSs[i];
            Debug.Log(cardHolderPoint.position);
            targetCard.MoveTransform(targetCard.originPRS, true, 2f);
        }
    }

    //카드를 둘글게 정렬하는 메서드
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.24f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
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
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
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
