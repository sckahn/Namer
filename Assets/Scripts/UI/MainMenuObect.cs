using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuObect : MonoBehaviour
{
    [SerializeField] GameObject popUpName;

    //카드를 선택한 상태에서 오브젝트를 호버링하면 카드의 타겟으로 설정
    //오브젝트의 이름을 화면에 띄움
    bool isHoverling = false;
    private void OnMouseOver()
    {
        if (UIManager.GetInstance.isPause) return;
        isHoverling = true;
        if (this.gameObject.CompareTag("InteractObj") && CardManager.GetInstance.isPickCard)
        {
            CardManager.GetInstance.target = this.gameObject;
        }
        if (this.gameObject.CompareTag("InteractObj"))
        {
            PopUpNameOn();
        }
    }

    //마우스가 떠나면 카드의 타겟은 다시 null로 설정
    //오브젝트의 이름을 화면에서 가림 
    private void OnMouseExit()
    {
        if (UIManager.GetInstance.isPause) return;
        isHoverling = false;
        CardManager.GetInstance.target = null;
        popUpName.SetActive(false);
        if (this.gameObject.CompareTag("InteractObj"))
        {
            PopUpNameOff();
        }
    }

    //오브젝트 현재 이름 팝업을 띄움 
    void PopUpNameOn()
    {
        popUpName.SetActive(true);
    }

    //오브젝트 현재 이름 팝업을 지움 
    void PopUpNameOff()
    {
        popUpName.SetActive(false);
    }

    //탭키에 따라 모든 네임 팝업을 띄움
    private void AllPopUpNameCtr()
    {
        if (GameManager.GetInstance.isTapDown && !popUpName.activeSelf)
        {
            PopUpNameOn();
        }
        if (!GameManager.GetInstance.isTapDown && popUpName.activeSelf && !isHoverling)
        {
            PopUpNameOff();
        }
    }
}
