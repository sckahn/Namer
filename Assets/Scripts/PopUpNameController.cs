using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PopUpNameController : MonoBehaviour
{
    [SerializeField] Text nameText;
    Transform cardHolder;
    InteractiveObject interactiveObject;

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
        nameText.text = interactiveObject.GetCurrentName();
        gameObject.transform.rotation = cardHolder.rotation;
    }
}
