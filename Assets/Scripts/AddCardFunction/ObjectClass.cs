using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectClass : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private bool[] checkSpec = new bool[10];
    [SerializeField] private int[] countSpec = new int[10];
    
    private List<NameClass> words = new List<NameClass>();
    private IAdjective[] specificities = new IAdjective[10];
    public IAdjective[] Specificities {get{return  specificities;}}

    private void OnEnable()
    {
        //Debug.Log(objectName);
        SetObject(objectName);
    }

    public void SetObject(string objectName)
    {
        Type type = Type.GetType(objectName);
        var word = Activator.CreateInstance(type) as NameClass;
        
        words.Add(word);

        foreach (var specificity in words[words.Count - 1].Specificities)
        {
            SetInit(specificity);
        }
    }

    public void SetSpecificity(string name)
    {
        Type type = Type.GetType(name + "Adj");
        var specificity = Activator.CreateInstance(type) as IAdjective;
        
        SetInit(specificity);
    }
    
    private void SetInit(IAdjective adjective)
    {
        int specIdx = (int)Enum.Parse(typeof(Adjective), adjective.GetName());
        specificities[specIdx] = adjective;
        checkSpec[specIdx] = true;

        adjective.Function(this);
    }
    
    public void Interact(GameObject go)
    {
        if (go.tag == "Player")
        {
            // isAffect -> Set to ISpecificity
            foreach (var specificity in specificities)
            {
                if (specificity != null)
                {
                    specificity.Function(this, go, false);
                }
            }
        }

        else if (go.tag == "Object")
        {
            // isAffect -> Set to ISpecificity
            foreach (var specificity in specificities)
            {
                if (specificity != null)
                {
                    specificity.Function(this, go.GetComponent<ObjectClass>(), false);
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
    private void OnMouseOver()
    {
        if (this.gameObject.CompareTag("InteractObj") && CardManager.GetInstance.isPickCard)
        {
            CardManager.GetInstance.target = this.gameObject;
        Debug.Log(this.gameObject.tag);
        }
        Debug.Log(CardManager.GetInstance.target);
    }

    //마우스가 떠나면 카드의 타겟은 다시 널로 설정
    private void OnMouseExit()
    {
        CardManager.GetInstance.target = null;
        Debug.Log("Exit");
    }
}
