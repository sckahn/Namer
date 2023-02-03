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

    GameDataManager gameDataManager;
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

    // LevelInfos 컴포넌트에서 씬이 열리고 바로 해당 함수를 호출
    // 호출시에 바로 모드를 파악하고, 맵을 로드하거나 (에디터모드인 경우는) 맵 파일을 저장함 
    public void Init(int level)
    {
        levelInfos = FindObjectOfType<LevelInfos>();
        GameObject player = GameObject.Find("Player");
        if (player != null) player.SetActive(false);
        gameDataManager = GameDataManager.GetInstance;
        gameDataManager.GetUserAndLevelData();

        if (levelInfos.IsCreateMode)
        {
            gameDataManager.CreateFile();
        }
        else
        {
            string levelName = gameDataManager.LevelDataDic[level].SceneName;
            Position position = gameDataManager.LevelDataDic[level].playerPosition;
            player.transform.position = new Vector3(position.x, position.y, position.z);
            gameDataManager.CreateMap(levelName);
            SetMapData();
            if (player != null) player.SetActive(true);
        }
    }

    // 맵을 로드할 때에 한 번 배열을 가져오는 메서드로 따로 사용하면 안 됨
    // 기존 맵 배열을 사용하는 것이 아니라 인게임용 배열을 가지고 검출, 수정 등을 할 예정 
    private void SetMapData()
    {
        currentObjects = (GameObject[,,])gameDataManager.InitObjects.Clone();
        //이전배열 == currentOBJECTS 배열을 만드는 것을 추가 했습니다.
        UpdatePrevBlockObjs();
        currentTiles = (GameObject[,,])gameDataManager.InitTiles.Clone();

        maxX = currentObjects.GetLength(0) - 1;
        maxY = currentObjects.GetLength(1) + 2;
        maxZ = currentObjects.GetLength(2) - 1;

        tileMaxX = currentTiles.GetLength(0) - 1;
        tileMaxY = currentTiles.GetLength(1) - 1;
        tileMaxZ = currentTiles.GetLength(2) - 1;
    }

    private void FixedUpdate()
    {
        //StartDetector();
    }

    // 전체 오브젝트 순회 검사 후 인터렉션 순차적으로 실행
    [ContextMenu("StartDetector")]
    public void StartDetector()
    {
        List<Dictionary<GameObject, List<IAdjective>>> interactions = IterateThroughMap();
        if (interactions == null || interactions.Count == 0) return;
        for (int i = 0; i < interactions.Count; i++)
        {
            int idx = 0;
            GameObject interactor = null;
            foreach (GameObject go in interactions[i].Keys)
            {
                if (idx == 0)
                {
                    interactor = go;
                    idx++;
                    continue;
                }

                foreach (IAdjective adj in interactions[i][go])
                {
                    if (interactor == null)
                    {
                        continue;
                    }
                    adj.Execute(go.GetComponent<InteractiveObject>(), interactor.GetComponent<InteractiveObject>());
                }
            }
        }
    }

    public void StartDetector(List<GameObject> changedObjects)
    {
        List<Dictionary<GameObject, List<IAdjective>>> interactions = InteractionDetector(changedObjects);
        if (interactions == null || interactions.Count == 0) return;
        for (int i = 0; i < interactions.Count; i++)
        {
            int idx = 0;
            GameObject interactor = null;
            foreach (GameObject go in interactions[i].Keys)
            {
                if (idx == 0)
                {
                    interactor = go;
                    idx++;
                    continue;
                }

                foreach (IAdjective adj in interactions[i][go])
                {
                    if (interactor == null)
                    {
                        continue;
                    }
                    adj.Execute(go.GetComponent<InteractiveObject>(), interactor.GetComponent<InteractiveObject>());
                }
            }
        }
    }

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
        OnObjectScaleChanged(levelInfos.changeScale, levelInfos.target.transform);
    }

    [ContextMenu("PrintScaledObjects")]
    public void PrintScaledObjects()
    {
        foreach(Vector3 vec in scaleChangedObjects.Keys)
        {
            Debug.Log(vec.x + "," + vec.y + "," + vec.z + " : " + scaleChangedObjects[vec], scaleChangedObjects[vec]);
        }
    }

#endregion
}

// y축으로 둥둥 시에 기존에 y축 length을 넘어버리면, out of range