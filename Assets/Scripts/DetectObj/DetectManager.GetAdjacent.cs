using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class DetectManager : Singleton<DetectManager>
{
#region Check Adjacent region By.HS

    // 오브젝트 데이터 배열 전체를 가져오는 메서드 
    public GameObject[,,] GetObjectsData()
    {
        return this.currentObjects;
    }

    // 타일 데이터 배열 전체를 가져오는 메서드 
    public GameObject[,,] GetTilesData()
    {
        return this.currentTiles;
    }

    // 어떤 오브젝트의 스케일을 건드릴 경우 반드시 호출해야 하는 함수들 
    #region On Any Object's Scale Changed
    // 길어질 수 있는지 체크 후에 실제로 변화시키기 전에 꼭 먼저 이 메서드를 호출하세요
    // 길어진 것이라면, isStretched = true / 줄어든 것이라면, isStretched = false
    public void OnObjectScaleChanged(Vector3 changedScale, Transform targetObj)
    {
        RemoveScaledObject(targetObj.gameObject);

        for (int x = 0; x < changedScale.x; x++)
        {
            for (int y = 0; y < changedScale.y; y++)
            {
                for (int z = 0; z < changedScale.z; z++)
                {
                    Vector3 stretchedPos = new Vector3(x, y, z);
                    if (stretchedPos == Vector3.zero) continue;
                    Vector3 vec = (targetObj.position + stretchedPos);
                    vec.x = Mathf.RoundToInt(vec.x);
                    vec.y = Mathf.RoundToInt(vec.y);
                    vec.z = Mathf.RoundToInt(vec.z);
                    scaleChangedObjects[vec] = targetObj.gameObject;
                }
            }
        }
    }

    public void RemoveScaledObject(GameObject targetObj)
    {
        List<Vector3> keys = scaleChangedObjects.Where(t => t.Value == targetObj).Select(t => t.Key).ToList();
        if (keys == null || keys.Count == 0) return;
        foreach (Vector3 key in keys)
        {
            scaleChangedObjects.Remove(key);
        }
    }

    // 스케일이 1,1,1이 아닌 오브젝트가 이동할 때, 없어질 때에 해당 오브젝트도 처리하기 위해 가져오는 메서드
    // 가져온 벡터3 리스트로 배열의 값을 수정하세요 
    public List<Vector3> GetStretchedObject(Vector3 scale, GameObject targetObj)
    {
        List<Vector3> returnObjs = new List<Vector3>();
        for (int x = 0; x < scale.x; x++)
        {
            for (int y = 0; y < scale.y; y++)
            {
                for (int z = 0; z < scale.z; z++)
                {
                    Vector3 stretchedPos = new Vector3(x, y, z);
                    if (stretchedPos == Vector3.zero) continue;
                    returnObjs.Add(targetObj.transform.position + scale);
                }
            }
        }
        return returnObjs;
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

    private bool isRightPos(Vector3Int pos, bool isObject = true)
    {
        if (isObject)
        {
            if (pos.x < 0 || pos.x > maxX || pos.y < 0 || pos.y > maxY || pos.z < 0 || pos.z > maxZ)
            {
                return false;
            }
            return true;
        }
        else
        {
            if (pos.x < 0 || pos.x > tileMaxX || pos.y < 0 || pos.y > tileMaxY || pos.z < 0 || pos.z > tileMaxZ)
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region Get Adjacent Object(s)
    private GameObject GetExistTileOrNull(int x, int y, int z)
    {
        if (!isRightPos(new Vector3Int(x, y, z), false))
            return null;
        return currentTiles[x, y, z];
    }

    private GameObject GetStretchedObjOrNull(Vector3 vec)
    {
        if (scaleChangedObjects.Keys.Contains(vec))
            return scaleChangedObjects[vec];
        return null;
    }

    private GameObject GetStretchedObjOrNull(Vector3Int vec)
    {
        if (scaleChangedObjects.Keys.Contains(vec))
            return scaleChangedObjects[vec];
        return null;
    }

    protected GameObject GetBlockOrNull(GameObject indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        return GetBlockOrNull(indicatedObj.transform.position, value, addValue, mapData);
    }

    protected GameObject GetBlockOrNull(Transform indicatedObj, string value, int addValue, GameObject[,,] mapData = null)
    {
        return GetBlockOrNull(indicatedObj.position, value, addValue, mapData);
    }

    protected GameObject GetBlockOrNull(Vector3 indicatedPos, string value, int addValue, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.currentObjects;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = GetAdjacentVector3(indicatedPos, value, addValue);
        Vector3 newPosFloat = new Vector3(newPos.x, newPos.y, newPos.z);
        if (!isRightPos(newPos, true)) return null;
        if (mapData.GetLength(1) <= newPos.y) return null; // 수정중 
        if (mapData[newPos.x, newPos.y, newPos.z] != null)
            return mapData[newPos.x, newPos.y, newPos.z];
        else
        {
            GameObject stretchedObj = GetStretchedObjOrNull(newPosFloat);
            if (stretchedObj == null)
            {
                if (!isRightPos(newPos)) return null;
                GameObject tile = GetExistTileOrNull(newPos.x, newPos.y, newPos.z);
                return tile;
            }
            else
                return stretchedObj;
        }
    }

    protected GameObject GetObjectInArray(Vector3 vec, GameObject[,,] mapData = null)
    {
        if (mapData == null)
            mapData = this.currentObjects;

        if (mapData.Length == 0)
            return null;

        Vector3Int newPos = new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        if (!isRightPos(newPos)) return null;
        if (mapData[newPos.x, newPos.y, newPos.z] != null)
            return mapData[newPos.x, newPos.y, newPos.z];
        return null;
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, int length = 1)
    {
        // 예외처리 - 배열 갱신 실패 
        if (!CheckValueInMap(indicatedObj))
            return null;

        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetBlockOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetBlockOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetBlockOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(Transform indicatedObj, Dir dir, int length = 1)
    {
        // 예외처리 - 배열 갱신 실패 
        if (!CheckValueInMap(indicatedObj.gameObject))
            return null;

        GameObject returnObj = null;
        switch (dir)
        {
            // 다른 맵 데이터에서 탐색하기 원한다면, GetObjectOrNull 네번째 파라미터로 3차원 배열 맵 데이터를 넣으세요
            case (Dir.right):
                returnObj = GetBlockOrNull(indicatedObj, "x", length);
                break;
            case (Dir.left):
                returnObj = GetBlockOrNull(indicatedObj, "x", -length);
                break;
            case (Dir.up):
                returnObj = GetBlockOrNull(indicatedObj, "y", length);
                break;
            case (Dir.down):
                returnObj = GetBlockOrNull(indicatedObj, "y", -length);
                break;
            case (Dir.forward):
                returnObj = GetBlockOrNull(indicatedObj, "z", length);
                break;
            case (Dir.back):
                returnObj = GetBlockOrNull(indicatedObj, "z", -length);
                break;
        }
        return returnObj;
    }

    // 스케일이 변경된 특정 오브젝트의 한 방향을 검출하는 로직 --> GameObject
    public GameObject GetAdjacentObjectWithDir(GameObject indicatedObj, Dir dir, Vector3 objScale)
    {
        // 예외처리 - 배열 갱신 실패 
        if (!CheckValueInMap(indicatedObj))
            return null;

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
                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
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
                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
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
                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
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
                    GameObject go = GetBlockOrNull(newPosition, "x", i % 2 == 1 ? scaleX : -1);
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
                    GameObject go = GetBlockOrNull(newPosition, "y", i % 2 == 1 ? scaleY : -1);
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
                    GameObject go = GetBlockOrNull(newPosition, "z", i % 2 == 1 ? scaleZ : -1);
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

    #region Get or Change Array Data
    // 맵 배열에서 Vector3의 값에 해당하는 게임 오브젝트들을 가져오는 메서드 
    private Dictionary<Vector3, GameObject> GetArrayObjects(params Vector3[] blocks)
    {
        Dictionary<Vector3, GameObject> returnDict = new Dictionary<Vector3, GameObject>();
        foreach (Vector3 block in blocks)
        {
            GameObject go = GetObjectInArray(block);
            returnDict.Add(block, go);
        }
        return returnDict;
    }

    // 맵 배열 데이터에서 두 개의 값을 교환하는 메서드 
    public void SwapBlockInMap(Vector3 block1, Vector3 block2)
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(block1, block2);
        GameObject go1 = dict[block1];
        GameObject go2 = dict[block2];
        
        ChangeValueInMap(block1, go2);
        ChangeValueInMap(block2, go1);

        Dictionary<Vector3, GameObject> newDict = GetArrayObjects(block1, block2);

        //Debug.Log(go1 + " -> " + newDict[block1]);
        //Debug.Log(go2 + " -> " + newDict[block2]);
    }

    // 맵 배열 데이터에서 한 개의 값을 새로운 값으로 변경하는 메서드
    // 스케일이 Vector3.one 이 아닌 오브젝트를 이동, 제거하는 경우 예외 처리 추가 
    public void ChangeValueInMap(Vector3 block, GameObject changedObject = null)
    {
        int x = Mathf.RoundToInt(block.x);
        int y = Mathf.RoundToInt(block.y);
        int z = Mathf.RoundToInt(block.z);

        GameObject preObj = currentObjects[x, y, z];
        if (preObj != null && scaleChangedObjects.Values.Contains(preObj))
        {
            RemoveScaledObject(preObj);
        }

        currentObjects[x, y, z] = changedObject;

        if (changedObject == null) return;
        if (scaleChangedObjects.Values.Contains(changedObject))
        {
            OnObjectScaleChanged(changedObject.transform.lossyScale, changedObject.transform);
        }
    }

    // 예외 처리용 함수 
    public bool CheckValueInMap(GameObject curObject)
    {
        Vector3 block = curObject.transform.position;
        int x = Mathf.RoundToInt(block.x);
        int y = Mathf.RoundToInt(block.y);
        int z = Mathf.RoundToInt(block.z);

        if (currentObjects[x, y, z] != curObject)
        {
            //Debug.LogError("배열을 제대로 갱신 하세요!");
            //if (curObject != null)
                //Debug.LogError("Error Object : " + curObject.name, curObject);
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

#endregion
}
