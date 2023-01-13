using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushed : AdjectiveHS
{
    public void Function(IngameObject thisObject)
    {
        throw new System.NotImplementedException();
    }

    public void Function(IngameObject thisObject, PlayerInteraction player, bool isAffect)
    {
        thisObject.Pushed(player);
    }

    public void Function(IngameObject thisObject, IngameObject otherObject, bool isAffect)
    {
        throw new System.NotImplementedException();
    }

    public int GetCount()
    {
        throw new System.NotImplementedException();
    }

    public string GetName()
    {
        return this.GetType().ToString();
    }

    public void SetCount(int addCount)
    {
        throw new System.NotImplementedException();
    }
}
