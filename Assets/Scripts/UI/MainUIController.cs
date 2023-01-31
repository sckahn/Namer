using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    [SerializeField] GameObject pressAnyKeyTxt;
    [SerializeField] GameObject title;
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject informationTxt;
    [SerializeField] GameObject levelInformationTxt;

    [SerializeField] float titleMovingTime = 1f;
    [SerializeField] float cameraMovingTime = 1f;
    [SerializeField] float levelSelectMovingTime = 1.5f;
    bool isPressAnyKey;
    float currentTime;
    [SerializeField] float speed = 2f;
    [SerializeField] float length = 15f;

    private void Start()
    {
        StartCoroutine(PressAnyKeyFloat());
    }

    void Update()
    {
        PressAnyKey();
        informationTxtOnOff();

    }

    //안내 문구를 상황에 따라 키고 끕니다 
    void informationTxtOnOff()
    {
        if (CardManager.GetInstance.isPickCard && informationTxt.activeSelf == false)
        {
            informationTxt.SetActive(true);

        }
        else if (!CardManager.GetInstance.isPickCard && informationTxt.activeSelf == true)
        {
            informationTxt.SetActive(false);
        }
    }

    //타이틀 화면에서 아무키나 입력하
    void PressAnyKey()
    {
        if (!isPressAnyKey && Input.anyKeyDown)
        {
            isPressAnyKey = true;
            TitleMove(titleMovingTime);
            pressAnyKeyTxt.SetActive(false);
            CameraMoving(cameraMovingTime);
            Invoke("CardholderStart", 0.1f);

        }
    }

    void TitleMove(float titleMovingTime)
    {
        title.transform.DOMove(new Vector3(Screen.width/2f, Screen.height/1.161f, 0f), titleMovingTime);
        title.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), titleMovingTime);
    }

    void CameraMoving(float cameraMovingTime)
    {
        Camera.main.transform.DORotate(new Vector3(60f, 0f, 0f), cameraMovingTime);
    }

    void CardholderStart()
    {
        cardHolder.SetActive(true);
    }

    IEnumerator PressAnyKeyFloat()
    {
        Vector3 currentPos = pressAnyKeyTxt.transform.localPosition;
        while (true)
        {
            currentTime += Time.deltaTime * speed;
            pressAnyKeyTxt.transform.
                localPosition = new Vector3(pressAnyKeyTxt.transform.localPosition.x,
                currentPos.y + Mathf.Sin(currentTime) * length,
                pressAnyKeyTxt.transform.localPosition.z);
            yield return null;
        }
    }

    public void LevelSelectScene()
    {
        Camera.main.transform.DOMove(new Vector3(10f, 7f, -3.17f), levelSelectMovingTime);
        Camera.main.transform.DORotate(new Vector3(60f, 90f, 0f), levelSelectMovingTime);
        title.transform.DOMove(new Vector3(Screen.width / 6.09f, Screen.height / 1.161f, 0f), levelSelectMovingTime);

    }
}
