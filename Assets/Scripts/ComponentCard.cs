using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComponentCard : MonoBehaviour
{
    public NamerCard namerCard;
    public IngameObject target;

    [ContextMenu("SetTarget")]    
    public void SetTarget()
    {
        if (!namerCard.isAdjective) target.name = namerCard.name;
        else target.name = namerCard.name + target.name;
        target.SetAdjectives(namerCard.adjectivesOrders);
    }
}
