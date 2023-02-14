using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamingPanelController : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text inputTxt;
    CardController cardController;

    private void Start()
    {
        cardController =
            Camera.main.gameObject.transform.
            Find("ClearRig").transform.
            Find("NamingRig").transform.
            Find("NamingCard").GetComponent<CardController>();
    }

    public void SaveString()
    {
        cardController.currentLevelName = inputTxt.text;
    }

    [ContextMenu("Victory")]
    public void VictoryState()
    {
        GameManager.GetInstance.ChangeGameState(GameStates.Victory);
    }
}
