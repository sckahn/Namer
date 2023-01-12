using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [SerializeField] private CardWord cardWord;
    [SerializeField] private GameObject target;

    public void AddCard()
    {
        if (cardWord.CardType == CardType.Word)
        {
            target.GetComponent<ObjectClass>().SetObject(cardWord.AbjFunc);
        }
        else if (cardWord.CardType == CardType.Specificity)
        {
            target.GetComponent<ObjectClass>().SetSpecificity(cardWord.AbjFunc);
        }
    }

}
