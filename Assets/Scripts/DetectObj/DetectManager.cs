using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

// Partial class로 파일을 나눠서 관리하도록 했습니다.
// DetectManager.cs & DetectManager.Detect.cs & DetectManager.GetAdjacent.cs
public partial class DetectManager : Singleton<DetectManager>
{
    private GameObject[,,] currentObjects;
    private GameObject[,,] previousObjects;

    protected GameObject[,,] currentTiles;

    MapDataManager mapManager;
    LevelInfos levelInfos;
    Dictionary<Vector3, GameObject> scaleChangedObjects = new Dictionary<Vector3, GameObject>();

    int maxX = 20;
    int maxY = 9;
    int maxZ = 20;

    int tileMaxX = 20;
    int tileMaxY = 9;
    int tileMaxZ = 20;

    // test
    private GameObject target;
    private Dir ECheckDir;
    public List<GameObject> ForTestPurpos;

#region Test
    
   
    public void TestSetForTestPurposeList()
    {
        ForTestPurpos = new List<GameObject>();
        Debug.Log(currentObjects.Length);
        foreach (var item in currentObjects)
        {
            if(item == null) continue;
            string goName = item.name;
            if (goName.Contains('W'))
            {
                ForTestPurpos.Add(item);
            }
        }
            
    }
        
    [ContextMenu("TestGameObjListDetector")]
    public void TestGameObjs()
    {
        TestSetForTestPurposeList();
        InteractionDetector(ForTestPurpos);
    }
  
    [ContextMenu("TestDetectMoveGameObj")]
    public void TestDetectMoveGameObj()
    {
        var test = DetectMovedGameObject();
        foreach (var item in test)
        {
            Debug.Log(item, item.transform);
        }
    }

    private void SetTarget()
    {
        target = levelInfos.target;
        ECheckDir = levelInfos.ECheckDir;
    }

    [ContextMenu("TestGetAdjacentObjs")]
    public void TestGetAdjacentObjs()
    {
        SetTarget();
        foreach (GameObject go in GetAdjacentObjects(target))
        {
            if (go == null) continue;
            Debug.Log(go.name + ", pos : " + go.transform.position.ToString(), go.transform);
        }
    }

    [ContextMenu("TestGetAdjacentObjWithDir")]
    public void TestGetAdjacentDict()
    {
        SetTarget();
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
        SetTarget();
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
        SetTarget();
        GameObject go = GetAdjacentObjectWithDir(target, ECheckDir);
        if (go == null) Debug.Log("There is nothing!");
        else Debug.Log(go.name, go.transform);
    }

    [ContextMenu("TestSwap")]
    public void TestSwapValue()
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1, levelInfos.block2);
        Debug.Log("Before");
        Debug.Log("block1 : " + dict[levelInfos.block1]);
        Debug.Log("block2 : " + dict[levelInfos.block2]);
        SwapBlockInMap(levelInfos.block1, levelInfos.block2);
        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1, levelInfos.block2);
        Debug.Log("After");
        Debug.Log("block1 : " + dict2[levelInfos.block1]);
        Debug.Log("block2 : " + dict2[levelInfos.block2]);
    }

    [ContextMenu("TestChange")]
    public void TestChnageValue()
    {
        Dictionary<Vector3, GameObject> dict = GetArrayObjects(levelInfos.block1);
        Debug.Log("block1 : " + dict[levelInfos.block1]);
        ChangeValueInMap(levelInfos.block1, levelInfos.newValue);
        Dictionary<Vector3, GameObject> dict2 = GetArrayObjects(levelInfos.block1);
        Debug.Log("block1 : " + dict2[levelInfos.block1]);
    }

    [ContextMenu("TestAddScaledObject")]
    public void TestAddScaledObject()
    {
        OnObjectScaleChanged(levelInfos.changeScale, levelInfos.target.transform, levelInfos.isStretched);
    }
#endregion
}

// y축으로 둥둥 시에 기존에 y축 length을 넘어버리면, out of range