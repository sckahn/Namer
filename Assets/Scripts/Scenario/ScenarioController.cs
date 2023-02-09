using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

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

public class ScenarioController : MonoBehaviour
{
    [Header("시나리오 추가할 때에 넣는 부분")]
    [SerializeField] Scenario[] scenarioList;
    private static Queue<Scenario> scenarios = new Queue<Scenario>();
    private Scenario curScenario;
    private int scenarioCount = 0;
    private Transform player;
    private CameraController cameraController;
    private float scenarioTime = 20f;

    [Header("필수로 등록해야 하는 부분")]
    [SerializeField] GameObject logBox;
    [SerializeField] Text logText;
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;

    [System.NonSerialized] public bool logOpened = false;
    [System.NonSerialized] public bool dialogOpened = false;
    [System.NonSerialized] public bool isUI = false;

    private void Awake()
    {
        GameManager.GetInstance.scenarioController = this;
    }

    public void LogOnOff(bool isOn)
    {
        switch (isOn)
        {
            case (true):
                logBox.SetActive(logOpened);
                dialogBox.SetActive(dialogOpened);
                break;
            case (false):
                logOpened = logBox.activeSelf;
                dialogOpened = dialogBox.activeSelf;
                logBox.SetActive(false);
                dialogBox.SetActive(false);
                break;
        }
    }

    public void Init()
    {
        curScenario = new Scenario();
        curScenario.type = ERequireType.Null;
        //logBox = GameObject.Find("IngameCanvas").transform.Find("SystemLog").gameObject;
        //logText = logBox.GetComponentInChildren<Text>();
        logBox.SetActive(false);
        logOpened = false;
        //dialogBox = GameObject.Find("IngameCanvas").transform.Find("Dialog").gameObject;
        //dialogText = dialogBox.GetComponentInChildren<Text>();
        dialogBox.SetActive(false);
        dialogOpened = false;

        isUI = false;

        scenarioCount = 0;
        scenarios = new Queue<Scenario>();
        scenarioTime = 20f;

        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        foreach (Scenario scenario in GameDataManager.GetInstance.LevelDataDic[GameManager.GetInstance.Level].scenario)
        {
            scenarios.Enqueue(scenario);
            scenarioCount++;
        }

        player = GameObject.Find("Player").transform;
        NextScenario();
        StartScenario();
    }

    private void SystemLog(string message)
    {
        logBox.SetActive(true);
        logText.text = message;
        logBox.GetComponent<LogText>().SetTime();
    }

    private void LogError(string message)
    {
        string errorText = $"<color=red>{message}</color>";
        SystemLog(errorText);
    }

    private void Log(string message, string objName)
    {
        string dialogMessage = $"<color=red>[{objName}]</color>\n{message}";
        dialogBox.SetActive(true);
        dialogText.text = dialogMessage;
        dialogBox.GetComponent<LogText>().SetTime();
    }

    private void StartScenario()
    {
        scenarioCount = scenarios.Count;
        DoScenario();
    }

    private void NextScenario()
    {
        if (scenarios.Count != 0)
            curScenario = scenarios.Dequeue();
        else
        {
            curScenario = new Scenario();
            curScenario.type = ERequireType.Null;
        }
        scenarioTime = 20f;
    }

    [ContextMenu("SaveNewScenario")]
    public void SaveScenario()
    {
        //테스트 완료 후, JSON 파일 저장하는 함수
        if (scenarioList == null || scenarioList.Length == 0)
        {
            SystemLog("[에러]리스트에 추가할 시나리오를 1개 이상 추가하세요.");
            return;
        }
        SaveLoadFile saveFile = new SaveLoadFile();
        saveFile.CreateJsonFile(scenarioList.ToList(), "Assets/Resources/Data/SaveLoad", "Level0" + GameManager.GetInstance.Level + "Scenario.json");
    }

    private void DoScenario()
    {
        if (cameraController == null) cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        if (curScenario.isDialog)
        {
            if (curScenario.message != null && curScenario.message != "")
            {
                if (curScenario.isFocus)
                {
                    Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
                    Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenarioPos);
                    Vector3 vec = Vector3Int.FloorToInt(curScenarioPos);
                    if (objDict[vec] == null)
                    {
                        LogError("[에러]json의 targetObj x, y, z에 해당하는 \n오브젝트가 없습니다.");
                        return;
                    }
                    cameraController.FocusOn(objDict[vec].transform, false);
                }
                else
                {
                    if (cameraController.isFocused)
                    {
                        cameraController.FocusOff();
                    }
                }

                if (curScenario.message.Contains("[System]"))
                {
                    SystemLog(curScenario.message.Replace("[System]", ""));
                }
                else
                {
                    if (!curScenario.message.Contains(":"))
                    {
                        LogError("[에러]json의 message에 ':'으로 이름과 메세지를 \n구분하세요.");
                        return;
                    }
                    string[] curMessage = curScenario.message.Split(":");
                    Log(curMessage[1], curMessage[0]);
                }
            }
        }

        if (curScenario.funcName != null && curScenario.funcName != "")
        {
            try
            {
                Invoke(curScenario.funcName, 0);
            }
            catch (NullReferenceException e)
            {
                LogError("[에러]json에 실제로 존재하는 함수명을 입력하세요.");
                Debug.LogError(e);
                return;
            }
        }
        else
        {
            NextScenario();
        }
    }

    private void MoveObject()
    {
        Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
        Vector3 nextScenarioPos = new Vector3(curScenario.nextPos.x, curScenario.nextPos.y, curScenario.nextPos.z);
        GameObject target = DetectManager.GetInstance.GetArrayObjects(curScenarioPos)[curScenarioPos];
        DetectManager.GetInstance.SwapBlockInMap(curScenarioPos, nextScenarioPos);
        target.transform.position = nextScenarioPos;

        NextScenario();
    }

    private InteractiveObject GetIObj()
    {
        Vector3 curScenarioPos = new Vector3(curScenario.targetObj.x, curScenario.targetObj.y, curScenario.targetObj.z);
        Dictionary<Vector3, GameObject> objDict = DetectManager.GetInstance.GetArrayObjects(curScenarioPos);
        if (objDict.Keys.Count <= 0) return null;
        Vector3 vec = Vector3Int.FloorToInt(curScenarioPos);
        return objDict[vec].GetComponent<InteractiveObject>();
    }

    private void Update()
    {
        if (GameManager.GetInstance.currentState != GameStates.InGame) return;
        if (scenarioTime > 0) scenarioTime -= Time.deltaTime;
        else
        {
            LogError("R키를 꾹 눌러서 재시작 할 수 있습니다.");
            scenarioTime = 10f;
        }

        if (curScenario.type != ERequireType.Null && scenarioCount != scenarios.Count)
        {
            switch (curScenario.type)
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
                    if (objName == null) return;
                    if (objName.Contains(curScenario.requiredName))
                    {
                        StartScenario();
                    }
                    break;
                case (ERequireType.MouseClick):
                    if (Input.GetMouseButtonDown(0) && !isUI)
                    {
                        StartScenario();
                    }
                    break;
            }
        }
    }
}
