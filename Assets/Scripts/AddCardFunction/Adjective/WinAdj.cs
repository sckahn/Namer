using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Win;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;
    
    public EAdjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public EAdjectiveType GetAdjectiveType()
    {
        return adjectiveType;
    }

    public int GetCount()
    {
        return count;
    }

    public void SetCount(int addCount)
    {
        this.count += addCount;
    }
    
    public void Execute(InteractiveObject thisObject)
    {
        //Debug.Log("this is Win");
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        CallWin();
        //Debug.Log("Win : this Object -> Player");
    }
    
    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Win : this Object -> other Object");
    }

    void CallWin()
    {
        GameManager.GetInstance.Win();
    }

    
}
