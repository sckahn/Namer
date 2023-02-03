using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public struct ObjectInfo
{
    public string prefabName;
    public int objectID;
    public EName nameType;
    public EAdjective[] adjectives;
}

public struct MapData
{
    public readonly StringBuilder tileMapData;
    public readonly StringBuilder objectMapData;
    public readonly List<ObjectInfo> objectInfoData;
    
    public MapData(StringBuilder tileMapData, StringBuilder objectMapData, List<ObjectInfo> objectInfoData)
    {
        this.tileMapData = tileMapData;
        this.objectMapData = objectMapData;
        this.objectInfoData = objectInfoData;
    }
}

public struct UserData
{
    public string userID;
    public string nickName;
    public int clearLevel;
    public List<string> levelNames;
    public List<int> levelScore;
    public CardView cardView;
}

public struct LevelData
{
    public int level;
    public string SceneName;
    public string scenario;
    public Position playerPosition;
    public CardView cardView;
}

public struct CardView
{
    public List<EName> nameRead;
    public List<EAdjective> adjectiveRead;
}

public struct Position
{
    public float x;
    public float y;
    public float z;

    public Position(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }
}

public class DataInfo
{
    
}