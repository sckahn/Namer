using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngameObject : InteractObject
{
    [SerializeField] public string name;
    [SerializeField] private List<Adjective> adjectiveOrders = new List<Adjective>();
    [SerializeField] private SortedList<int, AdjectiveHS> adjectives = new SortedList<int, AdjectiveHS>();

    public void SetAdjectives(Adjective[] orders)
    {
        foreach (Adjective order in orders)
        {
            Type adjType = System.Type.GetType(order.ToString());
            var adj = Activator.CreateInstance(adjType, this) as AdjectiveHS;
            adjectives.TryAdd((int)order, adj);

            adjectiveOrders.Add(order);
        }

        GetAdjectives();
    }

    public void GetAdjectives()
    {
        foreach (AdjectiveHS adj in adjectives.Values) Debug.Log(adj.GetName());
    }

    public void InteractByPlayer(PlayerInteraction pi)
    {
        foreach (AdjectiveHS adj in adjectives.Values)
        {
            adj.Function(this, pi, false);
        }
    }

    [ContextMenu("TryPushed")]
    public void TryPushed()
    {
        this.Pushed(new PlayerInteraction());
    }
}
