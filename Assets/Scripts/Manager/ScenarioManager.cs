using System.Collections;
using System.Collections.Generic;
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
    public Vector3 requirePos;
    public Vector3 targetObj;
    public Vector3 nextPos;
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

    private void Start()
    {
        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        foreach (Scenario scenario in scenarioList)
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
                    Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenario.targetObj);
                    Vector3 vec = Vector3Int.FloorToInt(curScenario.targetObj);
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
        GameObject target = DetectManager.GetInstance.GetArrayObjects(curScenario.targetObj)[curScenario.targetObj];
        DetectManager.GetInstance.SwapBlockInMap(curScenario.targetObj, curScenario.nextPos);
        target.transform.position = curScenario.nextPos;

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
        Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenario.targetObj);
        if (objDict.Keys.Count <= 0) return null;
        Vector3 vec = Vector3Int.FloorToInt(curScenario.targetObj);
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
                    if (playerPos == curScenario.requirePos)
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
