//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class DetectSurroundingHS : Singleton<DetectSurroundingHS>
//{
//    MapDataManager mapManager;
//    LevelInfos levelInfos;
//    Dictionary<Vector3, GameObject> scaleChangedObjects = new Dictionary<Vector3, GameObject>();
//    protected GameObject[,,] currentObjects;
//    protected GameObject[,,] currentTiles;
//    int maxX = 20;
//    int maxY = 9;
//    int maxZ = 20;

//    int tileMaxX = 20;
//    int tileMaxY = 9;
//    int tileMaxZ = 20;

//    // test
//    private GameObject target;
//    private Dir ECheckDir;

//    // LevelInfos 컴포넌트에서 씬이 열리고 바로 해당 함수를 호출
//    // 호출시에 바로 모드를 파악하고, 맵을 로드하거나 (에디터모드인 경우는) 맵 파일을 저장함 
//    public void Init(LevelInfos infos)
//    {
//        GameObject player = GameObject.Find("Player");
//        if (player != null) player.SetActive(false);
//        this.levelInfos = infos;
//        mapManager = MapDataManager.GetInstance;
//        if (levelInfos.IsCreateMode)
//        {
//            mapManager.CreateFile();
//        }
//        else
//        {
//            mapManager.CreateMap(levelInfos.LevelName);
//            SetMapData();
//            if (player != null) player.SetActive(true);
//        }
//    }

//    // 맵을 로드할 때에 한 번 배열을 가져오는 메서드로 따로 사용하면 안 됨
//    // 기존 맵 배열을 사용하는 것이 아니라 인게임용 배열을 가지고 검출, 수정 등을 할 예정 
//    private void SetMapData()
//    {
//        currentObjects = (GameObject[,,])mapManager.InitObjects.Clone();
//        currentTiles = (GameObject[,,])mapManager.InitTiles.Clone();

//        maxX = currentObjects.GetLength(0) - 1;
//        maxY = currentObjects.GetLength(1) + 2;
//        maxZ = currentObjects.GetLength(2) - 1;

//        tileMaxX = currentTiles.GetLength(0) - 1;
//        tileMaxY = currentTiles.GetLength(1) - 1;
//        tileMaxZ = currentTiles.GetLength(2) - 1;
//    }

//    // 오브젝트 데이터 배열 전체를 가져오는 메서드 
//    public GameObject[,,] GetObjectsData()
//    {
//        return this.currentObjects;
//    }

//    // 타일 데이터 배열 전체를 가져오는 메서드 
//    public GameObject[,,] GetTilesData()
//    {
//        return this.currentTiles;
//    }

//    // 어떤 오브젝트의 스케일을 건드릴 경우 반드시 호출해야 하는 함수들 
//    #region On Any Object's Scale Changed
//    // 길어질 수 있는지 체크 후에 실제로 변화시키기 전에 꼭 먼저 이 메서드를 호출하세요
//    // 길어진 것이라면, isStretched = true / 줄어든 것이라면, isStretched = false
//    public void OnObjectScaleChanged(Vector3 changedScale, Transform targetObj, bool isStreched = true)
//    {
//        if (isStreched)
//        {
//            for (int x = 0; x < changedScale.x; x++)
//            {
//                for (int y = 0; y < changedScale.y; y++)
//                {
//                    for (int z = 0; z < changedScale.z; z++)
//                    {
//                        Vector3 stretchedPos = new Vector3(x, y, z);
//                        if (stretchedPos == Vector3.zero) continue;
//                        scaleChangedObjects[targetObj.position + stretchedPos] = targetObj.gameObject;
//                    }
//                }
//            }
//        }
//        else
//        {
//            Vector3 LostScale = targetObj.lossyScale - changedScale;
//            for (int x = 0; x <= LostScale.x; x++)
//            {
//                for (int y = 0; y <= LostScale.y; y++)
//                {
//                    for (int z = 0; z <= LostScale.z; z++)
//                    {
//                        Vector3 newPos = new Vector3(x, y, z);
//                        if (newPos == Vector3.zero) continue;
//                        scaleChangedObjects.Remove(targetObj.position + changedScale + newPos);
//                    }
//                }
//            }
//        }
//    }

//    // 스케일이 1,1,1이 아닌 오브젝트가 이동할 때, 없어질 때에 해당 오브젝트도 처리하기 위해 가져오는 메서드
//    // 가져온 벡터3 리스트로 배열의 값을 수정하세요 
//    public List<Vector3> GetStretchedObject(Vector3 scale, GameObject targetObj)
//    {
//        List<Vector3> returnObjs = new List<Vector3>();
//        for (int x = 0; x < scale.x; x++)
//        {
//            for (int y = 0; y < scale.y; y++)
//            {
//                for (int z = 0; z < scale.z; z++)
//                {
//                    Vector3 stretchedPos = new Vector3(x, y, z);
//                    if (stretchedPos == Vector3.zero) continue;
//                    returnObjs.Add(targetObj.transform.position + scale);
//                }
//            }
//        }
//        return returnObjs;
//    }
//    #endregion

//    #region GetVector in TileMap & isRight?
//    private Vector3Int GetAdjacentVector3(Vector3 position, string value, int addValue)
//    {
//        Vector3Int returnVector = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
//        switch (value)
//        {
//            case ("x"):
//                returnVector += Vector3Int.right * addValue;
//                break;
//            case ("y"):
//                returnVector += Vector3Int.up * addValue;
//                break;
//            case ("z"):
//                returnVector += Vector3Int.forward * addValue;
//                break;
//            default:
//                break;
//        }
//        return returnVector;
//    }

//    private bool isRightPos(Vector3Int pos, bool isObject = true)
//    {
//        if (isObject)
//        {
//            if (pos.x < 0 || pos.x > maxX || pos.y < 0 || pos.y > maxY || pos.z < 0 || pos.z > maxZ)
//            {
//                return false;
//            }
//            return true;
//        }
//        else
//        {
//            if (pos.x < 0 || pos.x > tileMaxX || pos.y < 0 || pos.y > tileMaxY || pos.z < 0 || pos.z > tileMaxZ)
//            {
//                return false;
//            }
//            return true;
//        }
//    }
//    #endregion

//    #region Get Adjacent Object(s)
//    private GameObject GetExistTileOrNull(int x, int y, int z)
//    {
//        if (!isRightPos(new Vector3Int(x, y, z), false))
//            return null;
//        return currentTiles[x, y, z];
//    }

//    private GameObject GetStretchedObjOrNull(Vector3 vec)
//    {
//        if (scaleChangedObjects.Keys.Contains(vec))
//            return scaleChangedObjects[vec];
//        return null;
//    }

//    protected GameObject GetBlockOrNull(GameObject indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
//    {
//        return GetBlockOrNull(indicatedObj.transform.position, value, addValue, mapData);
//    }

//    protected GameObject GetBlockOrNull(Transform indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
//    {
//        return GetBlockOrNull(indicatedObj.position, value, addValue, mapData);
//    }

//    protected GameObject GetBlockOrNull(Vector3 indicatedPos, string value, int addValue, GameObject[,,] mapData = null)
//    {
//        if (mapData == null)
//            mapData = this.currentObjects;

//        if (mapData.Length == 0)
//            return null;

//        Vector3Int newPos = GetAdjacentVector3(indicatedPos, value, addValue);
//        if (!isRightPos(newPos)) return null;
//        if (mapData[newPos.x, newPos.y, newPos.z] != null)
//            return mapData[newPos.x, newPos.y, newPos.z];
//        else
//        {
//            GameObject stretchedObj = GetStretchedObjOrNull(newPos);
//            if (stretchedObj == null)
//            {
//                GameObject tile = GetExistTileOrNull(newPos.x, newPos.y, newPos.z);
//                return tile;
//            }
//            else
//                return stretchedObj;
//        }
//    }

//    protected GameObject GetObjectInArray(Vector3 vec, GameObject[,,] mapData = null)
//    {
//        if (mapData == null)
//            mapData = this.currentObjects;

//        if (mapData.Length == 0)
//            return null;

//        Vector3Int newPos = new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
//        if (!isRightPos(newPos)) return null;
//        if (mapData[newPos.x, newPos.y, newPos.z] != null)
//            return mapData[newPos.x, newPos.y, newPos.z];
//        return null;
//    } 

//    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
//    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, int length = 1)
//    {
//        GameObject returnObj = null;
//        switch (dir)
//        {
//            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
//            case (Dir.right):
//                returnObj = GetBlockOrNull(indicatedObj, "x", length);
//                break;
//            case (Dir.left):
//                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
//                break;
//            case (Dir.up):
//                returnObj = GetBlockOrNull(indicatedObj, "y", length);
//                break;
//            case (Dir.down):
//                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
//                break;
//            case (Dir.forward):
//                returnObj = GetBlockOrNull(indicatedObj, "z", length);
//                break;
//            case (Dir.back):
//                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
//                break;
//        }
//        return returnObj;
//    }

//    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
//    public GameObject GetAdjacentObjectWithDir(Transform indicatedObj, Dir dir, int length = 1)
//    {
//        GameObject returnObj = null;
//        switch (dir)
//        {
//            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
//            case (Dir.right):
//                returnObj = GetBlockOrNull(indicatedObj, "x", length);
//                break;
//            case (Dir.left):
//                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
//                break;
//            case (Dir.up):
//                returnObj = GetBlockOrNull(indicatedObj, "y", length);
//                break;
//            case (Dir.down):
//                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
//                break;
//            case (Dir.forward):
//                returnObj = GetBlockOrNull(indicatedObj, "z", length);
//                break;
//            case (Dir.back):
//                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
//                break;
//        }
//        return returnObj;
//    }

//    // 스케일이 변경된 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
//    // 생각해보니 여러개를 가져올 수도 있는데... 일단 그건 나중에 수정할 예정 
//    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, Vector3 objScale)
//    {
//        int length = 1;
//        if ((int)dir % 2 == 1)
//        {
//            length = (int)dir < 2 ? Mathf.RoundToInt(objScale.x)
//                : ((int)dir < 4 ? Mathf.RoundToInt(objScale.y)
//                : Mathf.RoundToInt(objScale.z));
//        }
//        return GetAdjacentObjectWithDir(indicatedObj, dir, length);
//    }

//    // 특정 오브젝트의 6 방향을 검출하는 로직 1 --> List<GameObject>
//    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj)
//    {
//        Vector3 objScale = indicatedObj.transform.lossyScale;
//        if (objScale != Vector3.one)
//        {
//            return GetAdjacentObjects(indicatedObj, objScale);
//        }

//        Vector3 indicatedObjPos = indicatedObj.transform.position;
//        string[] values = new string[3] { "x", "y", "z" };
//        int[] addValues = new int[2] { 1, -1 };

//        List<GameObject> returnObjects = new List<GameObject>(6);

//        for (int i = 0; i < 6; i++)
//        {
//            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
//            if (go == null) continue;
//            returnObjects.Add(go);
//        }
//        return returnObjects;
//    }

//    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> List<GameObject>
//    public List<GameObject> GetAdjacentObjects(GameObject indicatedObj, Vector3 objScale)
//    {
//        Vector3 indicatedObjPos = indicatedObj.transform.position;
//        string[] values = new string[3] { "x", "y", "z" };
//        int[] addValues = new int[2] { 1, -1 };

//        int scaleX = Mathf.RoundToInt(objScale.x);
//        int scaleY = Mathf.RoundToInt(objScale.y);
//        int scaleZ = Mathf.RoundToInt(objScale.z);
//        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

//        List<GameObject> returnObjects = new List<GameObject>(count);

//        for (int i = 0; i < 2; i++)
//        {
//            for (int y = 0; y < scaleY; y++)
//            {
//                for (int z = 0; z < scaleZ; z++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
//                    newPosition = GetAdjacentVector3(newPosition, "z", z);
//                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
//                    if (go == null || returnObjects.Contains(go)) continue;
//                    returnObjects.Add(go);
//                }
//            }
//        }
//        for (int i = 2; i < 4; i++)
//        {
//            for (int z = 0; z < scaleZ; z++)
//            {
//                for (int x = 0; x < scaleX; x++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
//                    newPosition = GetAdjacentVector3(newPosition, "x", x);
//                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
//                    if (go == null || returnObjects.Contains(go)) continue;
//                    returnObjects.Add(go);
//                }
//            }
//        }
//        for (int i = 4; i < 6; i++)
//        {
//            for (int x = 0; x < scaleX; x++)
//            {
//                for (int y = 0; y < scaleY; y++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
//                    newPosition = GetAdjacentVector3(newPosition, "y", y);
//                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
//                    if (go == null || returnObjects.Contains(go)) continue;
//                    returnObjects.Add(go);
//                }
//            }
//        }
//        return returnObjects;
//    }

//    // 특정 오브젝트의 6 방향을 검출하는 로직 2 --> Dictionary<Dir, GameObject>
//    public Dictionary<Dir, GameObject> GetAdjacentDictionary(GameObject indicatedObj)
//    {
//        Vector3 objScale = indicatedObj.transform.lossyScale;
//        if (objScale != Vector3.one)
//        {
//            Debug.LogError("Use GetAdjacentsDictionary");
//            return null;
//        }

//        Vector3 indicatedObjPos = indicatedObj.transform.position;
//        string[] values = new string[3] { "x", "y", "z" };
//        int[] addValues = new int[2] { 1, -1 };

//        Dictionary<Dir, GameObject> returnObjects = new Dictionary<Dir, GameObject>(6);

//        for (int i = 0; i < 6; i++)
//        {
//            GameObject go = GetAdjacentObjectWithDir(indicatedObj, (Dir)i);
//            if (go == null) continue;
//            returnObjects[(Dir)i] = go;
//        }
//        return returnObjects;
//    }

//    // 스케일이 변경된 특정 오브젝트의 6++ 방향을 검출하는 로직 --> Dictionary<Dir, List<GameObject>>
//    public Dictionary<Dir, List<GameObject>> GetAdjacentsDictionary(GameObject indicatedObj, Vector3 objScale)
//    {
//        Vector3 indicatedObjPos = indicatedObj.transform.position;
//        string[] values = new string[3] { "x", "y", "z" };
//        int[] addValues = new int[2] { 1, -1 };

//        int scaleX = Mathf.RoundToInt(objScale.x);
//        int scaleY = Mathf.RoundToInt(objScale.y);
//        int scaleZ = Mathf.RoundToInt(objScale.z);
//        int count = ((scaleX * scaleY) + (scaleY * scaleZ) + (scaleZ * scaleX)) * 2;

//        Dictionary<Dir, List<GameObject>> returnObjects = new Dictionary<Dir, List<GameObject>>(6);

//        for (int i = 0; i < 2; i++)
//        {
//            for (int y = 0; y < scaleY; y++)
//            {
//                for (int z = 0; z < scaleZ; z++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "y", y);
//                    newPosition = GetAdjacentVector3(newPosition, "z", z);
//                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
//                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
//                    if (returnObjects.Keys.Contains((Dir)i))
//                        returnObjects[(Dir)i].Add(go);
//                    else
//                    {
//                        returnObjects.Add((Dir)i, new List<GameObject>());
//                        returnObjects[(Dir)i].Add(go);
//                    }
//                }
//            }
//        }
//        for (int i = 2; i < 4; i++)
//        {
//            for (int z = 0; z < scaleZ; z++)
//            {
//                for (int x = 0; x < scaleX; x++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "z", z);
//                    newPosition = GetAdjacentVector3(newPosition, "x", x);
//                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
//                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
//                    if (returnObjects.Keys.Contains((Dir)i))
//                        returnObjects[(Dir)i].Add(go);
//                    else
//                    {
//                        returnObjects.Add((Dir)i, new List<GameObject>());
//                        returnObjects[(Dir)i].Add(go);
//                    }
//                }
//            }
//        }
//        for (int i = 4; i < 6; i++)
//        {
//            for (int x = 0; x < scaleX; x++)
//            {
//                for (int y = 0; y < scaleY; y++)
//                {
//                    Transform newPos = indicatedObj.transform;
//                    Vector3Int newPosition = GetAdjacentVector3(newPos.position, "x", x);
//                    newPosition = GetAdjacentVector3(newPosition, "y", y);
//                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
//                    if (go == null || (returnObjects.Keys.Contains((Dir)i) && returnObjects[(Dir)i].Contains(go))) continue;
//                    if (returnObjects.Keys.Contains((Dir)i))
//                        returnObjects[(Dir)i].Add(go);
//                    else
//                    {
//                        returnObjects.Add((Dir)i, new List<GameObject>());
//                        returnObjects[(Dir)i].Add(go);
//                    }
//                }
//            }
//        }
//        return returnObjects;
//    }

//    #endregion

//    #region Get or Change Array Data
//    // 맵 배열에서 Vector3의 값에 해당하는 게임 오브젝트들을 가져오는 메서드 
//    private Dictionary<Vector3, GameObject> GetArrayObjects(params Vector3[] blocks)
//    {
//        Dictionary<Vector3, GameObject> returnDict = new Dictionary<Vector3, GameObject>();
//        foreach (Vector3 block in blocks)
//        {
//            GameObject go = GetObjectInArray(block);
//            returnDict.Add(block, go);
//        }
//        return returnDict;
//    }

//    // 맵 배열 데이터에서 두 개의 값을 교환하는 메서드 
//    public void SwapBlockInMap(Vector3 block1, Vector3 block2)
//    {
//        Dictionary<Vector3, GameObject> dict = GetArrayObjects(block1, block2);
//        GameObject go1 = dict[block1];
//        GameObject go2 = dict[block2];

//        ChangeValueInMap(block1, go2);
//        ChangeValueInMap(block2, go1);

//        Dictionary<Vector3, GameObject> newDict = GetArrayObjects(block1, block2);

//        //Debug.Log(go1 + " -> " + newDict[block1]);
//        //Debug.Log(go2 + " -> " + newDict[block2]);
//    }

//    // 맵 배열 데이터에서 한 개의 값을 새로운 값으로 변경하는 메서드 
//    public void ChangeValueInMap(Vector3 block, GameObject curObject = null)
//    {
//        int x = Mathf.RoundToInt(block.x);
//        int y = Mathf.RoundToInt(block.y);
//        int z = Mathf.RoundToInt(block.z);

//        currentObjects[x, y, z] = curObject;
//    }
//    #endregion

//    #region Test
//    private void SetTarget()
//    {
//        target = levelInfos.target;
//        ECheckDir = levelInfos.ECheckDir;
//    }

//    [ContextMenu("TestGetAdjacentObjs")]
//    public void TestGetAdjacentObjs()
//    {
//        SetTarget();
//        foreach (GameObject go in GetAdjacentObjects(target))
//        {
//            if (go == null) continue;
//            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
//        }
//    }

//    [ContextMenu("TestGetAdjacentObjWithDir")]
//    public void TestGetAdjacentDict()
//    {
//        SetTarget();
//        Dictionary<Dir, GameObject> dict = GetAdjacentDictionary(target);
//        for (int i = 0; i < 6; i++)
//        {
//            if (!dict.Keys.Contains((Dir)i)) continue;
//            GameObject go = dict[(Dir)i];
//            Debug.Log("Dir " + ((Dir)i) + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
//        }
//    }

//    [ContextMenu("TestGetAdjacentsDict")]
//    public void TestGetAdjacentsDict()
//    {
//        SetTarget();
//        Dictionary<Dir, List<GameObject>> dict = GetAdjacentsDictionary(target, target.transform.lossyScale);
//        foreach (Dir d in dict.Keys)
//        {
//            foreach (GameObject go in dict[d])
//            {
//                Debug.Log("Dir " + d + " " + go.name + ", pos : " + go.transform.position.ToString(), go.transform);
//            }
//        }
//    }

//    [ContextMenu("TestDir")]
//    public void TestAdjacentObjWithDir()
//    {
//        SetTarget();
//        GameObject go = GetAdjacentObjectWithDir(target, ECheckDir);
//        if (go == null) Debug.Log("There is nothing!");
//        else Debug.Log(go.name, go.transform);
//    }

//    [ContextMenu("TestSwap")]
//    public void TestSwapValue()
//    {
//        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1, levelInfos.block2);
//        Debug.Log("Before");
//        Debug.Log("block1 : " + dict[levelInfos.block1]);
//        Debug.Log("block2 : " + dict[levelInfos.block2]);
//        SwapBlockInMap(levelInfos.block1, levelInfos.block2);
//        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1, levelInfos.block2);
//        Debug.Log("After");
//        Debug.Log("block1 : " + dict2[levelInfos.block1]);
//        Debug.Log("block2 : " + dict2[levelInfos.block2]);
//    }

//    [ContextMenu("TestChange")]
//    public void TestChnageValue()
//    {
//        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1);
//        Debug.Log("block1 : " + dict[levelInfos.block1]);
//        ChangeValueInMap(levelInfos.block1, levelInfos.newValue);
//        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1);
//        Debug.Log("block1 : " + dict2[levelInfos.block1]);
//    }
//    #endregion
//}

//// 전체 맵 데이터를 순환하는 검출 로직
//// 특정 오브젝트 인접한 친구들만 검출하는 로직 (gameObject, Transform)
//// return --> List{ (GameObject, Adjactive), .... }

//// 전체 맵 데이터를 순환하는 검출 로직
//// 특정 오브젝트 인접한 친구들만 검출하는 로직 (gameObject, Transform)
//// return --> List{ (GameObject, Adjactive), .... }