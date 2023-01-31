using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Adjectives
{
    Flammable = -1,
    Win,
    Flame,
    Movable,
    Climbable,
    Long,
    Float,
    Bouncy,
    Light,
    Obtainable,
}

[System.Serializable]
public struct BlockProperty
{
    [SerializeField] Adjectives adjective;
    [SerializeField] int count;
}
