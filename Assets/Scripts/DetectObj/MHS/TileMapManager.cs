using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

public class TileMapManager : Singleton<TileMapManager>
{
    GameObject[,,] tileMap;
    public int maxX;
    public int maxY;
    public int maxZ;
    int blockCount = 0;

    // Test
    [SerializeField] string fileName = "Level";
    [SerializeField] GameObject target;

    void Awake()
    {
        maxX = 20;
        maxY = 5;
        maxZ = 20;
        tileMap = new GameObject[maxX,maxY,maxZ];
    }

    public GameObject[,,] GetTileMap()
    {
        return this.tileMap;
    }

    public void SetTileMap(int x, int y, int z, GameObject value)
    {
        this.tileMap[x, y, z] = value;
        value.GetComponent<Block>().BlockID = blockCount;
        blockCount++;
    }

    public GameObject GetObjectInTileMap(int x, int y, int z)
    {
        return tileMap[x, y, z];
    }

    private Vector3Int GetAdjacentVector3(Vector3 position, string value, int addValue)
    {
        Vector3Int returnVector = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        switch (value)
        {
            case ("x"):
                returnVector += Vector3Int.right * addValue;
                break;
            case ("y"):
                returnVector += Vector3Int.up * addValue;
                break;
            case ("z"):
                returnVector += Vector3Int.forward * addValue;
                break;
            default:
                break;
        }
        return returnVector;
    }

    private bool isRightPos(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x > maxX || pos.y < 0 || pos.y > maxY || pos.z < 0 || pos.z > maxZ)
        {
            return false;
        }
        return true;
    }

    public List<GameObject> GetAdjacentObject(GameObject indicatedObj)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        List<GameObject> returnObjects = new List<GameObject>(6);
        foreach (string value in values)
        {
            foreach (int addValue in addValues)
            {
                Vector3Int newPos = GetAdjacentVector3(indicatedObjPos, value, addValue);
                if (!isRightPos(newPos)) continue;
                returnObjects.Add(GetObjectInTileMap(newPos.x, newPos.y, newPos.z));
            }
        }
        return returnObjects;
    }

    // test
    [ContextMenu("TestGetAdjacentObj")]
    public void TestGetAdjacentObj()
    {
        target = Indicator.GetInstance.target;
        foreach (GameObject go in GetAdjacentObject(target))
        {
            if (go == null) continue;
            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("SaveNewLevel")]
    public void SaveNewLevel()
    {
        string path = "Assets/Scripts/DetectObj/MHS/" + fileName + ".csv";

        StringBuilder sb = new StringBuilder();
        string json = "";
        for (int x = 0; x < maxX; x++)
        {
            sb.Append($"X = {x}\n");
            for (int y = 0; y < maxY; y++)
            {
                string line = "";
                string[] lineV = new string[maxZ];
                for (int z = 0; z < maxZ; z++)
                {
                    GameObject block = tileMap[x, y, z];
                    Block blockV = block.GetComponent<Block>();
                    if (block == null)
                    {
                        lineV[z] = "-1";
                        continue;
                    }
                    else
                    {
                        lineV[z] = blockV.BlockID.ToString();
                    }
                    string contents = JsonUtility.ToJson(block);
                    json += "\n" + contents;
                }
                line = string.Join(",", lineV);
                sb.Append(line);
                sb.Append("\n");
            }
        }

        StreamWriter sw = new StreamWriter(path);
        sw.Write(sb);
        sw.Close();
        File.WriteAllText("Assets/Scripts/DetectObj/MHS/" + fileName + ".json", json);
    }

    void Update()
    {
        
    }
}
