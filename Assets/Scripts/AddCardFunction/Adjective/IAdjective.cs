using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdjective
{
    public Adjective GetName(); 
    public int GetCount();
    public void SetCount(int addCount);
    
    public void Execute(ObjectClass thisObject);
    public void Execute(ObjectClass thisObject, GameObject player);
    public void Execute(ObjectClass thisObject, ObjectClass otherObject);
}
