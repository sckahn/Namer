using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : Singleton<GameDataManager>
{
    // map information - 3 dimensional space array
    private GameObject[,,] initTiles;
    public GameObject[,,] InitTiles { get { return initTiles; } set { initTiles = value; } }
    
    private GameObject[,,] initObjects;
    public GameObject[,,] InitObjects { get { return initObjects; } set { initObjects = value; } }

    // user information dictionary
    private Dictionary<string, SUserData> userDataDic = new Dictionary<string, SUserData>();
    public Dictionary<string, SUserData> UserDataDic { get { return userDataDic; } }
    
    // level information dictionary
    private Dictionary<int, SLevelData> levelDataDic = new Dictionary<int, SLevelData>();
    public Dictionary<int, SLevelData> LevelDataDic { get { return levelDataDic; } }
    
    // card information dictionary
    private Dictionary<EName, SNameInfo> names = new Dictionary<EName, SNameInfo>();
    public Dictionary<EName, SNameInfo> Names { get { return names; } }

    private Dictionary<EAdjective, SAdjectiveInfo> adjectives = new Dictionary<EAdjective, SAdjectiveInfo>();
    public Dictionary<EAdjective, SAdjectiveInfo> Adjectives { get { return adjectives; } }
    
    // file information
    private string filePath;
    private string tileMapFileName;
    private string objectMapFileName;
    private string objectInfoFileName;
    private string userDataFileName;
    private string levelDataFileName;
    
    private void FilePathInfo()
    {
        filePath = "Assets/Resources/Data/";
        
        tileMapFileName = "tileMapData.csv";
        objectMapFileName = "objectMapData.csv";
        objectInfoFileName = "objectInfoData.json";

        userDataFileName = "user.json";
        levelDataFileName = "levels.json";
    }

#region Map(tile, object) Data 

    public void CreateFile()
    {
        FilePathInfo();
        
        MapReader mapReader = gameObject.AddComponent<MapReader>();
        SMapData mapData = mapReader.GetMapData();
        
        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.CreateCsvFile(mapData.tileMapData, filePath + SceneManager.GetActiveScene().name, tileMapFileName);
        saveFile.CreateCsvFile(mapData.objectMapData, filePath + SceneManager.GetActiveScene().name, objectMapFileName);
        saveFile.CreateJsonFile(mapData.objectInfoData, filePath + SceneManager.GetActiveScene().name, objectInfoFileName);
    }

    public void CreateMap(string levelName)
    {
        FilePathInfo();
        
        if (levelName == null || levelName == "")
        {
            Debug.Log("Load Level를 입력해주세요");
            return;
        }
        
        SaveLoadFile loadFile = new SaveLoadFile();
        StreamReader tileMapData = loadFile.ReadCsvFile(filePath + levelName, tileMapFileName);
        StreamReader objectMapData = loadFile.ReadCsvFile(filePath + levelName, objectMapFileName);
        Dictionary<int, SObjectInfo> objectInfoDic = loadFile.ReadJsonFile<int, SObjectInfo>(filePath + levelName, objectInfoFileName);

        MapCreator mapCreator = gameObject.AddComponent<MapCreator>();
        initTiles = mapCreator.CreateTileMap(tileMapData);
        initObjects = mapCreator.CreateObjectMap(objectMapData, objectInfoDic);
    }
    
#endregion

#region User And Level Data

    public void GetUserAndLevelData()
    {
        FilePathInfo();
        
        SaveLoadFile loadFile = new SaveLoadFile();
        userDataDic = loadFile.ReadJsonFile<string, SUserData>(filePath + "SaveLoad", userDataFileName);
        levelDataDic = loadFile.ReadJsonFile<int, SLevelData>(filePath + "SaveLoad", levelDataFileName);
    }

    public void SetUserData(string userID, SUserData userData)
    {
        if (userDataDic.ContainsKey(userID))
        {
            userDataDic[userID] = userData;
        }
        else
        {
            userDataDic.Add(userID, userData);
        }
    }

    public void SetLevelData(int levelNumber, SLevelData levelData)
    {
        if (levelDataDic.ContainsKey(levelNumber))
        {
            levelDataDic[levelNumber] = levelData;
        }
        else
        {
            levelDataDic.Add(levelNumber, levelData);
        }
    }

#endregion

#region Card Data

    public void GetCardData()
    {
        SaveLoadFile loadFile = new SaveLoadFile();
        XmlNodeList data = loadFile.ReadXmlFile("Data/SaveLoad/XML/CardData", "root/worksheet");
        
        names = loadFile.GetCardData<EName, SNameInfo>(data, 0);
        adjectives = loadFile.GetCardData<EAdjective, SAdjectiveInfo>(data, 1);
    }

#endregion

    
}