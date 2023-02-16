using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class PopUpNameController : MonoBehaviour
{
    [SerializeField] Text nameText;
    Transform cardHolder;
    InteractiveObject interactiveObject;
    int currentAdjCount;

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
        var scene = SceneManager.GetActiveScene();
        if (scene.name == "MainScene") return;
        nameText.text = interactiveObject.GetCurrentName();

        currentAdjCount = interactiveObject.GetAddAdjectiveCount();

        if(currentAdjCount == 1)
        {
            nameText.gameObject.transform.localScale = new Vector3(0.0095f, 0.01f, 0.01f);
        }

        if(currentAdjCount == 2)
        {
            nameText.gameObject.transform.localScale = new Vector3(0.0065f, 0.01f, 0.01f);
        }
        else
        {
            nameText.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
}
