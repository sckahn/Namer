using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [SerializeField] private CardWord cardWord;
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerInteraction pi;
 
    public void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<PlayerInteraction>(out pi);
    }

    private void Update()
    {
        if (CardManager.GetInstance.target != null)
        {
            target = CardManager.GetInstance.target;
        }
    }

    public void AddCard()
    {
        if (target)
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

}
