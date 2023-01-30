using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 합치지 않고 테스트 하시려면, GetObjectOrNull과 mapData를 protected로 하고 실행하세요 
public class ChangeArrayHS : DetectSurroundingHS
{
    //Test
    [SerializeField] Vector3 vec1;
    [SerializeField] Vector3 vec2;
    [SerializeField] GameObject changeGo;

    // 맵 배열에서 Vector3의 값에 해당하는 게임 오브젝트들을 가져오는 메서드 
    private Dictionary<Vector3, GameObject> GetArrayObject(params Vector3[] blocks)
    {
        Dictionary<Vector3, GameObject> returnDict = new Dictionary<Vector3, GameObject>();
        foreach (Vector3 block in blocks)
        {
            GameObject go = GetBlockOrNull(block, "x", 0);
            returnDict.Add(block, go);
        }
        return returnDict;
    }

    // 맵 배열 데이터에서 두 개의 값을 교환하는 메서드 
    public void SwapBlockInMap(Vector3 block1, Vector3 block2)
    {
        Dictionary <Vector3, GameObject> dict = GetArrayObject(block1, block2);
        GameObject go1 = dict[block1];
        GameObject go2 = dict[block2];

        ChangeValueInMap(block1, go2);
        ChangeValueInMap(block2, go1);

        Dictionary<Vector3, GameObject> newDict = GetArrayObject(block1, block2);

        Debug.Log(go1 + " -> " + newDict[block1]);
        Debug.Log(go2 + " -> " + newDict[block2]);
    }

    // 맵 배열 데이터에서 한 개의 값을 새로운 값으로 변경하는 메서드 
    public void ChangeValueInMap(Vector3 block, GameObject curObject = null)
    {
        int x = Mathf.RoundToInt(block.x);
        int y = Mathf.RoundToInt(block.y);
        int z = Mathf.RoundToInt(block.z);

        currentObjects[x, y, z] = curObject;
    }

    #region Test
    [ContextMenu("TestSwap")]
    public void TestSwap()
    {
        SwapBlockInMap(vec1, vec2);
    }

    [ContextMenu("TestChange")]
    public void TestChange()
    {
        ChangeValueInMap(vec1, changeGo);

        Dictionary<Vector3, GameObject> dict = GetArrayObject(vec1);
        Debug.Log(dict[vec1]);
    }
    #endregion
}

// 1,1,1인 오브젝트가 위치가 바뀌어서 6면을 검사할 때에 해당 타일에 없다고 쳐도
// 만약 그 아래 그 위 등이 스케일이 변경된 친구면 걔네까지 검사



//ㅁBㅇㅁ
//ㅁㅁㅁㅁ

// ㅇ이 객체
// B는 안 움직였? --> 움직인 애들에서만 for
// ㅇ은 빠지게 되는데 사실 B랑 처리는 해야되죠 ?
// 근데 저 로직에서는 아예 처리가 안 될거 같습니다 