using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CardType
{
    Word,
    Specificity
}

public enum Specificity
{
    Heavy,
    Light,
    Push,
    Pull,
    Burst,
    Fly,
    Long
}

public class ObjectManager : MonoBehaviour
{
    private string selectCard;
    [SerializeField]private GameObject target;
    private void OnEnable()
    {
        string[] targetName = target.name.Split().ToArray();
        target.AddComponent<ObjectClass>();
        target.GetComponent<ObjectClass>().SetObject(targetName[0]);
    }
}
