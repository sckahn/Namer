using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectSurroundingHS : Singleton<DetectSurroundingHS>
{
    TileMapManager tilemap;
    protected GameObject[,,] mapData;
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
    public GameObject[,,] GetTileMap()
    {
        return this.mapData;
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

    private GameObject GetObjectOrNull(Transform indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.mapData;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = GetAdjacentVector3(indicatedObj.position, value, addValue);
        if (!isRightPos(newPos)) return null;
        return mapData[newPos.x, newPos.y, newPos.z];
    }

    protected GameObject GetObjectOrNull(Vector3 indicatedPos, string value, int addValue, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.mapData;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = GetAdjacentVector3(indicatedPos, value, addValue);
        if (!isRightPos(newPos)) return null;
        return mapData[newPos.x, newPos.y, newPos.z];
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, int length = 1)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetObjectOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetObjectOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetObjectOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetObjectOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetObjectOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetObjectOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(Transform indicatedObj, Dir dir, int length = 1)
    {
        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetObjectOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetObjectOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetObjectOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetObjectOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetObjectOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetObjectOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 스케일이 변경된 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, Vector3 objScale)
    {
        int length = 1;
        if ((int)dir % 2 == 1)
        {
            length = (int)dir < 2 ? Mathf.RoundToInt(objScale.x)
                : ((int)dir < 4 ? Mathf.RoundToInt(objScale.y)
                : Mathf.RoundToInt(objScale.z));
        }
        return GetAdjacentObjectWithDir(indicatedObj, dir, length);
    }

    // 특정 오브젝트의 6 방향을 검출하는 로직 1 --> List<GameObject>
    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj)
    {
        Vector3 objScale = indicatedObj.transform.lossyScale;
        if (objScale != Vector3.one)
        {
            return GetAdjacentObjects(indicatedObj, objScale);
        }

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

    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> List<GameObject>
    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj, Vector3 objScale)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        int scaleX = Mathf.RoundToInt(objScale.x);
        int scaleY = Mathf.RoundToInt(objScale.y);
        int scaleZ = Mathf.RoundToInt(objScale.z);
        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

        List<GameObject> returnObjects = new List<GameObject>(count);

        for (int i = 0; i < 2; i++)
        {
            for (int y = 0; y < scaleY; y++)
            {
                for (int z = 0; z < scaleZ; z++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
                    newPosition = GetAdjacentVector3(newPosition, "z", z);
                    GameObject go = GetObjectOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        for (int i = 2; i < 4; i++)
        {
            for (int z = 0; z < scaleZ; z++)
            {
                for (int x = 0; x < scaleX; x++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
                    newPosition = GetAdjacentVector3(newPosition, "x", x);
                    GameObject go = GetObjectOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        for (int i = 4; i < 6; i++)
        {
            for (int x = 0; x < scaleX; x++)
            {
                for (int y = 0; y < scaleY; y++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
                    newPosition = GetAdjacentVector3(newPosition, "y", y);
                    GameObject go = GetObjectOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
                    if (go == null || returnObjects.Contains(go)) continue;
                    returnObjects.Add(go);
                }
            }
        }
        return returnObjects;
    }

    // 특정 오브젝트의 6 방향을 검출하는 로직 2 --> Dictionary<Dir, GameObject>
    public Dictionary<Dir, GameObject> GetAdjacentDictionary(GameObject indicatedObj)
    {
        Vector3 objScale = indicatedObj.transform.lossyScale;
        if (objScale != Vector3.one)
        {
            Debug.LogError("Use GetAdjacentsDictionary");
            return null;
        }

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

    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> Dictionary<Dir, List<GameObject>>
    public Dictionary<Dir, List<GameObject>> GetAdjacentsDictionary(GameObject indicatedObj, Vector3 objScale)
    {
        Vector3 indicatedObjPos = indicatedObj.transform.position;
        string[] values = new string[3] { "x", "y", "z" };
        int[] addValues = new int[2] { 1, -1 };

        int scaleX = Mathf.RoundToInt(objScale.x);
        int scaleY = Mathf.RoundToInt(objScale.y);
        int scaleZ = Mathf.RoundToInt(objScale.z);
        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

        Dictionary<Dir, List<GameObject>> returnObjects = new Dictionary<Dir, List<GameObject>>(6);

        for (int i = 0; i < 2; i++)
        {
            for (int y = 0; y < scaleY; y++)
            {
                for (int z = 0; z < scaleZ; z++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
                    newPosition = GetAdjacentVector3(newPosition, "z", z);
                    GameObject go = GetObjectOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        for (int i = 2; i < 4; i++)
        {
            for (int z = 0; z < scaleZ; z++)
            {
                for (int x = 0; x < scaleX; x++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
                    newPosition = GetAdjacentVector3(newPosition, "x", x);
                    GameObject go = GetObjectOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        for (int i = 4; i < 6; i++)
        {
            for (int x = 0; x < scaleX; x++)
            {
                for (int y = 0; y < scaleY; y++)
                {
                    Transform newPos = indicatedObj.transform;
                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
                    newPosition = GetAdjacentVector3(newPosition, "y", y);
                    GameObject go = GetObjectOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
                    if (returnObjects.Keys.Contains((Dir)i))
                        returnObjects[(Dir)i].Add(go);
                    else
                    {
                        returnObjects.Add((Dir)i, new List<GameObject>());
                        returnObjects[(Dir)i].Add(go);
                    }
                }
            }
        }
        return returnObjects;
    }

    #endregion

    #region Test
    [ContextMenu("TestGetAdjacentObjs")]
    public void TestGetAdjacentObjs()
    {
        foreach (GameObject go in GetAdjacentObjects(target))
        {
            if (go == null) continue;
            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentObjWithDir")]
    public void TestGetAdjacentDict()
    {
        Dictionary<Dir, GameObject> dict = GetAdjacentDictionary(target);
        for (int i = 0; i < 6; i++)
        {
            if (!dict.Keys.Contains((Dir)i)) continue;
            GameObject go = dict[(Dir)i];
            Debug.Log("Dir " + ((Dir)i) + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentsDict")]
    public void TestGetAdjacentsDict()
    {
        Dictionary<Dir, List<GameObject>> dict = GetAdjacentsDictionary(target, target.transform.lossyScale);
        foreach (Dir d in dict.Keys)
        {
            foreach (GameObject go in dict[d])
            {
                Debug.Log("Dir " + d + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
            }
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

// 전체 맵 데이터를 순환하는 검출 로직
// 특정 오브젝트 인접한 친구들만 검출하는 로직 (gameObject, Transform)
// return --> List{ (GameObject, Adjactive), .... }

// 전체 맵 데이터를 순환하는 검출 로직
// 특정 오브젝트 인접한 친구들만 검출하는 로직 (gameObject, Transform)
// return --> List{ (GameObject, Adjactive), .... }