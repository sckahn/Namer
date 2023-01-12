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
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerInteraction pi;

    private void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<PlayerInteraction>(out pi);
    }

    private void Update()
    {
        target = pi.forwardObjectInfo;

        if (target)
        {
            string[] targetName = target.name.Split().ToArray();

            if (!target.TryGetComponent<ObjectClass>(out var targetComponent))
            {
                target.AddComponent<ObjectClass>();
            }

            else
            {
                targetComponent.SetObject(targetName[0]);
            }
        }
    }
}
