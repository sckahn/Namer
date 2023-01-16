using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card Word", menuName = "Scriptable Object/Card Word", order = 1)]
public class CardWord : ScriptableObject
{
    [SerializeField] private eCardType cardType;
    public eCardType CardType { get { return cardType; } }
    [SerializeField] private string uiWordText;
    public string UIWordText { get { return uiWordText; } }
    [SerializeField] private string addWordClass;
    public string AddWordClass{ get { return addWordClass; } }
}
