using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamingCardController : MonoBehaviour
{
    [SerializeField] Text inputedText;
    [SerializeField] CardController cardController;

    private void OnEnable()
    {
        inputedText.text = cardController.currentLevelName;   
    }
}
