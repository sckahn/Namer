using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

[Serializable]
public class ObjectInfo : MonoBehaviour
{
    public string prefabName;
    public int objectID;
    public string nameType;
    public string[] adjectives;
}

[Serializable]
public class ObjectInfoList
{
    public ObjectInfo[] ObjectInfos;
}

public class MapDataManager : Singleton<MapDataManager>
{
    [SerializeField] private Transform groundsParent;
    [SerializeField] private Transform objectsParent;
    
    private int minWidth;
    private int minHeight;
    private int width;
    private int height;
    
    private string[] tileNames = {"Ground", "Glass"};
    
    private Dictionary<int, string[,]> mapGroundData = new Dictionary<int, string[,]>();
    public Dictionary<int, string[,]> MapGroundData { get { return mapGroundData; } }
    
    private Dictionary<int, string[,]> mapObjectData = new Dictionary<int, string[,]>();
    public Dictionary<int, string[,]> MapObjectData { get { return mapObjectData; } }

    private ObjectInfoList objectsList = new ObjectInfoList();
    private List<ObjectInfo> objectInfos = new List<ObjectInfo>();

    private void Awake()
    {
        GetMapSize();
        mapGroundData = GetMapTiles();
        WriteCsvFile(mapGroundData, "mapTileData.csv");
    }

    private void Start()
    {
        mapObjectData = GetObjects();
        WriteCsvFile(mapObjectData, "objectData.csv");
    }

    private void GetMapSize()
    {
        Transform[] mapGrounds = groundsParent.GetComponentsInChildren<Transform>();
        
        minWidth = (int)mapGrounds.Min(a => a.transform.position.x) * -1;
        minHeight = (int)mapGrounds.Min(a => a.transform.position.z) * -1;
        width = (int)mapGrounds.Max(a => a.transform.position.x) + minWidth + 1;
        height = (int)mapGrounds.Max(a => a.transform.position.z) + minHeight + 1;
    }

    private Dictionary<int, string[,]> GetMapTiles()
    {
        Dictionary<int, string[,]> groundData = new Dictionary<int, string[,]>();

        for (int i = 0; i < groundsParent.childCount; i++)
        {
            Transform groundsLayer = groundsParent.GetChild(i);
            string[,] grounds = new string[height, width];
            
            for (int j = 0; j < groundsLayer.childCount; j++)
            {
                Transform ground = groundsLayer.GetChild(j);
                int x = (int)ground.position.x + minWidth;
                int z = (int)ground.position.z + minHeight;

                for (int k = 0; k < tileNames.Length; k++)
                {
                    if (ground.name.Contains(tileNames[k]))
                    {
                        grounds[z, x] = string.Format("{0:D2}", k);
                        // Debug.Log(y + ", " + x + " : " + groundsLayer[j].name + " : " + grounds[y, x]);
                        break;
                    }
                }
            }

            if (groundsLayer.GetChild(0) != null)
            {
                int y = (int)groundsLayer.GetChild(0).position.y;
                groundData.Add(y, grounds);
            }
        }
        
        return groundData;
    }

    private Dictionary<int, string[,]> GetObjects()
    {
        CardDataManager cardData = CardDataManager.GetInstance;
        
        // csv : 오브젝트 프리펩 타입(00), 적용된 Name(00), 적용된 Adjective(00 ...)
        Dictionary<int, string[,]> objectData = new Dictionary<int, string[,]>();

        int IDCount = 0;

        for (int i = 0; i < objectsParent.childCount; i++)
        {
            Transform objectTransform = objectsParent.GetChild(i);
            InteractiveObject interObj = objectTransform.GetComponent<InteractiveObject>();
            
            int x = (int)objectTransform.position.x + minWidth;
            int z = (int)objectTransform.position.z + minHeight;
            int y = (int)objectTransform.position.y;

            // string prefabInfo = "";
            // for (int j = 0; j < cardData.PriorityName.Length; j++)
            // {
            //     if (objectTransform.name.Contains(cardData.PriorityName[j]))
            //     {
            //         prefabInfo = string.Format("{0:D2}", j);
            //         break;
            //     }
            // }
            //
            // string nameIndex = interObj.GetObjectName().ToString();
            // string nameInfo = string.Format("{0:D2}", cardData.Names[nameIndex].priority);
            //

            // string adjectiveInfo = "";
            bool[] checkAdj = interObj.GetCheckAdj();
            string adjs = "";
            for (int j = 0; j < checkAdj.Length; j++)
            {
                if (checkAdj[j])
                {
                    // adjectiveInfo += string.Format("{0:D2}", j);
                    adjs += cardData.PriorityAdjective[j] + " ";
                }
            }

            if (!objectData.ContainsKey(y))
            {
                objectData.Add(y, new string[height, width]);
            }

            // objectData[y][z,x] = prefabInfo + nameInfo + adjectiveInfo;
            // Debug.Log(y + " , " + x + " , " + z + " : " + objectTransform.name + " : " + objectData[y][z,x]);

            objectData[y][z, x] = IDCount.ToString();
            
            // Json
            ObjectInfo objectInfo = new ObjectInfo();
            string[] prefabName = objectTransform.name.Split().ToArray();
            objectInfo.prefabName = prefabName[0];
            objectInfo.nameType = interObj.GetObjectName().ToString();
            objectInfo.objectID = IDCount++;
            objectInfo.adjectives = adjs.Split().ToArray();

            objectInfos.Add(objectInfo);
        }

        return objectData;
    }

    private void WriteCsvFile(Dictionary<int, string[,]> datas, string fileName)
    {
        string filePath = "Assets/Scripts/MapData/CsvData/" + SceneManager.GetActiveScene().name + "/";
        // if (File.Exists(filePath + fileName))
        // {
        //     File.Delete(filePath + fileName);
        // }
        
        string delimiter = ",";
        StringBuilder stringBuilder = new StringBuilder();
        
        for (int i = datas.Keys.Min(); i <= datas.Keys.Max(); i++)
        {
            if (!datas.ContainsKey(i))
            {
                continue;
            }
            
            stringBuilder.AppendLine("Layer" + i);
            string[,] layerData = datas[i];
            
            for (int j = 0; j < layerData.GetLength(0); j++)
            {
                for (int k = 0; k < layerData.GetLength(1); k++)
                {
                    if (layerData[j, k] == null)
                    {
                        layerData[j, k] = "-1";
                    }
                    stringBuilder.Append(layerData[j, k] + delimiter);
                }
                
                stringBuilder.AppendLine();
            }
        }
        
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        
        StreamWriter outStram = File.CreateText(filePath + fileName);
        outStram.Write(stringBuilder);
        outStram.Close();

        if (fileName == "objectData.csv")
        {
            objectsList.ObjectInfos = objectInfos.ToArray();
            string data = JsonUtility.ToJson(objectsList);
            File.WriteAllText(filePath + "/" + "objectInfo.json", data);
        }
    }
}
