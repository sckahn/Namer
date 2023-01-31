using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ObjectInfo
{
    public string prefabName;
    public int objectID;
    public EName nameType;
    public EAdjective[] adjectives;
}
