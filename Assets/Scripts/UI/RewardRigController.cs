using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardRigController : MonoBehaviour
{
    GameObject rightArrow;
    GameObject leftArrow;
    GameObject[] rewardCards;
    [SerializeField] List<GameObject> currentRewardCards;
    bool[] rewardPage;

    int rightArrowCount;

    private void OnEnable()
    {
        Init();
        RewardCardsInstantiate();        
    }

    private void Init()
    {
        rightArrow =
            GameObject.Find("IngameCanvas").transform.
            Find("StageClearPanel").transform.
            Find("RewardPanel").transform.
            Find("RightArrowBtn").gameObject;
        leftArrow =
            GameObject.Find("IngameCanvas").transform.
            Find("StageClearPanel").transform.
            Find("RewardPanel").transform.
            Find("LeftArrowBtn").gameObject;
        rightArrowCount = 0;
        rightArrow.GetComponent<Button>().onClick.AddListener(delegate {
            RightArrowClick();
        });
        leftArrow.GetComponent<Button>().onClick.AddListener(delegate {
            LeftArrowClick();
        });
    }

    private void RewardCardsInstantiate()
    {

        rewardCards = GameDataManager.GetInstance.GetRewardCardEncyclopedia();
        if (rewardCards == null) return;
        rewardPage = new bool[(rewardCards.Length / 5) + 1];

        if (rewardCards.Length <= 5)
        {
            for (int i = 0; i < rewardCards.Length; i++)
            {
                var cardObject = (GameObject)Instantiate(rewardCards[i], new Vector3(0, 0, 0), Quaternion.identity);
                cardObject.transform.parent = GameObject.Find("LayoutGroup").transform;
                cardObject.transform.localPosition = new Vector3(-1f + (0.5f * (i % 5)), 0, 0);
                cardObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentRewardCards.Add(cardObject);
                cardObject.layer = 6;
                ChangeChildrenLayer(cardObject.transform, 6);
                cardObject.transform.GetChild(0).gameObject.SetActive(true);

            }
            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                var cardObject = (GameObject)Instantiate(rewardCards[i], new Vector3(0, 0, 0), Quaternion.identity);
                cardObject.transform.parent = GameObject.Find("LayoutGroup").transform;
                cardObject.transform.localPosition = new Vector3(-1f + (0.5f * (i % 5)), 0, 0);
                cardObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentRewardCards.Add(cardObject);
                cardObject.layer = 6;
                ChangeChildrenLayer(cardObject.transform, 6);
                cardObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }

        rewardPage[rightArrowCount] = true;
    }

    int currentLoop;

    public void RightArrowClick()
    {
        rightArrowCount++;
        for (int i = 5 * (rightArrowCount - 1); i < (5 * rightArrowCount); i++)
        {
            currentRewardCards[i].SetActive(false);
        }

        currentLoop = Mathf.Min(5 * (rightArrowCount + 1), rewardCards.Length);

        if (!rewardPage[rightArrowCount])
        {
            for (int i = (5 * rightArrowCount); i < currentLoop; i++)
            {
                var cardObject = (GameObject)Instantiate(rewardCards[i], new Vector3(0, 0, 0), Quaternion.identity);
                cardObject.transform.parent = GameObject.Find("LayoutGroup").transform;
                cardObject.transform.localPosition =
                    new Vector3(-1f + (0.5f * (i % 5)),
                    0, 0);
                cardObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentRewardCards.Add(cardObject);
                cardObject.layer = 6;
                ChangeChildrenLayer(cardObject.transform, 6);
                cardObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            rewardPage[rightArrowCount] = true;
            leftArrow.SetActive(true);
        }
        else
        {
            for (int i = (5 * rightArrowCount); i < currentLoop; i++)
            {
                currentRewardCards[i].SetActive(true);
            }
            leftArrow.SetActive(true);
        }

        //보상 카드가 더 남아있으면 오른쪽 화살표 버튼 생성 
        if (5 * (rightArrowCount + 1) >= rewardCards.Length)
        {
            rightArrow.SetActive(false);
        }
        else
        {
            rightArrow.SetActive(true);
        }
    }

    public void LeftArrowClick()
    {
        currentLoop = Mathf.Min(5 * (rightArrowCount + 1), rewardCards.Length);

        for (int i = 5 * (rightArrowCount); i < currentLoop; i++)
        {
            currentRewardCards[i].SetActive(false);
        }
        for (int i = (5 * (rightArrowCount - 1)); i < (5 * rightArrowCount); i++)
        {
            currentRewardCards[i].SetActive(true);
        }

        rightArrowCount--;

        if (rightArrowCount == 0)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
        else
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
    }


    void ChangeChildrenLayer(Transform t, int newLayer)
    {
        t.gameObject.layer = newLayer;
        foreach (Transform child in t)
        {
            ChangeChildrenLayer(child, newLayer);
        }
    }

}
