using System.Linq;
using Unity.Collections;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [ReadOnly][SerializeField] private EName objectName;
    [Range(0, 2)][SerializeField] private int[] countAdj = new int[20];
    [SerializeField] GameObject popUpName;

    // Todo 후에 체크하는 로직이 다른 곳으로 이동하면, 수정 예정
    Vector3 objectPos;

    // Todo 테스트 완료 후, Test 관련 코드 삭제 예정
    #region [Test] Change Count To Inspector
    
        private int[] initCountAdj = new int[20];
        private bool isCard = false;

    #endregion

    // object's name = adjective card's ui texts + name card's ui text
    private string addNameText;
    private string[] addAdjectiveTexts;
    private int[] countNameAdj;
    private int addAdjectiveCount;
    
    // Max Value of Adjective Count
    private int maxAdjCount = 2;

    // added adjective functions(interface)
    private IAdjective[] adjectives;
    public IAdjective[] Adjectives { get { return  adjectives; } }
    
    // object's information
    public SObjectInfo objectInfo;
    public int GetObjectID()
    {
        return objectInfo.objectID;
    }
    public EName GetObjectName()
    {
        return objectName;
    }
    
    // manager to get card data 
    private GameDataManager gameData;
    
    private void OnEnable()
    {
        if (!gameObject.CompareTag("InteractObj"))
        {
            Debug.LogError("태그를 InteractObj로 설정해주세요!");
        }
        
        gameData = GameDataManager.GetInstance;
        addAdjectiveTexts = new string[gameData.Adjectives.Count];
        countNameAdj = new int[gameData.Adjectives.Count];
        adjectives = new IAdjective[gameData.Adjectives.Count];
    }

    private void Start()
    {
        // Delete IF After Finish Test
        if (!FindObjectOfType<MapReader>())
        {
            objectName = objectInfo.nameType;
            addNameText = gameData.Names[objectName].uiText;

            SetAdjectiveFromData(gameData.Names[objectName].adjectives, false);
            SetAdjectiveFromData(objectInfo.adjectives);

            // Test
            countAdj.CopyTo(initCountAdj, 0);
            //
        }
    }

    private void SetAdjectiveFromData(EAdjective[] addedAdjectives, bool isAdjective = true)
    {
        if (addedAdjectives == null || addedAdjectives.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < addedAdjectives.Length; i++)
        {
            int adjIndex = (int)addedAdjectives[i];
            if (isAdjective && countAdj[adjIndex] - countNameAdj[adjIndex] >= maxAdjCount)
            {
                Debug.LogError("같은 꾸밈 성질은 2개만 부여할 수 있어요");
            }
            
            SetAdjective(addedAdjectives[i], isAdjective);
        }
    }

#region Run When Inspector Data Change & Test
    private void Update()
    {
        // todo 후에 배열 포지션 체크하는 로직이 이동하면, 수정 예정
        if (GameManager.GetInstance.CurrentState != GameStates.InGame) return;
        if (objectPos != Vector3Int.RoundToInt(this.gameObject.transform.position))
        {
            DetectManager.GetInstance.CheckValueInMap(this.gameObject);
            objectPos = Vector3Int.RoundToInt(this.gameObject.transform.position);

            // 물체가 중력에 의해 떨어진 후 디텍팅 하는 로직, 급하게 짜느라 문제가 있을 수 있음
            DetectManager.GetInstance.StartDetector(new[] { this.gameObject }.ToList());
        }

        AllPopUpNameCtr();
        AdjectiveTest();
    }

    private void AdjectiveTest()
    {
        if (!isCard && countAdj.Sum() != initCountAdj.Sum())
        {
            for (int i = 0; i < countAdj.Length; i++)
            {
                if (countAdj[i] < initCountAdj[i])
                {
                    int diff = initCountAdj[i] - countAdj[i];
                    while (diff > 0)
                    {
                        TestSubtractAdjective((EAdjective)i);
                        diff--;
                    }
            
                    initCountAdj[i] = countAdj[i];
                }
                else if (countAdj[i] > initCountAdj[i])
                {
                    int diff = countAdj[i] - initCountAdj[i];
                    while (diff > 0)
                    {
                        TestSetAdjective((EAdjective)i);
                        diff--;
                    }
            
                    initCountAdj[i] = countAdj[i];
                }
            }
        }
    }
    
    private void TestSetAdjective(EAdjective addAdjective)
    {
        int adjIndex = (int)addAdjective;
        SAdjectiveInfo adjectiveInfo = gameData.Adjectives[addAdjective];
        if (adjectives[adjIndex] == null)
        {
            adjectives[adjIndex] = adjectiveInfo.adjective.DeepCopy();
        }

        adjectives[adjIndex].SetCount(1);
        addAdjectiveTexts[adjIndex] = adjectiveInfo.uiText;
        
        // Todo 다른 곳으로 이동해야하는 IAdjective 함수?
        switch (adjectives[adjIndex].GetAdjectiveType())
        {
            case (EAdjectiveType.Normal): // normal
            case (EAdjectiveType.Repeat): // repeat
                adjectives[adjIndex].Execute(this);
                break;
            case (EAdjectiveType.Contradict): // contradict
                break;
        }
        //
    }
    
    private void TestSubtractAdjective(EAdjective subtractAdjective)
    {
        int adjIndex = (int)subtractAdjective;
        if (adjectives[adjIndex] == null)
        { 
            return;
        }
        
        // Todo 다른 곳으로 이동해야하는 IAdjective 함수?
        adjectives[adjIndex].Abandon(this);
        //
        
        if (countAdj[adjIndex] == 0)
        {
            adjectives[adjIndex] = null;
        }
        else
        {
            adjectives[adjIndex].SetCount(-1);
        }
        
        if (countAdj[adjIndex] - countNameAdj[adjIndex] == 0)
        {
            addAdjectiveTexts[adjIndex] = null;
        }
    }

#endregion

    public void AddName(EName? addedName)
    {
        // Test
        isCard = true;
        //
        
        SubtractName(objectName);
        
        // Check Error
        if (addedName == null)
        {
            Debug.LogError("Card의 Name 정보를 확인해주세요!");
            return;
        }
        
        EAdjective[] addAdjectives = gameData.Names[(EName)addedName].adjectives;
        if (addAdjectives != null)
        {
            foreach (EAdjective addAdjective in addAdjectives)
            {
                SetAdjective(addAdjective, false);
            }
        }
        
        objectName = (EName)addedName;
        addNameText = gameData.Names[(EName)addedName].uiText;
    }

    public void AddAdjective(EAdjective addAdjective)
    {
        // Test
        isCard = true;
        //
        
        if (addAdjective == EAdjective.Null)
        {
            Debug.Log("Null 꾸밈 성질 카드가 맞으시죠? 확인 부탁해요!");
        }
        
        SetAdjective(addAdjective);
    }

    private void SetAdjective(EAdjective addAdjective, bool isAdjective = true)
    {
        int adjIndex = (int)addAdjective;

        SAdjectiveInfo adjectiveInfo = gameData.Adjectives[addAdjective];
        if (adjectives[adjIndex] == null)
        {
            adjectives[adjIndex] = adjectiveInfo.adjective.DeepCopy();
        }

        if (isAdjective)
        {
            addAdjectiveTexts[adjIndex] = adjectiveInfo.uiText;
        }
        else
        {
            ++countNameAdj[adjIndex];
        }
        
        adjectives[adjIndex].SetCount(1);
        ++countAdj[adjIndex];
        // Test
        ++initCountAdj[adjIndex];
        //
        
        // Todo 다른 곳으로 이동해야하는 IAdjective 함수?
        switch (adjectives[adjIndex].GetAdjectiveType())
        {
            case (EAdjectiveType.Normal): // normal
            case (EAdjectiveType.Repeat): // repeat
                adjectives[adjIndex].Execute(this);
                break;
            case (EAdjectiveType.Contradict): // contradict
                break;
        }
        //
        
        // Test
        isCard = false;
        //
    }

    private void SubtractName(EName subtractName)
    {
        EAdjective[] subtractAdjectives = gameData.Names[subtractName].adjectives;

        if (subtractAdjectives != null)
        {
            foreach (var adjective in subtractAdjectives)
            {
                SubtractAdjective(adjective, false);
            }
        }
    }

    public void SubtractAdjective(EAdjective subtractAdjective, bool isAdjective = true)
    {
        int adjIndex = (int)subtractAdjective;
        if (adjectives[adjIndex] == null)
        { 
            return;
        }
        
        // Todo 다른 곳으로 이동해야하는 IAdjective 함수?
        adjectives[adjIndex].Abandon(this);
        //
        
        --countAdj[adjIndex];
        // Test
        --initCountAdj[adjIndex];
        //
        if (countAdj[adjIndex] <= 0)
        {
            adjectives[adjIndex] = null;
        }
        else
        {
            adjectives[adjIndex].SetCount(-1);
        }
        
        if (isAdjective)
        {
            if (countAdj[adjIndex] - countNameAdj[adjIndex] == 0)
            {
                addAdjectiveTexts[adjIndex] = null;
            }
        }
        else
        {
            --countNameAdj[adjIndex];
        }
        
        // Test
        isCard = false;
        //
    }
    
    public bool CheckAdjective(EAdjective checkAdjective)
    {
        if (adjectives[(int)checkAdjective] != null)
        {
            return true;
        }

        return false;
    }
    
    public bool CheckCountAdjective(EAdjective checkAdjective)
    {
        if (countAdj[(int)checkAdjective] >= 2)
        {
            return false;
        }

        return true;
    }

#region Method for UI
    
    // 오브젝트 이름을 리턴하는 함수
    public string GetCurrentName()
    {
        string currentObjectName = null;
        addAdjectiveCount = 0;
        
        for (int i = 0; i < gameData.Adjectives.Count; i++)
        {
            if (addAdjectiveTexts[i] != null)
            {
                for (int j = 0; j < countAdj[i] - countNameAdj[i]; j++)
                {
                    currentObjectName += addAdjectiveTexts[i] + " ";
                    addAdjectiveCount++;
                }
            }
        }
        currentObjectName += addNameText;
        
        return currentObjectName;
    }

    public int GetAddAdjectiveCount()
    {
        return addAdjectiveCount;
    }

    //카드를 선택한 상태에서 오브젝트를 호버링하면 카드의 타겟으로 설정
    //오브젝트의 이름을 화면에 띄움
    bool isHoverling = false;
    private void OnMouseOver()
    {
        if (GameManager.GetInstance.CurrentState == GameStates.Pause) return;
        if (GameManager.GetInstance.CurrentState == GameStates.Victory &&
            this.name != "PlanetObj") return;
        isHoverling = true;
        if (this.gameObject.CompareTag("InteractObj") && CardManager.GetInstance.isPickCard)
        {
            CardManager.GetInstance.target = this.gameObject;
            
            if (!CheckCountAdjective(CardManager.GetInstance.pickCard.GetComponent<CardController>().GetAdjectiveTypeOfCard()))
            {
                CardManager.GetInstance.ableAddCard = false;
                return;
            }
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
        if (GameManager.GetInstance.CurrentState == GameStates.Pause) return;
        isHoverling = false;
        CardManager.GetInstance.target = null;
        popUpName.SetActive(false);
        if (this.gameObject.CompareTag("InteractObj"))
        {
            PopUpNameOff();
            CardManager.GetInstance.ableAddCard = true;
        }
    }

    //오브젝트 현재 이름 팝업을 띄움 
    void PopUpNameOn()
    {
        if (GameManager.GetInstance.CurrentState == GameStates.Encyclopedia)
            return;
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
         if (UIManager.GetInstance.isShowNameKeyPressed && !popUpName.activeSelf)
         {
             PopUpNameOn();
         }
         if (!UIManager.GetInstance.isShowNameKeyPressed && popUpName.activeSelf && !isHoverling)
         {
             PopUpNameOff();
         }
     }
#endregion

}
