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

    void Update()
    {
        ScrollWheel();
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
