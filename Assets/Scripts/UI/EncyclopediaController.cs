using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaController : MonoBehaviour
{
    [SerializeField] GameObject layoutGroup;
    [SerializeField] float wheelSpeed = 0.1f;
    [SerializeField] float maxHeight = 1f;
    [SerializeField] Scrollbar scrollbar;
    GameObject[] pediaCards;

    GameDataManager gameDataManager;

    private void OnEnable()
    {
        gameDataManager = GameDataManager.GetInstance;
        gameDataManager.GetCardData();
        gameDataManager.GetUserAndLevelData();
        EncyclopediaInit();
    }

    void Update()
    {
        ScrollWheel();
    }

    private void EncyclopediaInit()
    {
        pediaCards = GameDataManager.GetInstance.GetMainCardEncyclopedia("000000");

        maxHeight = 0.5f + (float) 0.5 * (pediaCards.Length / 4);

        for (int i = 0; i < pediaCards.Length; i++)
        {
            var cardObject = (GameObject)Instantiate(pediaCards[i], new Vector3(0, 0, 0), Quaternion.identity);
            cardObject.transform.parent = GameObject.Find("LayoutCards").transform;
            cardObject.transform.localPosition =
                new Vector3(-0.9f + (0.6f * (i % 4)),
                -0.7f * (int) (i / 4), 0);
            cardObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        }
    }

    public void SyncScrollBar()
    {
        layoutGroup.transform.localPosition =
            new Vector3(0f, scrollbar.value * maxHeight, 0f);
    }

    private void ScrollWheel()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            // 휠을 밀어 돌렸을 때의 처리 ↑
            layoutGroup.transform.localPosition -= new Vector3(0, wheelSpeed, 0);
            scrollbar.value = layoutGroup.transform.localPosition.y / maxHeight;
            if (layoutGroup.transform.localPosition.y <= 0f)
            {
                layoutGroup.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        else if (wheelInput < 0)
        {
            // 휠을 당겨 올렸을 때의 처리 ↓
            layoutGroup.transform.localPosition += new Vector3(0, wheelSpeed, 0);
            scrollbar.value = layoutGroup.transform.localPosition.y / maxHeight;
            if (layoutGroup.transform.localPosition.y >= maxHeight)
            {
                layoutGroup.transform.localPosition = new Vector3(0, maxHeight, 0);
            }
        }
    }
}
