using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

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

    #region Create TileMap & Get TileMap
    public void Init()
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
    #endregion

    #region GetVector in TileMap & isRight?
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
        if (pos.x < 0 || pos.x >= maxX || pos.y < 0 || pos.y >= maxY || pos.z < 0 || pos.z >= maxZ)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region Get Adjacent Object(s)
    private GameObject GetObjectOrNull(GameObject indicatedObj, string value, int addValue)
    {
        Vector3Int newPos = GetAdjacentVector3(indicatedObj.transform.position, value, addValue);
        if (!isRightPos(newPos)) return null;
        return GetObjectInTileMap(newPos.x, newPos.y, newPos.z);
        // 나중에 맵 3차원 배열이 나오면 해당 배열에서 가져오도록 메서드를 수정해야 함 
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            case (Dir.right):
                returnObj = GetObjectOrNull(indicatedObj, "x", 1);
                break;
            case (Dir.left):
                returnObj = GetObjectOrNull(indicatedObj, "x", -1);
                break;
            case (Dir.up):
                returnObj = GetObjectOrNull(indicatedObj, "y", 1);
                break;
            case (Dir.down):
                returnObj = GetObjectOrNull(indicatedObj, "y", -1);
                break;
            case (Dir.forward):
                returnObj = GetObjectOrNull(indicatedObj, "z", 1);
                break;
            case (Dir.back):
                returnObj = GetObjectOrNull(indicatedObj, "z", -1);
                break;
        }
        return returnObj;
    }

    // 특정 오브젝트의 전 방향을 검출하는 로직 1 --> Dictionary<Dir, GameObject>
    public Dictionary<Dir, GameObject> GetAdjacentDictionary(GameObject indicatedObj)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        Dictionary<Dir, GameObject> returnObjects = new Dictionary<Dir, GameObject>(6);

        for (int i = 0; i < 6; i++)
        {
            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
            if (go == null) continue;
            returnObjects[(Dir)i] = go;
        }
        return returnObjects;
    }

    // 특정 오브젝트의 전 방향을 검출하는 로직 2 --> List<GameObject>
    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        List<GameObject> returnObjects = new List<GameObject>(6);

        for (int i = 0; i < 6; i++)
        {
            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
            if (go == null) continue;
            returnObjects.Add(go);
        }
        return returnObjects;
    }
    #endregion

    #region Test
    // test
    [ContextMenu("TestGetAdjacentObj")]
    public void TestGetAdjacentObj()
    {
        target = Indicator.GetInstance.target;
        foreach (GameObject go in GetAdjacentObjects(target))
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
    #endregion
}
