using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GameStates
{
    Lobby,
    InGame,
    Pause,
    Lose,
    Victory,
    Encyclopedia
}

public class GameManager : Singleton<GameManager>
{
    #region GameStates
    public GameStates CurrentState { get; private set; }
    private GameStates previousState;
    #endregion

    #region Player variable
    public PlayerEntity localPlayerEntity;
    public bool isPlayerDoAction; // Action = PlayerInteraction + Addcard
    public bool isPlayerCanInput;
    #endregion

    #region Camera variable
    [Header("Camera Variable")]
    public CameraController cameraController;
    public bool canSwitchCam;
    #endregion

    #region Scenario variable
    public ScenarioController scenarioController;
    #endregion

    #region Input Delegate

    public Action KeyAction;
    #endregion
    
    #region TestKey Settings
    [Header("Key Settings")] 
    public KeyCode restartKey;
    public KeyCode interactionKey;
    public KeyCode showNameKey;
    public KeyCode pauseKey;
    public KeyCode cameraKey;
    public KeyCode cardToggleKey;
    #endregion

    [Header("Manager Prefabs")]
    [SerializeField] private List<GameObject> managerPrefabs;

    public float curTimeScale { get; private set; }
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
    }
    
    private void Init()
    {
        #region StateMachine Runner
        this.gameObject.AddComponent<StateMachineRunner>();
        #endregion

        #region GameStates
        CurrentState = GameStates.Lobby;
        previousState = CurrentState;
        #endregion
        
        #region Player variable
        isPlayerDoAction = false;
        isPlayerCanInput = true;
        #endregion

        #region Instantiate Managers
        if (managerPrefabs != null)
        {
            foreach (var var in managerPrefabs)
            {
                if (var &&
                    !GameObject.Find(var.name))
                {
                    if (!var.name.Contains("Manager"))
                    {
                        Debug.LogError("Prefab 이름을 확인해 주세요. 매니저 인스턴스는 \"Manager\" 단어가 포함되어야 합니다.");
                    }

                    else
                    {
                        GameObject singleton = Instantiate(var);
                        singleton.name = var.name;
                        DontDestroyOnLoad(singleton);
                    }
                }
            }
        }
        #endregion
        
        #region InputKey & KeyAction Delegate Initialize 
        restartKey = KeyCode.R;
        interactionKey = KeyCode.Space;
        showNameKey = KeyCode.Tab;
        pauseKey = KeyCode.Escape;
        cameraKey = KeyCode.Q;
        cardToggleKey = KeyCode.E;
        KeyAction = null;
        #endregion

        #region Get User and Level Data & Set UserID "111111"
        GameDataManager.GetInstance.GetUserAndLevelData();
        GameDataManager.GetInstance.AddUserData("111111");
        #endregion

        SetTimeScale(1);
    }

    private void Start()
    {
        KeyAction += Reset;
    }

    public void SetTimeScale(float timeScale)
    {
        curTimeScale = timeScale;
        Time.timeScale = timeScale;
    }

    #region ResetUIVariable
    private Coroutine loadingCoroutine;
    private Coroutine subLoadingCoroutine;
    private float resetLoadValue = 0f;
    [Range(0.1f,0.9f)] float fillSpeed = 0.5f;
    #endregion
    
    private void Update()
    {


        DetectInputkey();
        #region Exceptions
        if ((int)(Time.timeScale * 10000) != (int)(curTimeScale * 10000))
        {
            Debug.LogError("GameManager의 SetTimeScale() 함수를 통해 TimeScale을 변경해주세요.");
        }
        #endregion
        
    }
    
    public void DetectInputkey()
    {
        if (KeyAction != null)
        {
            KeyAction.Invoke();
        }
    }
    
    
    private void UpdateGameState()
    {
        switch (CurrentState)
        {
            case GameStates.Lobby: 
                HandleLobby();
                break;
            case GameStates.InGame:
                HandleInGame();
                break;
            case GameStates.Pause:
                break;
            case GameStates.Victory:
                HandleVictory();
                break;
            case GameStates.Lose:
                HandleLost();
                break;
            case GameStates.Encyclopedia:
                HandleEncyclopedia();
                break;
        }
    }
    public void ChangeGameState(GameStates newState)
    {
        if (CurrentState == newState)
        {
            UpdateGameState();
            Debug.Log("바꾸려는 State가 이전의 State와 같습니다. 의도하신 상황이 맞나요?");
            return;
        }
        
        previousState = CurrentState;
        CurrentState = newState;
        UpdateGameState();
    }

    public void ReturnPreviousState()
    {
        CurrentState = previousState;
        UpdateGameState();
    }

    private void HandleEncyclopedia()
    {
        scenarioController.LogOnOff(false);
    }

    private void HandleLost()
    {
        Debug.Log("You Lost");
        //menu for reset
        throw new NotImplementedException();
    }

    private void HandleVictory()
    {
        Debug.Log("You win");
        // show viectory ui
        // menu for next lever or quit
    }

    private void HandleInGame()
    {
        //LoadMap();
        //load new scene 
        //LoadScene(Scenes.InGame,LoadSceneMode.Single);
        //instantiate player
        //instantiate objects
        //instantiate cards

        scenarioController.LogOnOff(true);
    }

    void HandleLobby()
    {
        // menu 
        // start game 
    }
    
    public void Reset()
    {
        if(CurrentState != GameStates.InGame) return;
        if (Input.GetKeyDown(restartKey))
        {
            UIManager.GetInstance.ingameCanvas.GetComponent<IngameCanvasController>().TurnOnAndOffLoadingImg(true);
            if(subLoadingCoroutine != null)
                StopCoroutine(subLoadingCoroutine);
            loadingCoroutine = StartCoroutine(AddResetLoad());
        }

        if (Input.GetKeyUp(restartKey))
        {
            StopCoroutine(loadingCoroutine);
            subLoadingCoroutine = StartCoroutine(SubResetLoad());
        }
    }

    public void LoadScene(Scenes scenes, LoadSceneMode loadSceneMode)
    {
        SceneBehaviorManager.LoadScene(scenes,loadSceneMode);
    }
    
    public void Win()
    {
        ChangeGameState(GameStates.Victory);
    }

    public void Lost()
    {
        ChangeGameState(GameStates.Lose);
    }
     #region JSCODE

    int curLevel =-3;
    public int Level { get { return curLevel; } }
    private GameObject groundObjs;
    private GameObject objcts;
    public string userId = "000000";

     GameObject player;
    void LoadMap(int level)
    {
        // DetectManager.GetInstance.Init(level);
        DetectManager.GetInstance.Init(level);
    }

    int GetCurrentLevel()
    {
        var gameDataManager = GameDataManager.GetInstance;
        gameDataManager.GetUserAndLevelData();
        curLevel = gameDataManager.UserDataDic[userId].clearLevel;
        return curLevel;
    }

    int GetCurrentLevel(int level)
    {
        curLevel = level;
        return curLevel;
    }

    public void SetLevelFromCard(string CardName)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var letter in CardName)
        {
            if (letter >= '0' && letter <= '9')
            {
                sb.Append(letter);
            }
        }
        int level = int.Parse(sb.ToString()) - 1;
        //testCode
        // level = 1;        
        GetCurrentLevel(level);
    }

    [ContextMenu("DeleteCurrentMap Test")]
    void DeleteCurrentMap()
    {
        groundObjs = GameObject.Find("Grounds");
        objcts = GameObject.Find("Objects");
        if (groundObjs != null && objcts != null)
        {
            Destroy(groundObjs);
            Destroy(objcts);
        }
    }

    [ContextMenu("DeleteCard")]
    void DeleteCurrentCard()
    {
        GameObject buttons = buttons = GameObject.Find("IngameCanvas").transform.GetChild(1).gameObject;
        buttons.SetActive(false);
        CardManager cardManager = CardManager.GetInstance;
        var currentDeck = cardManager.myCards;
        foreach (var eachCard in currentDeck)
        {
            Destroy(eachCard.gameObject);
        }
        currentDeck.Clear();
    }

    void GetNewCardDeck()
    {
        CardManager.GetInstance.CardStart();
    }

    [ContextMenu("ResetMap")]
    void ResetCurrentLvl()
    {
        if (curLevel == -3)
        {
            Debug.LogError("There are no level Data!!!");
            return;
        }
        DeleteCurrentCard();
        DeleteCurrentMap();
        LoadMap(curLevel);
        GetNewCardDeck();
        cameraController.Init();
        scenarioController.Init();
    }

    
    //DemoScene에서 하면 왜됌?
    //근데 씬불러올때는 안되네;
    [ContextMenu("LoadMapTest")]
    public void LoadMap()
    {
        // LoadPlayerPrefabs();
        if(curLevel == -3)
            curLevel=GetCurrentLevel();
   
        DetectManager.GetInstance.Init(curLevel);
        CardManager.GetInstance.CardStart(); // 여기서 문제네
        scenarioController.Init();
    }
    //load scene with loading card -> get level data from level card

    //TODO Change PlayerPrefabs to Resources or set it to inspector
    // void LoadPlayerPrefabs()
    // {
    //     string prefabFilePath = "Assets/Prefabs/Characters/Player/Player.prefab";
    //     //Assets/Prefabs/Characters/Player/Player.prefab
    //     GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFilePath);
    //      player = Instantiate(prefab);
    //     player.name = "Player";
    // }

    #region SceneTester

    private GameObject levelInfos;

    [ContextMenu("TestLevelLoad")]
    public void ForTester()
    { 
        levelInfos = GameObject.Find("LevelInfos");
       int testlevel= levelInfos.GetComponent<LevelInfos>().LevelNumber;
       ChangeGameState(GameStates.InGame);
       setLevel(testlevel);
       LoadMap();
    }

    void setLevel(int level)
    {
        curLevel = level;
    }


    #endregion



    IEnumerator AddResetLoad()
    {
        while (resetLoadValue < 1f)
        {
            resetLoadValue += Time.deltaTime * fillSpeed;
            UIManager.GetInstance.ingameCanvas.GetComponent<IngameCanvasController>().SetLoadingImage(resetLoadValue);
            yield return null;
        }

        if (resetLoadValue > 1f)
        {
            UIManager.GetInstance.ingameCanvas.GetComponent<IngameCanvasController>().TurnOnAndOffLoadingImg(false);
        }
        ResetCurrentLvl();
        
    }

    IEnumerator SubResetLoad()
    {
        while (resetLoadValue > 0f)
        {
            resetLoadValue -= Time.deltaTime;
            UIManager.GetInstance.ingameCanvas.GetComponent<IngameCanvasController>().SetLoadingImage(resetLoadValue);
            if (resetLoadValue <= 0f)
            {
                UIManager.GetInstance.ingameCanvas.GetComponent<IngameCanvasController>().TurnOnAndOffLoadingImg(false);
            }
            yield return null;
        }
    }
    
    

    #endregion
}
