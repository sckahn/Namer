using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    private int mapX;
    private int mapY;
    private int mapZ;
    
    private GameObject[] tilePrefabs;
    private GameObject[] objectPrefabs;
    
    public GameObject[,,] CreateTileMap(StreamReader tileMapData)
    {
        tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/GroundTiles");
        string[,,] tileMap = ReadMapDataCsv(tileMapData);

        return TileCreator(tileMap);
    }

    public GameObject[,,] CreateObjectMap(StreamReader objectMapData, Dictionary<int, ObjectInfo> objectInfoDic)
    {
        objectPrefabs = Resources.LoadAll<GameObject>("Prefabs/Objects");
        string[,,] objectMap = ReadMapDataCsv(objectMapData);

        return ObjectCreator(objectMap, objectInfoDic);
    }

    private string[,,] ReadMapDataCsv(StreamReader sr)
    {
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

    private GameObject[,,] ObjectCreator(string[,,] objectMap, Dictionary<int, ObjectInfo> objectInfoDic)
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
