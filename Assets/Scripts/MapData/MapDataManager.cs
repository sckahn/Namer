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

    private void OnEnable()
    {
        filePath = "Assets/Resources/Data/";
        tileMapFileName = "tileMapData.csv";
        objectMapFileName = "objectMapData.csv";
        objectInfoFileName = "objectInfoData.json";

        if ((shouldCreateFile && shouldCreateMap) || (!shouldCreateFile && !shouldCreateMap))
        {
            Debug.Log("하나만 선택 가능합니다");
            return;
        }
        
        if (shouldCreateFile)
        {
            fileCreator = this.AddComponent<FileCreator>();
            fileCreator.CreateFile(filePath + SceneManager.GetActiveScene().name, tileMapFileName, objectMapFileName, objectInfoFileName);
        }
        
        if (shouldCreateMap)
        {
            if (loadLevel == null || loadLevel == "")
            {
                Debug.Log("Load Level를 입력해주세요");
                return;
            }
            
            mapCreator = this.AddComponent<MapCreator>();
            
            mapCreator.CreateTileMap(filePath + loadLevel, tileMapFileName);
            initObjects = mapCreator.CreateObjectMap(filePath + loadLevel, objectMapFileName, objectInfoFileName);
        }
    }
}
