using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameCanvasController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject encyclopedia;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject pediaBtn;
    [SerializeField] GameObject optionBtn;
    [SerializeField] GameObject gameOptionPanel;
    [SerializeField] GameObject topPanel;
    [SerializeField] Text stageName;
    Canvas canvas;

    bool isCardVisible = true;
    #region ResetRelatedVal
    [SerializeField] GameObject LoadingImg;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        canvas = this.gameObject.GetComponent<Canvas>();
        canvas.worldCamera =
            Camera.main.transform.Find("UICamera").gameObject.GetComponent<Camera>();
        StageNameSetUp();
        encyclopedia = Camera.main.gameObject.transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        CardsToggle();
    }

    private void CardsToggle()
    {
        if (Input.GetKeyDown(GameManager.GetInstance.cardToggleKey) && CardManager.GetInstance.isCardDealingDone)
        {
            if (isCardVisible)
            {
                CardManager.GetInstance.CardsDown();
                isCardVisible = false;
            }
            else
            {
                CardManager.GetInstance.CardsUp();
                isCardVisible = true;
            }
        }
    }

    public void EncyclopediaOpen()
    {
        GameManager.GetInstance.ChangeGameState(GameStates.Encyclopedia);
        GameManager.GetInstance.isPlayerCanInput = false;
        encyclopedia.SetActive(true);
        buttons.SetActive(false);
        topPanel.SetActive(false);
        CardManager.GetInstance.CardsHide();
    }

    public void EncyclopediaClose()
    {
        GameManager.GetInstance.isPlayerCanInput = true;
        encyclopedia.SetActive(false);
        buttons.SetActive(true);
        CardManager.GetInstance.CardsReveal();
        topPanel.SetActive(true);
        GameManager.GetInstance.ChangeGameState(GameStates.InGame);
    }

    public void OptionBtn()
    {
        UIManager.GetInstance.UIOn();
    }

    public void StartBtn()
    {
        UIManager.GetInstance.UIOff();
    }

    public void RestartBtn()
    {
        UIManager.GetInstance.UIOff();
        GameManager.GetInstance.ResetCurrentLvl();
    }

    public void ReturnLobby()
    {
        UIManager.GetInstance.UIOff();
        GameManager.GetInstance.ChangeGameState(GameStates.LevelSelect);
        SceneManager.LoadScene("MainScene");
    }

    public void GameOff()
    {
        Application.Quit();
    }

    public void GameOptionPanelOn()
    {
        gameOptionPanel.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (GameObject obj in eventData.hovered)
        {
            if (obj.name == "Dialog")
            {
                OnPointerExit(eventData);
                return;
            }
        }

        GameManager.GetInstance.scenarioController.isUI = true;
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.GetInstance.scenarioController.isUI = false;
    }

    public void SetLoadingImage(float fillValue)
    {
        LoadingImg.GetComponent<Image>().fillAmount = fillValue;
    }

    public void TurnOnAndOffLoadingImg(bool switchTurn)
    {
        LoadingImg.SetActive(switchTurn);
    }

    void StageNameSetUp()
    {
        string currentName =
            GameDataManager.GetInstance.GetLevelName(GameManager.GetInstance.Level);

        if (currentName == "")
        {
            stageName.text = $"{(GameManager.GetInstance.Level) + 1}" + " Stage";
        }
        else if (currentName != "")
        {
            stageName.text = currentName;
        }
    }

}