using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card/create Card", order = 0)]
public class NamerCard : ScriptableObject
{
    public string name;
    public bool isAdjective = false;
    public Adjective[] adjectivesOrders;
}
