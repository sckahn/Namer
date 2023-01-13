using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AdjectiveHS
{
    public string GetName();
    public int GetCount();
    public void SetCount(int addCount);

    // affect to this.Object 
    public void Function(IngameObject thisObject);

    // isAffect == true : this.Object -> player
    // isAffect == false : this.Object <- player
    public void Function(IngameObject thisObject, PlayerInteraction player, bool isAffect);

    // isAffect == true : this.Object -> other.Object
    // isAffect == false : this.Object <- other.Object
    public void Function(IngameObject thisObject, IngameObject otherObject, bool isAffect);
}
