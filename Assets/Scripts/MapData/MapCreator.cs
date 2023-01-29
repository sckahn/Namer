using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    private string filePath;
    
    private string[,,] tileMap;
    private string[,,] objectMap;

    private int tileX;
    private int tileY;
    private int tileZ;
    
    private int objectX;
    private int objectY;
    private int objectZ;
    
    private GameObject[] tilePrefabs;
    private GameObject[] objectPrefabs;

    private Dictionary<int, ObjectInfo> objectInfoDic = new Dictionary<int, ObjectInfo>();

    public GameObject[,,] CreateTileMap(string filePath, string tileMapFileName)
    {
        this.filePath = filePath;
        tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/GroundTiles");
        tileMap = MapCsvFileReader(tileMapFileName);

        return TileCreator();
    }

    public GameObject[,,] CreateObjectMap(string filePath, string objectMapFileName, string objectInfoFileName)
    {
        this.filePath = filePath;
        objectPrefabs = Resources.LoadAll<GameObject>("Prefabs/Objects");
        objectMap = MapCsvFileReader(objectMapFileName);
        ObjectInfoFileReader(objectInfoFileName);
        
        return ObjectCreater();
    }

    private string[,,] MapCsvFileReader(string fileName)
    {
        if (!File.Exists(filePath + "/CSV/" + fileName))
        {
            Debug.Log(fileName + " 파일이 없습니다! 원하는 씬으로 가서 맵 정보 파일을 먼저 생성해주세요.");
            return null;
        }
        
        StreamReader sr = new StreamReader(filePath + "/CSV/" + fileName);
        
        int[] lineGetSize = Array.ConvertAll(sr.ReadLine().Split(','), int.Parse);
        string[,,] map = null;
        if (fileName.Contains("tile"))
        {
            tileX = lineGetSize[0];
            tileY = lineGetSize[1];
            tileZ = lineGetSize[2];
            
            map = new string[tileX, tileY, tileZ];
        }
        else if (fileName.Contains("object"))
        {
            objectX = lineGetSize[0];
            objectY = lineGetSize[1];
            objectZ = lineGetSize[2];
            
            map = new string[objectX, objectY, objectZ];
        }

        int y = 0, z = 0;
        while (true)
        {
            string line = sr.ReadLine();
            
            if (line == null)
            {
                break;
            }

            if (line.Contains("Layer"))
            {
                y = line[line.Length - 1] - '0';
                z = 0;
            }
            else
            {
                //데이터 배열에 한줄씩 저장
                string[] data = line.Split(',').ToArray();
                
                for (int x = 0; x < data.Length; x++)
                {
                    map[x, y, z] = data[x];
                }
                z++;
            }
        }
        return map;
    }
    
    private void ObjectInfoFileReader(string fileName)
    {
        if (!File.Exists(filePath + "/JSON/" + fileName))
        {
            Debug.Log( fileName + " 파일이 없습니다! 원하는 씬으로 가서 맵 정보 파일을 먼저 생성해주세요.");
            return;
        }
        
        string jsonFilePath = filePath.Replace("Assets/Resources/", "");
        fileName = fileName.Replace(".json", "");
        
        TextAsset textAsset = Resources.Load<TextAsset>(jsonFilePath + "/JSON/" + fileName);
        List<ObjectInfo> objectInfos = JsonConvert.DeserializeObject<List<ObjectInfo>>(textAsset.text);
        
        foreach (var info in objectInfos)
        {
            if (!objectInfoDic.ContainsKey(info.objectID))
            {
                objectInfoDic.Add(info.objectID, info);
            }
        }
    }

    private GameObject[,,] TileCreator()
    {
        GameObject parent = new GameObject("Grounds");


        GameObject[,,] initTiles = new GameObject[tileX, tileY, tileZ];
        for (int y = 0; y < tileY; y++)
        {
            GameObject Layer = new GameObject(y + "F");
            Layer.transform.parent = parent.transform;
            
            for (int z = 0; z < tileZ; z++)
            {
                for (int x = 0; x < tileX; x++)
                {
                    for (int i = 0; i < tilePrefabs.Length; i++)
                    {
                        if (tilePrefabs[i].name == tileMap[x, y, z])
                        {
                            initTiles[x, y, z] = Instantiate(tilePrefabs[i], new Vector3(x, y, z), Quaternion.identity, Layer.transform);
                            break;
                        }
                    }
                }
            }

            if (Layer.transform.childCount == 0)
            {
                Destroy(Layer);
            }
        }
        return initTiles;
    }

    private GameObject[,,] ObjectCreater()
    {
        GameObject parent = new GameObject("Objects");
        
        GameObject[,,] initObjects = new GameObject[objectX, objectY, objectZ];
        for (int y = 0; y < objectY; y++)
        {
            for (int z = 0; z < objectZ; z++)
            {
                for (int x = 0; x < objectX; x++)
                {
                    if (objectMap[x, y, z] == "-1")
                    {
                        continue;
                    }
                    
                    int id = int.Parse(objectMap[x, y, z]);
                    for (int i = 0; i < objectPrefabs.Length; i++)
                    {
                        if (objectPrefabs[i].name == objectInfoDic[id].prefabName)
                        {
                            initObjects[x, y, z] = Instantiate(objectPrefabs[i], new Vector3(x, y, z), Quaternion.identity, parent.transform);
                            break;
                        }
                    }
                }
            }
        }
        return initObjects;
    }
}
