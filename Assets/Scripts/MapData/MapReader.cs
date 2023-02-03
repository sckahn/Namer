using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapReader : MonoBehaviour
{
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

    public MapData GetMapData()
    {
        GetMapSize();
        return Indicator();
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

    private MapData Indicator()
    {
        Transform[] tilePrefabs = Resources.LoadAll<Transform>("Prefabs/GroundTiles");
        Transform[] objectPredabs = Resources.LoadAll<Transform>("Prefabs/Objects");
        
        string[,,] tileMapData = new string[totalX, totalY, totalZ];
        string[,,] objectMapData = new string[totalX, totalY, totalZ];
        List<ObjectInfo> objectInfos = new List<ObjectInfo>();
        
        int id = 0;
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Ray ray = new Ray(new Vector3(x, y + 0.5f, z - 1.5f), transform.forward);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 1f))
                    {
                        if (hit.collider.CompareTag("InteractObj"))
                        {
                            objectMapData[x - minX, y - minY, z - minZ] = id.ToString();
                            objectInfos.Add(AddObjectInfo(hit.collider, id++,
                                GetPrefabName(objectPredabs, hit.collider.name)));
                        }
                        else if (!hit.collider.CompareTag("Player"))
                        {
                            tileMapData[x - minX, y - minY, z - minZ] = GetPrefabName(tilePrefabs, hit.collider.name);
                        }
                    }
                }
            }
        }

        return new MapData(CreateCsvData(tileMapData), CreateCsvData(objectMapData), objectInfos);
    }

    private string GetPrefabName(Transform[] prefabs, string colliderName)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (colliderName.Contains(prefabs[i].name))
            {
                return prefabs[i].name;
            }
        }

        return null;
    }

    private ObjectInfo AddObjectInfo(Collider objectCollider, int id, string prefabName)
    {
        InteractiveObject interObj = objectCollider.GetComponent<InteractiveObject>();
        
        // prefabname, id, name, adjs
        ObjectInfo objectInfo = new ObjectInfo();
        objectInfo.prefabName = prefabName;
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

        return objectInfo;
    }

    private StringBuilder CreateCsvData(string[,,] dataStrings)
    {
        StringBuilder sb = new StringBuilder();
        string delimiter = ",";

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

        return sb;
    }
}
