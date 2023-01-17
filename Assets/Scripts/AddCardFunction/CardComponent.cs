using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerInteraction pi;
 
    public void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<PlayerInteraction>(out pi);
    }

    private void Update()
    {
        target = pi.forwardObjectInfo;
    }

    public void AddCard()
    {
        if (target)
        {
            if (cardData.cardType == CardType.Name)
            {
                target.GetComponent<InteractiveObject>().AddName(cardData.addedName, cardData.uiText);
            }
            else if (cardData.cardType == CardType.Adjective)
            {
                target.GetComponent<InteractiveObject>().AddAdjective(cardData.addedAdjectives, cardData.uiText);
            }
        }
    
    }

}
