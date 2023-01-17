using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private bool setInit;
    [SerializeField] private Name objectName;
    [SerializeField] private bool[] checkAdj = new bool[20];
    [SerializeField] private int[] countAdj = new int[20];

    private Vector3 currentPosition;
    private Material currentMaterial;
    
    private string currentObjectName = "???";
    private string addNameText;
    private string[] addAdjectiveTexts = {"승리", "잘타는", "활활", "밀림", "오름", "길쭉", "둥둥", "통통", "반짝", "줍줍"};
    
    /// Test Field
    private Name initName;
    private Name checkName;
    private int checkAdjCount;
    ///
    
    private IAdjective[] adjectives = new IAdjective[20];
    public IAdjective[] Adjectives { get { return  adjectives; } }
    
    private NameData nameData;

    private void OnEnable()
    {
        currentPosition = gameObject.transform.position;
        currentMaterial = gameObject.GetComponentInChildren<MeshRenderer>().material;
        
        nameData = FindObjectOfType<NameData>();
        if (nameData.NameInfos.Length == 0)
        {
            Debug.Log("NameData의 정보를 확인해주세요!");
        }
        
        if (!gameObject.CompareTag("InteractObj"))
        {
            Debug.Log("태그를 InteractObj로 설정해주세요!");
        }
        
        initName = objectName;
        checkName = objectName;
        checkAdjCount = checkAdj.Count(a => a);
        addNameText = currentObjectName;
        
        Init();
    }

    private void Init()
    {
        objectName = initName;
        
        if (objectName == checkName)
        {
            AddName(objectName);
        }
        else
        {
            ChangeName(objectName);
        }
    }

#region Run When Inspector Data Change & Test
    private void FixedUpdate()
    {
        // set init
        if (setInit)
        {
            Init();
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
        
        Debug.Log(GetCurrentName());
    }

    // Use When data type of objectName is string
    private string CheckObjectName(string objectName)
    {
        foreach (var name in Enum.GetValues(typeof(Name)))
        {
            if (objectName.Contains(name.ToString()))
            {
                return name.ToString();
            }
        }

        return null;
    }

    private void ChangeName(Name changeName)
    {
        foreach (var adjective in nameData.NameInfos[(int)checkName].adjectives)
        {
            int adjIdx = (int)adjective;
            adjectives[adjIdx] = null;
            checkAdj[adjIdx] = false;
            countAdj[adjIdx]--;
        }
        
        checkName = changeName;
        addNameText = nameData.NameInfos[(int)changeName].uiText;
        
        AddName(changeName);
    }
    
    private void ChangeAdjective()
    {
        for (int i = 0; i < checkAdj.Length; i++)
        {
            if (!checkAdj[i] && adjectives[i] != null)
            {
                if ((int)Adjective.Long == i)
                {
                    this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
                
                adjectives[i] = null;
                --countAdj[i];
            }
        }
        
        for (int i = 0; i < checkAdj.Length; i++)
        {
            if (checkAdj[i] && adjectives[i] == null)
            {
                Adjective adjective = (Adjective)i;
                SetAdjective(adjective);
            }
        }
    }

#endregion
    
    public string GetCurrentName()
    {
        currentObjectName = null;
        int currentCheckAdj = checkAdj.Count(a => a);

        Adjective[] adjectives = nameData.NameInfos[(int)objectName].adjectives;
        if (currentCheckAdj != 0)
        {
            for (int i = addAdjectiveTexts.Length - 1; i >= 0; i--)
            {
                if (checkAdj[i])
                {
                    bool check = false;
                    foreach (var adjective in adjectives)
                    {
                        if (i == (int)adjective)
                        {
                            check = true;
                            break;
                        }
                    }

                    if (!check)
                    {
                        currentObjectName += addAdjectiveTexts[i] + " ";
                    }
                }
            }
        }
        currentObjectName += addNameText;
        
        return currentObjectName;
    }

    public void AddName(Name? addedName, string uiText = null)
    {
        // Check Error
        if (addedName == null || nameData.NameInfos[(int)addedName].name != addedName)
        {
            Debug.Log("Card 또는 NameData의 정보를 확인해주세요!");
            return;
        }

        foreach (var addAdjective in nameData.NameInfos[(int)addedName].adjectives)
        {
            SetAdjective(addAdjective);
        }
        
        if (uiText != null)
        {
            addNameText = uiText;
        }
    }

    public void AddAdjective(Adjective[] addAdjectives, string uiText = null)
    {
        if (addAdjectives == null)
        {
            Debug.Log("Card의 정보를 채워주세요!");
            return;
        }

        foreach (var addAdjective in addAdjectives)
        {
            SetAdjective(addAdjective);
        }
        
        if (uiText != null)
        {
            addAdjectiveTexts[(int)addAdjectives[0]] = uiText;
        }
    }
    
    private void SetAdjective(Adjective addAdjective)
    {
        int adjIdx = (int)addAdjective;
        if (adjectives[adjIdx] == null)
        {
            Type type = Type.GetType(addAdjective + "Adj");
            var adjective = Activator.CreateInstance(type) as IAdjective;
            if (adjective == null)
            {
                Debug.Log(addAdjective + " Adjective 인터페이스의 이름를 확인해주세요!");
                return;
            }
            
            adjectives[adjIdx] = adjective;
            checkAdj[adjIdx] = true;
        }
        
        adjectives[adjIdx].Execute(this);
        ++countAdj[adjIdx];
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
}
