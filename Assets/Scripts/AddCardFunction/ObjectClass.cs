using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class ObjectClass : MonoBehaviour
{
    [SerializeField] private bool setInit;
    [SerializeField] private Name objectName;
    [SerializeField] private bool[] checkAdj = new bool[10];
    // [SerializeField] private int[] countAdj = new int[10];

    private Name initName;
    private Name checkName;
    
    private int checkAdjCount;

    private NameClass[] names = new NameClass[2];
    // names[0] : object name
    // names[1] : stored name by adding name card
    private IAdjective[] adjectives = new IAdjective[10];
    public IAdjective[] Adjectives { get { return  adjectives; } }

    private void OnEnable()
    {
        initName = objectName;
        checkName = objectName;
        checkAdjCount = checkAdj.Count(a => a);
        
        Init();
    }

    private void Init()
    {
        objectName = initName;
        // this.gameObject.transform.localScale = new Vector3(1, 1, 1);

        if (names[0]== null)
        {
            AddName(initName.ToString());
        }
        else
        {
            ChangeName(initName.ToString());
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
            ChangeName(objectName.ToString());
        }
        
        // change adjective
        int currentCheckAdj = checkAdj.Count(a => a);
        if (currentCheckAdj != checkAdjCount)
        {
            ChangeAdjective();
        }
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

    private void ChangeName(string name)
    {
        // delete name & adjectives of names[0] 
        foreach (var adjective in names[0].Adjectives)
        {
            int adjIdx = (int)Enum.Parse(typeof(eAdjective), adjective.GetName());
            checkAdj[adjIdx] = false;
            adjectives[adjIdx] = null;
        }
        
        names[0] = null;
        checkName = objectName;
        
        AddName(name);
    }
    
    private void ChangeAdjective()
    {
        bool subtractAdj = false;
        
        for (int i = 0; i < checkAdj.Length; i++)
        {
            if (checkAdj[i] && adjectives[i] == null)
            {
                string name = Enum.GetName(typeof(eAdjective), i);
                AddAdjective(name);
            }
            else if (!checkAdj[i] && adjectives[i] != null)
            {
                this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                adjectives[i] = null;
                subtractAdj = true;
            }
        }

        if (subtractAdj)
        {
            for (int i = 0; i < checkAdj.Length; i++)
            {
                if (checkAdj[i])
                {
                    string name = Enum.GetName(typeof(eAdjective), i);
                    AddAdjective(name);
                }
            }
        }
    }

    #endregion

    public void AddName(string objectName)
    {
        if (objectName == null) return;
        
        Type type = Type.GetType(objectName);
        var word = Activator.CreateInstance(type) as NameClass;
        
        int nameIndex = 0;
        if (names[0] == null)
        {
            names[0] = word;
        }
        else
        {
            names[1] = word;
            nameIndex = 1;
        }

        foreach (var adjective in names[nameIndex].Adjectives)
        {
            SetAdjective(adjective);
        }
    }

    public void AddAdjective(string name)
    {
        Type type = Type.GetType(name + "Adj");
        var adjective = Activator.CreateInstance(type) as IAdjective;
        
        SetAdjective(adjective);
    }
    
    private void SetAdjective(IAdjective adjective)
    {
        int adjIdx = (int)Enum.Parse(typeof(eAdjective), adjective.GetName());
        adjectives[adjIdx] = adjective;
        checkAdj[adjIdx] = true;

        adjective.Execute(this);
    }
    
    public void Interact(GameObject go)
    {
        if (go.tag == "Player")
        {
            foreach (var adjective in adjectives)
            {
                if (adjective != null)
                {
                    adjective.Execute(this, go, false);
                }
            }
        }

        else if (go.tag == "Object")
        {
            foreach (var specificity in adjectives)
            {
                if (specificity != null)
                {
                    specificity.Execute(this, go.GetComponent<ObjectClass>(), false);
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
