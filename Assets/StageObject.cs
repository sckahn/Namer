using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NewAdjective
{
    public int index;
    public bool value;
}

public class StageObject : MonoBehaviour
{
    private string name;
    public string Name
    {
        get { return name; }
        set { if (value == "") return; name = value; }
    }
    private bool[] specificity;

    public StageObject(string name, NewAdjective[] adjectives)
    {
        this.name = name;
        SetSpeficity(adjectives);
    }

    public void SetSpeficity(NewAdjective[] adjectives)
    {
        foreach (NewAdjective adjective in adjectives)
        {
            specificity[adjective.index] = adjective.value;
        }
    }

    //public void Interact(Player player)
    //{

    //}

    public void Interact(StageObject obj)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.CompareTag("Player"))
            //Interact(collision.transform.GetComponent<Player>());
        //else
            Interact(collision.transform.GetComponent<StageObject>());
    }
}