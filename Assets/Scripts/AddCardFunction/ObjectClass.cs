using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectClass : MonoBehaviour
{
    // private string name;
    private List<NameClass> words = new List<NameClass>();
    private IAdjective[] specificities = new IAdjective[10];
    public IAdjective[] Specificities {get{return  specificities;}}
    [SerializeField] private bool[] checkSpec = new bool[10];
    [SerializeField] private int[] countSpec = new int[10];

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
        int specIdx = (int)Enum.Parse(typeof(Specificity), adjective.GetName());
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
}
