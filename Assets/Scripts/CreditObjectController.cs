using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreditObjectController : MonoBehaviour
{
    [SerializeField] float movingSpeed = 60f;
    MainUIController mainUIController;

    private void Start()
    {
        mainUIController = FindObjectOfType<MainUIController>();    
    }

    void OnEnable()
    {
        this.transform.DOMove(new Vector3(0, 6f, 0) ,movingSpeed);
        Invoke("ReturnToMainMenu", movingSpeed + 5f);
    }


    void ReturnToMainMenu()
    {
        mainUIController.MainMenuScene();
    }

}
