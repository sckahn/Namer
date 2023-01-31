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

    private int mapX;
    private int mapY;
    private int mapZ;
    
    private GameObject[] tilePrefabs;
    private GameObject[] objectPrefabs;
    
    public GameObject[,,] CreateTileMap(string filePath, string tileMapFileName)
    {
        this.filePath = filePath;
        tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/GroundTiles");
        string[,,] tileMap = MapCsvFileReader(tileMapFileName);

        return TileCreator(tileMap);
    }

    public GameObject[,,] CreateObjectMap(string filePath, string objectMapFileName, string objectInfoFileName)
    {
        this.filePath = filePath;
        objectPrefabs = Resources.LoadAll<GameObject>("Prefabs/Objects");
        string[,,] objectMap = MapCsvFileReader(objectMapFileName);
        Dictionary<int, ObjectInfo> objectInfoDic = ObjectInfoFileReader(objectInfoFileName);
        
        return ObjectCreater(objectMap, objectInfoDic);
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
        
        mapX = lineGetSize[0];
        mapY = lineGetSize[1];
        mapZ = lineGetSize[2];
        string[,,] map = new string[mapX, mapY, mapZ];

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
    
    private Dictionary<int, ObjectInfo> ObjectInfoFileReader(string fileName)
    {
        Dictionary<int, ObjectInfo> objectInfoDic = new Dictionary<int, ObjectInfo>();
        
        if (!File.Exists(filePath + "/JSON/" + fileName))
        {
            Debug.Log( fileName + " 파일이 없습니다! 원하는 씬으로 가서 맵 정보 파일을 먼저 생성해주세요.");
            return null;
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
        
        return objectInfoDic;
    }

    private GameObject[,,] TileCreator(string[,,] tileMap)
    {
        GameObject parent = new GameObject("Grounds");
        
        GameObject[,,] initTiles = new GameObject[mapX, mapY, mapZ];
        for (int y = 0; y < mapY; y++)
        {
            GameObject Layer = new GameObject(y + "F");
            Layer.transform.parent = parent.transform;
            
            for (int z = 0; z < mapZ; z++)
            {
                for (int x = 0; x < mapX; x++)
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

    private GameObject[,,] ObjectCreater(string[,,] objectMap, Dictionary<int, ObjectInfo> objectInfoDic)
    {
        GameObject parent = new GameObject("Objects");
        
        GameObject[,,] initObjects = new GameObject[mapX, mapY, mapZ];
        for (int y = 0; y < mapY; y++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                for (int x = 0; x < mapX; x++)
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
                            initObjects[x, y, z].GetComponent<InteractiveObject>().objectInfo = objectInfoDic[id];
                            break;
                        }
                    }
                }
            }
        }
        return initObjects;
    }
}
