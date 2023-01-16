using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpNameController : MonoBehaviour
{
    [SerializeField] string currentObjectName = "돌";
    [SerializeField] Text nameText;
    Transform cardHolder;

    private void OnEnable()
    {
        cardHolder = Camera.main.transform;
    }

    void Update()
    {
        setUpPopUpName();
    }

    private void setUpPopUpName()
    {
        nameText.text = currentObjectName;
        gameObject.transform.rotation = cardHolder.rotation;
    }
}
