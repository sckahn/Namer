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
        target = pi.forwardObjectInfo;
    }

    public void AddCard()
    {
        if (target)
        {
            if (cardWord.CardType == eCardType.Name)
            {
                target.GetComponent<ObjectClass>().AddName(cardWord.AddWordClass);
            }
            else if (cardWord.CardType == eCardType.Adjective)
            {
                target.GetComponent<ObjectClass>().AddAdjective(cardWord.AddWordClass);
            }
        }

    }

}
