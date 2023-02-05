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
    
    public GameObject[,,] CreateTileMap(StreamReader tileMapData)
    {
        string[,,] tileMap = ReadMapDataCsv(tileMapData);
        return TileCreator(tileMap);
    }

    public GameObject[,,] CreateObjectMap(StreamReader objectMapData, Dictionary<int, SObjectInfo> objectInfoDic)
    {
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
                    if (tileMap[x, y, z] == "-1")
                    {
                        continue;
                    }
                    
                    GameObject tilePrefab = Resources.Load("Prefabs/GroundTiles/" + tileMap[x, y, z]) as GameObject;
                    initTiles[x, y, z] = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity, Layer.transform);
                }
            }

            if (Layer.transform.childCount == 0)
            {
                Destroy(Layer);
            }
        }

        return initTiles;
    }

    private GameObject[,,] ObjectCreator(string[,,] objectMap, Dictionary<int, SObjectInfo> objectInfoDic)
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
                    
                    GameObject objectPrefabs = Resources.Load("Prefabs/Objects/" + objectInfoDic[id].prefabName) as GameObject;
                    initObjects[x, y, z] = Instantiate(objectPrefabs, new Vector3(x, y, z), Quaternion.identity, parent.transform);
                    initObjects[x, y, z].GetComponent<InteractiveObject>().objectInfo = objectInfoDic[id];
                }
            }
        }
        return initObjects;
    }
}
