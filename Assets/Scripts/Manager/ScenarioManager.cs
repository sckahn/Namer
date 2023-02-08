using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ERequireType
{
    Null = -1,
    PlayerPos,
    AddCard,
    MouseClick
}

[System.Serializable]
public struct Scenario
{
    public ERequireType type;
    public SPosition requirePos;
    public SPosition targetObj;
    public SPosition nextPos;
    public string requiredName;
    public bool isDialog;
    public string message;
    public string funcName;
    public bool isFocus;
}

public class ScenarioManager : Singleton<ScenarioManager>
{
    public Scenario[] scenarioList;
    public Queue<Scenario> scenarios = new Queue<Scenario>();
    public Scenario curScenario;
    private int scenarioCount = 0;
    private Transform player;
    private CameraController cameraController;

    public void InitScenario()
    {
        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        foreach (Scenario scenario in GameDataManager.GetInstance.LevelDataDic[GameManager.GetInstance.Level].scenario)
        {
            scenarios.Enqueue(scenario);
            scenarioCount++;
        }

        player = GameObject.Find("Player").transform;
        if (scenarios.Count != 0)
        {
            curScenario = scenarios.Dequeue();
        }
        else
        {
            curScenario = new Scenario();
            curScenario.type = ERequireType.Null;
        }

        // 테스트 완료 후, JSON 파일 저장하는 함수
        // SaveLoadFile saveFile = new SaveLoadFile();
        // saveFile.CreateJsonFile(scenarioList.ToList(), "Assets/Resources/Data/SaveLoad", "Level0" + GameManager.GetInstance.Level + "Scenario.json");
    }

    public void Init()
    {
        scenarioCount = 0;
        scenarios = new Queue<Scenario>();
    }

    private void DoScenario()
    {
        if (cameraController == null) cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        if (curScenario.isDialog)
        {
            if (curScenario.message != null)
            {
                if (curScenario.isFocus)
                { 
                    Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
                    Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenarioPos);
                    Vector3 vec = Vector3Int.FloorToInt(curScenarioPos);
                    cameraController.FocusOn(objDict[vec].transform);
                }
                Debug.Log(curScenario.message);
            }
        }

        if (curScenario.funcName != null && curScenario.funcName != "")
        {
            Invoke(curScenario.funcName, 0);
        }
        else
        {
            if (scenarios.Count != 0)
                curScenario = scenarios.Dequeue();
            else
            {
                curScenario = new Scenario();
                curScenario.type = ERequireType.Null;
            }
        }
    }

    private void FocusOff()
    {
        cameraController.FocusOff();

        if (scenarios.Count != 0)
            curScenario = scenarios.Dequeue();
        else
        {
            curScenario = new Scenario();
            curScenario.type = ERequireType.Null;
        }
    }

    private void MoveObject()
    {
        Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
        Vector3 nextScenarioPos = new Vector3(curScenario.nextPos.x, curScenario.nextPos.y, curScenario.nextPos.z);
        GameObject target = DetectManager.GetInstance.GetArrayObjects(curScenarioPos)[curScenarioPos];
        DetectManager.GetInstance.SwapBlockInMap(curScenarioPos, nextScenarioPos);
        target.transform.position = nextScenarioPos;

        if (scenarios.Count != 0)
            curScenario = scenarios.Dequeue();
        else
        {
            curScenario = new Scenario();
            curScenario.type = ERequireType.Null;
        }
    }

    private InteractiveObject GetIObj()
    {
        Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
        Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenarioPos);
        if (objDict.Keys.Count <= 0) return null;
        Vector3 vec = Vector3Int.FloorToInt(curScenarioPos);
        return objDict[vec].GetComponent<InteractiveObject>();
    }

    private void StartScenario()
    {
        scenarioCount = scenarios.Count;
        DoScenario();
    }

    private void FixedUpdate()
    {
        if (curScenario.type != ERequireType.Null && scenarioCount != scenarios.Count)
        {
            switch(curScenario.type)
            {
                case (ERequireType.PlayerPos):
                    Vector3 playerPos = new Vector3(Mathf.Round(player.position.x), Mathf.Round(player.position.y), Mathf.Round(player.position.z));
                    Vector3 requireScenarioPos = new Vector3(curScenario.requirePos.x, curScenario.requirePos.y, curScenario.requirePos.z);
                    if (playerPos == requireScenarioPos)
                    {
                        StartScenario();
                    }
                    break;
                case (ERequireType.AddCard):
                    InteractiveObject tarObj = GetIObj();
                    if (tarObj == null) return;
                    string objName = tarObj.GetCurrentName();
                    if (objName.Contains(curScenario.requiredName))
                    {
                        StartScenario();
                    }
                    break;
                case (ERequireType.MouseClick):
                    if (Input.GetMouseButton(0))
                    {
                        StartScenario();
                    }
                    break;
            }
        }    
    }
}
