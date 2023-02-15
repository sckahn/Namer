using UnityEngine;
using DG.Tweening;

public class CreditObjectController : MonoBehaviour
{
    [SerializeField] float movingSpeed = 60f;
    MainUIController mainUIController;

    private void Start()
    {ㄴㄴ
        mainUIController = FindObjectOfType<MainUIController>();    
    }

    void OnEnable()
    {
        this.transform.position = new Vector3(0, -10, 0);
        this.transform.DOMove(new Vector3(0, 6f, 0) ,movingSpeed);
    }

    private void Update()
    {
        ReturnToMainMenu();
    }


    void ReturnToMainMenu()
    {
        if(this.transform.position.y == 6f)
        {
            mainUIController.MainMenuScene();
        }
    }

}
