using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardSpawnPoint;

    void AddCard()
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
    }

}

public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}
