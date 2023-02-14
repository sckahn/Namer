using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetObjController : MonoBehaviour
{
    [SerializeField] Text nameText;
    Transform cardHolder;
    InteractiveObject interactiveObject;
    [SerializeField] CardController cardController;


    private void OnEnable()
    {
        cardHolder = Camera.main.transform;
        interactiveObject = GetComponentInParent<InteractiveObject>();
    }

    void Update()
    {
        setUpPopUpName();
    }

    private void setUpPopUpName()
    {
        gameObject.transform.rotation = cardHolder.rotation;
    }

    public void UpdateTxt()
    {
        nameText.text = cardController.currentLevelName;
    }
}
