using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNameController : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] GameObject namePlate;
    [SerializeField] int stageNum;

    private void OnEnable()
    {
        nameTxt.text = NameText();
        NamePlateOnOff();
    }

    string NameText()
    {
        if (GameDataManager.GetInstance.GetLevelName(stageNum) == "")
        {
            return "???";
        }
        else
        {
            return GameDataManager.GetInstance.GetLevelName(stageNum);
        }
    }

    void NamePlateOnOff()
    {
        if (stageNum <= GameDataManager.GetInstance.UserDataDic[GameManager.GetInstance.userId].clearLevel)
        {
            namePlate.SetActive(true);
        }
        else
        {
            namePlate.SetActive(false);
        }
 
       
    }

}
