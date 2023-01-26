using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectSurroundingHS : MonoBehaviour
{
    TileMapManager tilemap;
    GameObject[,,] mapData;
    int maxX;
    int maxY;
    int maxZ;

    // test
    [SerializeField] GameObject target;
    [SerializeField] Dir ECheckDir;

    private void Awake()
    {
        tilemap = TileMapManager.GetInstance;
        tilemap.Init();
    }

    void Start()
    {
        Indicator.GetInstance.CreateNewLevel();
        mapData = TileMapManager.GetInstance.GetTileMap();
        maxX = TileMapManager.GetInstance.maxX;
        maxY = TileMapManager.GetInstance.maxY;
        maxZ = TileMapManager.GetInstance.maxZ;
    }

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
        if (pos.x < 0 || pos.x > maxX || pos.y < 0 || pos.y > maxY || pos.z < 0 || pos.z > maxZ)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region Get Adjacent Object(s)
    private GameObject GetObjectOrNull(GameObject indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.mapData;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = GetAdjacentVector3(indicatedObj.transform.position, value, addValue);
        if (!isRightPos(newPos)) return null;
        return mapData[newPos.x, newPos.y, newPos.z];
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
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
    [ContextMenu("TestGetAdjacentObjs")]
    public void TestGetAdjacentObjs()
    {
        target = Indicator.GetInstance.target;
        foreach (GameObject go in GetAdjacentObjects(target))
        {
            if (go == null) continue;
            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentObjWithDir")]
    public void TestGetAdjacentDict()
    {
        target = Indicator.GetInstance.target;
        Dictionary<Dir, GameObject> dict = GetAdjacentDictionary(target);
        for (int i = 0; i < 6; i++)
        {
            if (!dict.Keys.Contains((Dir)i)) continue;
            GameObject go = dict[(Dir)i];
            Debug.Log("Dir" + ((Dir)i).ToString() + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestDir")]
    public void TestAdjacentObjWithDir()
    {
        GameObject go = GetAdjacentObjectWithDir(target, ECheckDir);
        if (go == null) Debug.Log("There is nothing!");
        else Debug.Log(go.name, go.transform);
    }
    #endregion
}

// 겹치는 거 예외
// 