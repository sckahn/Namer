using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapDataManager : Singleton<MapDataManager>
{
    [SerializeField] private bool shouldCreateFile;
    [SerializeField] private bool shouldCreateMap;
    [SerializeField] private string loadLevel;
    
    private string filePath;
    private string tileMapFileName;
    private string objectMapFileName;
    private string objectInfoFileName;
    
    private FileCreator fileCreator;
    private MapCreator mapCreator;

    private GameObject[,,] initObjects;
    public GameObject[,,] InitObjects { get { return initObjects; } set { initObjects = value; } }
    private GameObject[,,] initTiles;
    public GameObject[,,] InitTiles { get { return initTiles; } set { initTiles = value; } }

    //private void OnEnable()
    //{
    //    filePath = "Assets/Resources/Data/";
    //    tileMapFileName = "tileMapData.csv";
    //    objectMapFileName = "objectMapData.csv";
    //    objectInfoFileName = "objectInfoData.json";

    //    if ((shouldCreateFile && shouldCreateMap) || (!shouldCreateFile && !shouldCreateMap))
    //    {
    //        Debug.Log("하나만 선택 가능합니다");
    //        return;
    //    }
        
    //    if (shouldCreateFile)
    //    {
    //        fileCreator = this.AddComponent<FileCreator>();
    //        fileCreator.CreateFile(filePath + SceneManager.GetActiveScene().name, tileMapFileName, objectMapFileName, objectInfoFileName);
    //    }
        
    //    if (shouldCreateMap)
    //    {
    //        if (loadLevel == null || loadLevel == "")
    //        {
    //            Debug.Log("Load Level를 입력해주세요");
    //            return;
    //        }
            
    //        mapCreator = this.AddComponent<MapCreator>();
            
    //        initTiles = mapCreator.CreateTileMap(filePath + loadLevel, tileMapFileName);
    //        initObjects = mapCreator.CreateObjectMap(filePath + loadLevel, objectMapFileName, objectInfoFileName);
    //    }
    //}

    private void SettingManager()
    {
        filePath = "Assets/Resources/Data/";
        tileMapFileName = "tileMapData.csv";
        objectMapFileName = "objectMapData.csv";
        objectInfoFileName = "objectInfoData.json";
    }

    public void CreateFile()
    {
        SettingManager();

        fileCreator = this.AddComponent<FileCreator>();
        fileCreator.CreateFile(filePath + SceneManager.GetActiveScene().name, tileMapFileName, objectMapFileName, objectInfoFileName);
    }

    public void CreateMap(string levelName)
    {
        SettingManager();

        if (levelName == null || levelName == "")
        {
            Debug.Log("LevelName을 입력해주세요");
            return;
        }

        mapCreator = this.AddComponent<MapCreator>();

        initTiles = mapCreator.CreateTileMap(filePath + levelName, tileMapFileName);
        initObjects = mapCreator.CreateObjectMap(filePath + levelName, objectMapFileName, objectInfoFileName);
    }
}
