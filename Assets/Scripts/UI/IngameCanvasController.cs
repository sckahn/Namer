using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCanvasController : MonoBehaviour
{
    GameObject encyclopedia;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject pediaBtn;
    [SerializeField] GameObject optionBtn;
    [SerializeField] GameObject gameOptionPanel;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        encyclopedia = Camera.main.gameObject.transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EncyclopediaOpen()
    {
        GameManager.GetInstance.ChangeGameState(GameStates.Encyclopedia);
        GameManager.GetInstance.isPlayerCanInput = false;
        encyclopedia.SetActive(true);
        buttons.SetActive(false);
        CardManager.GetInstance.CardsHide();
    }

    public void EncyclopediaClose()
    {
        GameManager.GetInstance.isPlayerCanInput = true;
        encyclopedia.SetActive(false);
        buttons.SetActive(true);
        CardManager.GetInstance.CardsReveal();
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
        GameManager.GetInstance.Reset();
    }

    public void GameOptionPanelOn()
    {
        gameOptionPanel.SetActive(true);
    }
}