using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCanvasController : MonoBehaviour
{
    GameObject encyclopedia;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject pediaBtn;
    [SerializeField] GameObject optionBtn;

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

        for (int i = 0; i < CardManager.GetInstance.myCards.Count; i++)
        {
            CardManager.GetInstance.myCards[i].gameObject.SetActive(false);
        }
    }

    public void EncyclopediaClose()
    {
        GameManager.GetInstance.isPlayerCanInput = true;
        encyclopedia.SetActive(false);
        buttons.SetActive(true);
        for (int i = 0; i < CardManager.GetInstance.myCards.Count; i++)
        {
            CardManager.GetInstance.myCards[i].gameObject.SetActive(true);
        }
        GameManager.GetInstance.ReturnPreviousState();
    }
}