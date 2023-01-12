using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdjective
{
    public string GetName(); 
    public int GetCount();
    public void SetCount(int addCount);
    
    // affect to this.Object 
    public void Function(ObjectClass thisObject);

    // isAffect == true : this.Object -> player
    // isAffect == false : this.Object <- player
    public void Function(ObjectClass thisObject, GameObject player, bool isAffect);
    
    // isAffect == true : this.Object -> other.Object
    // isAffect == false : this.Object <- other.Object
    public void Function(ObjectClass thisObject, ObjectClass otherObject, bool isAffect);
}
