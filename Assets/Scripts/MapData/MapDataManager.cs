using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapDataManager : Singleton<MapDataManager>
{
    // Test
    [SerializeField] private bool isTest = false;
    [SerializeField] private bool shouldCreateFile;
    [SerializeField] private bool shouldCreateMap;
    [SerializeField] private string loadLevel;
    //
    private string filePath;
    private string tileMapFileName;
    private string objectMapFileName;
    private string objectInfoFileName;

    private GameObject[,,] initTiles;
    public GameObject[,,] InitTiles { get { return initTiles; } set { initTiles = value; } }
    
    private GameObject[,,] initObjects;
    public GameObject[,,] InitObjects { get { return initObjects; } set { initObjects = value; } }

    private void OnEnable()
    {
        if (!isTest) return;

        if ((shouldCreateFile && shouldCreateMap) || (!shouldCreateFile && !shouldCreateMap))
        {
            Debug.Log("하나만 선택 가능합니다");
            return;
        }
        
        if (shouldCreateFile)
        {
            CreateFile();
        }
        
        if (shouldCreateMap)
        {
            CreateMap(loadLevel);
        }
    }

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

        FileCreator fileCreator = this.AddComponent<FileCreator>();
        fileCreator.CreateFile(filePath + SceneManager.GetActiveScene().name, tileMapFileName, objectMapFileName, objectInfoFileName);
    }

    public void CreateMap(string levelName)
    {
        SettingManager();

        if (levelName == null || levelName == "")
        {
            Debug.Log("Load Level를 입력해주세요");
            return;
        }

        MapCreator mapCreator = this.AddComponent<MapCreator>();

        initTiles = mapCreator.CreateTileMap(filePath + levelName, tileMapFileName);
        initObjects = mapCreator.CreateObjectMap(filePath + levelName, objectMapFileName, objectInfoFileName);
    }
}
