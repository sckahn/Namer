using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

#region Card Information Struct

public struct SNameInfo
{
    public readonly int priority;
    public readonly string uiText;
    public readonly EName name;
    public readonly EAdjective[] adjectives;
    public readonly string cardPrefabName;
    public readonly string contentText;

    public SNameInfo(int priority, string uiText, EName name, EAdjective[] adjectives, string cardPrefabName, string contentText = "")
    {
        this.priority = priority;
        this.uiText = uiText;
        this.name = name;
        this.adjectives = adjectives;
        this.cardPrefabName = cardPrefabName;
        this.contentText = contentText;
    }
}

public struct SAdjectiveInfo
{
    public readonly int priority;
    public readonly string uiText;
    public readonly EAdjective adjectiveName;
    public readonly IAdjective adjective;
    public readonly string cardPrefabName;
    public readonly string contentText;

    public SAdjectiveInfo(int priority, string uiText, EAdjective adjectiveName, string cardPrefabName, string contentText = "")
    {
        this.priority = priority;
        this.uiText = uiText;
        this.adjectiveName = adjectiveName;
        
        Type type = Type.GetType(adjectiveName + "Adj");
        var adjFunc = Activator.CreateInstance(type) as IAdjective;
        this.adjective = adjFunc;
        
        this.cardPrefabName = cardPrefabName;
        this.contentText = contentText;
    }
}

#endregion

#region Object Information Struct

public struct SObjectInfo
{
    public string prefabName;
    public int objectID;
    public EName nameType;
    public EAdjective[] adjectives;
}

#endregion

#region Game Information(Map, User, Level) Struct

public struct SMapData
{
    public readonly StringBuilder tileMapData;
    public readonly StringBuilder objectMapData;
    public readonly List<SObjectInfo> objectInfoData;
    
    public SMapData(StringBuilder tileMapData, StringBuilder objectMapData, List<SObjectInfo> objectInfoData)
    {
        this.tileMapData = tileMapData;
        this.objectMapData = objectMapData;
        this.objectInfoData = objectInfoData;
    }
}

public struct SUserData
{
    public string userID;
    public string nickName;
    public int clearLevel;
    public List<SLevelName> levelNames;
    public SCardView cardView;
    public SGameSetting gameSetting;
}

public struct SLevelData
{
    public int level;
    public string sceneName;
    public Scenario[] scenario;
    public SPosition playerPosition;
    public SCardView cardView;
}

public struct SLevelName
{
    public int level;
    public string levelName;

    public SLevelName(int level, string levelName)
    {
        this.level = level;
        this.levelName = levelName;
    }
}

public struct SCardView
{
    public List<EName> nameRead;
    public List<EAdjective> adjectiveRead;

    public SCardView(List<EName> nameRead, List<EAdjective> adjectiveRead)
    {
        this.nameRead = nameRead;
        this.adjectiveRead = adjectiveRead;
    }
}

public struct SGameSetting
{
    public int resolution;
    public int maxFrame;
    public float volume;
    public float backgroundVolume;
    public float soundEffects;
    public bool isfullScreen;
    public bool isborderlessFullScreen;
    public bool isMute;
    public bool isMuteInBackground;

    public SGameSetting(int resolution, int maxFrame, float volume, float backgroundVolume, float soundEffects,
        bool isfullScreen, bool isborderlessFullScreen, bool isMute, bool isMuteInBackground)
    {
        this.resolution = resolution;
        this.maxFrame = maxFrame;
        this.volume = volume;
        this.backgroundVolume = backgroundVolume;
        this.soundEffects = soundEffects;
        this.isfullScreen = isfullScreen;
        this.isborderlessFullScreen = isborderlessFullScreen;
        this.isMute = isMute;
        this.isMuteInBackground = isMuteInBackground;
    }
}

[Serializable]
public struct SPosition
{
    public float x;
    public float y;
    public float z;

    public SPosition(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }
}

#endregion

public class DataInfo
{
    
}