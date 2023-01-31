using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class FileCreator : MonoBehaviour
{
    private string filePath;

    private int gapY = 2;

    private int minX;
    private int maxX;
    private int minY;
    private int maxY;
    private int minZ;
    private int maxZ;
    
    private int totalX;
    private int totalY;
    private int totalZ;

    private string[,,] tileMapData;
    private string[,,] objectMapData;
    
    private List<ObjectInfo> objectInfos = new List<ObjectInfo>();
    
    public void CreateFile(string filePath, string tileMapFileName, string objectMapFileName, string objectInfoFileName)
    {
        this.filePath = filePath;

        GetMapSize();
        Indicator();
        
        WriteCsvFile(tileMapData, tileMapFileName);
        WriteCsvFile(objectMapData, objectMapFileName);
        WriteJsonFile(objectInfoFileName);
    }

    private void GetMapSize()
    {
        Transform[] groundChilds = GameObject.Find("Grounds").transform.GetComponentsInChildren<Transform>();
        Transform[] objectChilds = GameObject.Find("Objects").transform.GetComponentsInChildren<Transform>();

        minX = (int)groundChilds.Min(item => item.position.x);
        maxX = (int)groundChilds.Max(item => item.position.x);
        minY = (int)groundChilds.Min(item => item.position.y);
        maxY = (int)objectChilds.Max(item => item.position.y) + gapY;
        minZ = (int)groundChilds.Min(item => item.position.z);
        maxZ = (int)groundChilds.Max(item => item.position.z);
        
        totalX = maxX - minX + 1;
        totalY = maxY - minY + 1;
        totalZ = maxZ - minZ + 1;
    }

    private void Indicator()
    {
        tileMapData = new string[totalX, totalY, totalZ];
        objectMapData = new string[totalX, totalY, totalZ];
        
        int id = 0;
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Ray ray = new Ray(new Vector3(x, y + 0.5f, z - 1.5f), transform.forward);
                    RaycastHit hit;
                    
                    if (Physics.Raycast(ray, out hit,1f))
                    {
                        if (hit.collider.CompareTag("InteractObj"))
                        {
                            objectMapData[x - minX, y - minY, z - minZ] = id.ToString();
                            AddObjectInfo(hit.collider, id++);
                        }
                        else if (!hit.collider.CompareTag("Player"))
                        {
                            string[] prefabName = hit.collider.name.Split();
                            if (prefabName[0].Contains("(Clone)"))
                            {
                                prefabName[0] = prefabName[0].Replace("(Clone)", "");
                            }
                            
                            tileMapData[x - minX, y - minY, z - minZ] = prefabName[0];
                        }
                    }
                }
            }
        }
    }

    private void AddObjectInfo(Collider objectCollider, int id)
    {
        InteractiveObject interObj = objectCollider.GetComponent<InteractiveObject>();
        
        // prefabname, id, name, adjs
        ObjectInfo objectInfo = new ObjectInfo();
        string[] prefabName = objectCollider.name.Split();
        objectInfo.prefabName = prefabName[0];
        objectInfo.objectID = id;
        objectInfo.nameType = interObj.GetObjectName();

        List<EAdjective> adjectives = new List<EAdjective>();
        for (int i = 0; i < interObj.GetCheckAdj().Length; i++)
        {
            if (interObj.GetCheckAdj()[i])
            {
                if (CardDataManager.GetInstance.Adjectives.Count == 0)
                {
                    CardDataManager.GetInstance.ReadXmlFile();
                }
                
                EAdjective adjective = CardDataManager.GetInstance.Adjectives.FirstOrDefault(item => item.Value.priority == i).Key;
                adjectives.Add(adjective);
            }
        }
        objectInfo.adjectives = adjectives.ToArray();
        
        objectInfos.Add(objectInfo);
    }

    void WriteCsvFile(string[,,] dataStrings, string fileName)
    {
        StringBuilder sb = new StringBuilder();
        string delimiter = ",";
        
        if (!Directory.Exists(filePath + "/CSV/"))
        {
            Directory.CreateDirectory(filePath + "/CSV/");
        }

        sb.AppendLine(totalX + delimiter + totalY + delimiter + totalZ);
        for (int y = 0; y < totalY; y++)
        {
            sb.AppendLine("Layer" + y);
            for (int z = 0; z < totalZ; z++)
            {
                for (int x = 0; x < totalX; x++)
                {
                    delimiter = x < totalX - 1 ? "," : "";
                    
                    if (dataStrings[x, y, z] == null)
                    {
                        sb.Append("-1" + delimiter);
                    }
                    else
                    {
                        sb.Append(dataStrings[x, y, z] + delimiter);
                    }
                }
                sb.AppendLine();
            }
        }
        
        StreamWriter outStream = File.CreateText(filePath + "/CSV/"+ fileName);
        outStream.Write(sb);
        outStream.Close();
    }

    private void WriteJsonFile(string fileName)
    {
        if (!Directory.Exists(filePath + "/JSON/"))
        {
            Directory.CreateDirectory(filePath + "/JSON/");
        }
        
        string data = JsonConvert.SerializeObject(objectInfos, Formatting.Indented);
        File.WriteAllText(filePath + "/JSON/" + fileName, data);
    }
    
}
