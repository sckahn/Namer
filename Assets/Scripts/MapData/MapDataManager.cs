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
    [SerializeField] private string loadLevel = "";
    
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
        
        if (shouldCreateFile)
        {
            fileCreator = this.AddComponent<FileCreator>();
            fileCreator.CreateFile(filePath + SceneManager.GetActiveScene().name, tileMapFileName, objectMapFileName, objectInfoFileName);
        }
        
        if (shouldCreateMap)
        {
            mapCreator = this.AddComponent<MapCreator>();
            
            mapCreator.CreateTileMap(filePath + loadLevel, tileMapFileName);
            initObjects = mapCreator.CreateObjectMap(filePath + loadLevel, objectMapFileName, objectInfoFileName);
        }
    }
}
