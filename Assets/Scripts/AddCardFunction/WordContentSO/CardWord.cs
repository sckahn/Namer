using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Word Data", menuName = "Scriptable Object/Word Data", order = 1)]
public class CardWord : ScriptableObject
{
    [SerializeField] private string wordText;
    public string WordText {get {return wordText;}}
    [SerializeField] private string specFucn;
    public string SpecFunc{get {return specFucn;}}
    [SerializeField] private CardType cardType;
    public CardType CardType { get {return cardType;}}
}
