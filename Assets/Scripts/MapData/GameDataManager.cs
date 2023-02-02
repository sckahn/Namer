using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : Singleton<GameDataManager>
{
    // file information
    private string filePath;
    private string tileMapFileName;
    private string objectMapFileName;
    private string objectInfoFileName;
    private string userDataFileName;
    private string levelDataFileName;
    
    // map information - 3 dimensional space array
    private GameObject[,,] initTiles;
    public GameObject[,,] InitTiles { get { return initTiles; } set { initTiles = value; } }
    
    private GameObject[,,] initObjects;
    public GameObject[,,] InitObjects { get { return initObjects; } set { initObjects = value; } }

    // user information dictionary
    private Dictionary<string, UserData> userDataDic = new Dictionary<string, UserData>();
    public Dictionary<string, UserData> UserDataDic { get { return userDataDic; } }
    
    // level information dictionary
    private Dictionary<int, LevelData> levelDataDic = new Dictionary<int, LevelData>();
    public Dictionary<int, LevelData> LevelDataDic { get { return levelDataDic; } }
    
    
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
        MapData mapData = mapReader.GetMapData();
        
        SaveLoadData saveData = new SaveLoadData();
        saveData.CreateCsvFile(mapData.tileMapData, filePath + SceneManager.GetActiveScene().name, tileMapFileName);
        saveData.CreateCsvFile(mapData.objectMapData, filePath + SceneManager.GetActiveScene().name, objectMapFileName);
        saveData.CreateJsonFile(mapData.objectInfoData, filePath + SceneManager.GetActiveScene().name, objectInfoFileName);
    }

    public void CreateMap(string levelName)
    {
        FilePathInfo();
        
        if (levelName == null || levelName == "")
        {
            Debug.Log("Load Level를 입력해주세요");
            return;
        }
        
        SaveLoadData loadData = new SaveLoadData();
        StreamReader tileMapData = loadData.ReadCsvFile(filePath + levelName, tileMapFileName);
        StreamReader objectMapData = loadData.ReadCsvFile(filePath + levelName, objectMapFileName);
        Dictionary<int, ObjectInfo> objectInfoDic = loadData.ReadJsonFile<int, ObjectInfo>(filePath + levelName, objectInfoFileName);

        MapCreator mapCreator = gameObject.AddComponent<MapCreator>();
        initTiles = mapCreator.CreateTileMap(tileMapData);
        initObjects = mapCreator.CreateObjectMap(objectMapData, objectInfoDic);
    }
    
#endregion

#region User And Level Data

    public void GetUserAndLevelData()
    {
        FilePathInfo();
        
        SaveLoadData loadData = new SaveLoadData();
        userDataDic = loadData.ReadJsonFile<string, UserData>(filePath + "SaveLoad", userDataFileName);
        levelDataDic = loadData.ReadJsonFile<int, LevelData>(filePath + "SaveLoad", levelDataFileName);
    }
    
#endregion

    
}