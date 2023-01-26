using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private GameObject groundTile;
    [SerializeField] private GameObject grassTile;
    
    private const int MAP_LENGTH = 20;
    private string[,,] tileMap;
    private string[,,] objectMap;

    private GameObject[] prefabs;
    
    private void OnEnable()
    {
        prefabs = Resources.LoadAll<GameObject>("Prefabs/Objects");
        
        tileMap = FileReader("mapTileData.csv");
        objectMap = FileReader("objectData.csv");
        
        TileCreator();
        ObjectCreater();
    }

    private string[,,] FileReader(string fileName)
    {
        string[,,] map = new string[MAP_LENGTH,MAP_LENGTH,MAP_LENGTH];
        StreamReader sr = new StreamReader("Assets/Scripts/MapData/CsvData/TutorialScene/" + fileName);

        int y = 0;
        int z = 0;

        while (true)
        {
            string line = sr.ReadLine();
            
            if (line==null)
            {
                //Debug.Log("Escape/현지님 의견 : 탈출이라고 적어라");
                break;
            }

            if (line.Contains("Layer"))
            {
                y = line[line.Length - 1] - '0';
                z = 0;
                //Debug.Log(y);
            }
            else
            {
                //데이터 배열에 한줄씩 저장
                string[] data = line.Split(',').ToArray();
                
                for (int x = 0; x < data.Length -1; x++)
                {
                    map[x, y, z] = data[x];
                    //Debug.Log(x + ", " + y + ", " + z + " : " + tileMap[x, y, z]);
                }
                z++;
            }
        }

        return map;
    }

    private void TileCreator()
    {
        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                for (int z = 0; z < tileMap.GetLength(2); z++)
                {
                    switch (tileMap[x,y,z])
                    {
                        case "00" :
                            Instantiate(groundTile, new Vector3(x, y, z), Quaternion.identity);
                            break;
                        case "01" :
                            Instantiate(grassTile, new Vector3(x, y, z), Quaternion.identity);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void ObjectCreater()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JsonData/objectInfo");
        ObjectInfoList objectInfoList = JsonUtility.FromJson<ObjectInfoList>(textAsset.text);

        Dictionary<int, ObjectInfo> objectDic = new Dictionary<int, ObjectInfo>();
        foreach (var objectInfo in objectInfoList.ObjectInfos)
        {
            objectDic.Add(objectInfo.objectID, objectInfo);
        }

        for (int x = 0; x < objectMap.GetLength(0); x++)
        {
            for (int y = 0; y < objectMap.GetLength(1); y++)
            {
                for (int z = 0; z < objectMap.GetLength(2); z++)
                {
                    if (objectMap[x, y, z] == null)
                    {
                        continue;
                    }
                    
                    int id = int.Parse(objectMap[x, y, z]);
                    if (id >= 0)
                    {
                        for (int i = 0; i < prefabs.Length; i++)
                        {
                            if (prefabs[i].name == objectDic[id].prefabName)
                            {
                                // Debug.Log(x + ", " + y + ", " + z + ", " + " : " + id + " : " + prefabs[i].name);
                                Instantiate(prefabs[i], new Vector3(x, y, z), Quaternion.identity);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    
}
