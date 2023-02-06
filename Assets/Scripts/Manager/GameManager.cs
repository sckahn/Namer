using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameStates
{
    Lobby,
    InGame,
    Lose,
    Victory
}

public enum GetKeyTypes
{
    Toggle = 0,
    GetKeyDown,
    GetKeyUp
}

public class GameManager : Singleton<GameManager>
{
    private GameStates state;
    public bool isTapDown;
    private CheckSurrounding checkSurrounding;

    #region Player variable
    [Header("Player Variable")]
    public bool isPlayerDoInteraction;
    public bool isPlayerCanInput;
    #endregion

    #region TestKey Settings
    [Header("Key Settings")] 
    public KeyCode RestartKey = KeyCode.R;
    public KeyCode interactionKey = KeyCode.Space;
    public KeyCode ShowNameKey = KeyCode.Tab;
    #endregion
    
    [Header("Manager Prefabs")]
    [SerializeField] private List<GameObject> ManagerPrefabs;

    public float curTimeScale { get; private set; }
    
    public CheckSurrounding GetCheckSurrounding
    {
        get
        {
            if (checkSurrounding == null)
            {
                gameObject.AddComponent<CheckSurrounding>();
                checkSurrounding = gameObject.GetComponent<CheckSurrounding>();
            }
            return checkSurrounding;
        }
    }
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        #region StateMachine Runner
        this.gameObject.AddComponent<StateMachineRunner>();
        #endregion

        #region Player variable
        isPlayerDoInteraction = false;
        isPlayerCanInput = true;
        #endregion

        #region Instantiate Managers
        if (ManagerPrefabs != null)
        {
            foreach (var var in ManagerPrefabs)
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
                        GameObject Singleton = Instantiate(var);
                        Singleton.name = var.name;
                        DontDestroyOnLoad(Singleton);
                    }
                }
            }
        }
        #endregion
        
        // TODO InputManager 필요함
        #region InputKey Initialize
        RestartKey = KeyCode.R;
        interactionKey = KeyCode.Space;
        ShowNameKey = KeyCode.Tab;
        #endregion

        isTapDown = false;
        SetTimeScale(1);
    }

    public void SetTimeScale(float timeScale)
    {
        curTimeScale = timeScale;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        if(Input.GetKey(RestartKey))
            Reset();
        TapKeyCheck();

        if ((int)(Time.timeScale * 10000) != (int)(curTimeScale * 10000))
        {
            Debug.LogError("GameManager의 SetTimeScale() 함수를 통해 TimeScale을 변경해주세요.");
        }
        
    }

    private void UpdateGameState()
    {
        switch (state)
        {
            case GameStates.Lobby: 
                HandleLobby();
                break;
            case GameStates.InGame:
                HandleInGame();
                break;
            case GameStates.Victory:
                HandleVictory();
                break;
            case GameStates.Lose:
                HandleLost();
                break;
        }
    }
    public void ChangeGameState(GameStates newState)
    {
        state = newState;
        UpdateGameState();
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
        
        //load new scene 
        LoadScene(Scenes.InGame,LoadSceneMode.Single);
        //instantiate player
        //instantiate objects
        //instantiate cards
    }

    void HandleLobby()
    {
        // menu 
        // start game 
    }
    
    public void Reset()
    {
        if (state == GameStates.Lobby) return;
            SceneBehaviorManager.ResetScene();
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

    //탭 키를 누르면 이름 팝업 토글을 위한 isTapDown의 불값 변경 
    void TapKeyCheck()
    {
        if (Input.GetKeyDown(ShowNameKey))
        {
            isTapDown = true;
            Debug.Log(" Get Key Down ");
        }

        if (Input.GetKeyUp(ShowNameKey))
        {
            isTapDown = false;
        }
    }
}
