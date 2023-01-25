using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool setInit;
    [SerializeField] private EName objectName;
    [SerializeField] private bool[] checkAdj = new bool[20];
    [SerializeField] private int[] countAdj = new int[20];
    [SerializeField] GameObject popUpName;

    private Vector3 currentPosition;
    
    private string currentObjectName;
    private string addNameText;
    private string[] addAdjectiveTexts = new string[20];
    
    /// Test Field
    private EName initName;
    private EName checkName;
    private int checkAdjCount;
    private bool[] initAdj;
    ///
    
    private IAdjective[] adjectives = new IAdjective[20];
    public IAdjective[] Adjectives { get { return  adjectives; } }
    private CardDataManager cardData;
    
    public EName GetObjectName()
    {
        return objectName;
    }

    public bool[] GetCheckAdj()
    {
        return checkAdj;
    }
    
    public UnityEvent Execute { get; set; }
    public UnityEvent InteractPlayer { get; set; }
    public UnityEvent InteractOtherObject { get; set; }


    #region doingRepeat
    private delegate void ExcuteRepeat(InteractiveObject io);
    ExcuteRepeat excuteRepeat;

    public void AddExcuteRepeat(IAdjective adj)
    {
        excuteRepeat += adj.Execute;
    }

    public void DeleteExcuteRepeat(IAdjective adj)
    {
        excuteRepeat -= adj.Execute;
    }
    #endregion

    private void OnEnable()
    {
        cardData = CardDataManager.GetInstance;

        if (!gameObject.CompareTag("InteractObj"))
        {
            Debug.Log("태그를 InteractObj로 설정해주세요!");
        }
        
        currentPosition = gameObject.transform.position;
        initName = objectName;
        initAdj = checkAdj;

        if (cardData != null)
        {
            if (cardData.Names.Count != 0)
            {
                currentObjectName = cardData.Names[objectName.ToString()].uiText;
                InitCard();
            }
        }
    }

    private void InitCard(bool setInit = false)
    {
        if (setInit)
        {
            SubtractName(objectName);
            objectName = initName;
        }
        
        for (int i = 0; i < initAdj.Length; i++)
        {
            if (initAdj[i])
            {
                EAdjective adjective = (EAdjective)Enum.Parse(typeof(EAdjective), cardData.PriorityAdjective[i]);
                SetAdjective(adjective);
            }
        }
        
        checkName = initName;
        checkAdjCount = checkAdj.Count(a => a);
        addNameText = cardData.Names[initName.ToString()].uiText;

        AddName(objectName);
    }

    #region Run When Inspector Data Change & Test
    private void Update()
    {
        AllPopUpNameCtr();
        if (cardData == null)
        {
            Debug.Log("start");
            cardData = CardDataManager.GetInstance;
            InitCard();
        }

        // set init
        if (setInit)
        {
            InitCard(setInit);
            setInit = false;
        }
        
        // change name
        if (objectName != checkName)
        {
            ChangeName(objectName);
        }
        
        // change adjective
        int currentCheckAdj = checkAdj.Count(a => a);
        if (currentCheckAdj != checkAdjCount)
        {
            ChangeAdjective();
        }

        // run excuteRepeat
        if (excuteRepeat != null)
            excuteRepeat.Invoke(this);
    }

    // Use When data type of objectName is string

    private void ChangeName(EName changeName)
    {
        SubtractName(checkName);
        
        checkName = changeName;
        addNameText = cardData.Names[changeName.ToString()].uiText;
        checkAdjCount = checkAdj.Count(a => a);
        
        AddName(changeName);
    }
    
    private void ChangeAdjective()
    {
        checkAdjCount = checkAdj.Count(a => a);
        
        for (int i = 0; i < cardData.Adjectives.Count; i++)
        {
            if (checkAdj[i] && adjectives[i] == null)
            {
                string adjIndex = cardData.PriorityAdjective[i];
                EAdjective adjective = cardData.Adjectives[adjIndex].adjectiveName;

                SetAdjective(adjective);
            }

            if (!checkAdj[i] && adjectives[i] != null)
            {
                string adjIndex = cardData.PriorityAdjective[i];
                EAdjective adjective = cardData.Adjectives[adjIndex].adjectiveName;
                SubtractAdjective(adjective);
            }
        }
    }

#endregion
    
    public string GetCurrentName()
    {
        if (cardData == null)
        {
            return currentObjectName;
        }
        
        currentObjectName = null;
        int currentCheckAdj = checkAdj.Count(a => a);
        
        if (currentCheckAdj != 0)
        {
            for (int i = 0; i < cardData.Adjectives.Count; i++)
            {
                if (addAdjectiveTexts[i] != null)
                {
                    currentObjectName += addAdjectiveTexts[i] + " ";
                }
            }
        }
        currentObjectName += addNameText;
        
        return currentObjectName;
    }

    public void AddName(Enum addedName)
    {
        // Check Error
        if (addedName == null)
        {
            Debug.Log("Card의 Name 정보를 확인해주세요!");
            return;
        }

        EAdjective[] addAdjectives = cardData.Names[addedName.ToString()].adjectives;
        if (addAdjectives != null)
        {
            foreach (EAdjective addAdjective in addAdjectives)
            {
                SetAdjective(addAdjective, false);
            }
        }

        objectName = (EName)addedName;
        addNameText = cardData.Names[addedName.ToString()].uiText;
    }

    public void AddAdjective(EAdjective[] addAdjectives)
    {
        if (addAdjectives == null)
        {
            Debug.Log("Card의 Adjective 정보를 채워주세요!");
            return;
        }
        
        foreach (var addAdjective in addAdjectives)
        {
            SetAdjective(addAdjective);
        }
    }
    
    private void SetAdjective(EAdjective addAdjective, bool isAdjective = true)
    {
        SAdjectiveInfo adjectiveInfo = cardData.Adjectives[addAdjective.ToString()];
        int adjIndex = adjectiveInfo.priority;

        if (adjectives[adjIndex] == null)
        {
            adjectives[adjIndex] = adjectiveInfo.adjective;
            checkAdj[adjIndex] = true;
        }

        if (isAdjective)
        {
            addAdjectiveTexts[adjIndex] = adjectiveInfo.uiText;
        }

        switch (adjectives[adjIndex].GetAdjectiveType())
        {
            case (EAdjectiveType.Normal): // normal
                adjectives[adjIndex].Execute(this);
                break;
            case (EAdjectiveType.Repeat): // repeat
                AddExcuteRepeat(adjectives[adjIndex]);
                break;
            case (EAdjectiveType.Contradict): // contradict
                break;
        }
       
        ++countAdj[adjIndex];
    }

    private void SubtractName(EName subtractName)
    {
        EAdjective[] subtractAdjectives = cardData.Names[subtractName.ToString()].adjectives;

        if (subtractAdjectives != null)
        {
            foreach (var adjective in subtractAdjectives)
            {
                SubtractAdjective(adjective, false);
            }
        }
    }

    private void SubtractAdjective(EAdjective subtractAdjective, bool isAdjective = true)
    {
        int adjIndex = cardData.Adjectives[subtractAdjective.ToString()].priority;
        adjectives[adjIndex] = null;
        --countAdj[adjIndex];
        checkAdj[adjIndex] = false;

        if (isAdjective)
        {
            addAdjectiveTexts[adjIndex] = null;
        }
        
        if (subtractAdjective == EAdjective.Long)
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public bool CheckAdj(IAdjective checkAdjective)
    {
        if (adjectives.Contains(checkAdjective))
        {
            return true;
        }

        return false;
    }

    public void Interact(GameObject go)
    {
        if (go.tag == "Player")
        {
            foreach (var adjective in adjectives)
            {
                if (adjective != null)
                {
                    adjective.Execute(this, go);
                }
            }
        }

        else if (go.tag == "Object")
        {
            foreach (var specificity in adjectives)
            {
                if (specificity != null)
                {
                    specificity.Execute(this, go.GetComponent<InteractiveObject>());
                }
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.transform.name);
        //Interact(collision.gameObject);
    }

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
