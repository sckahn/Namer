using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameStates
{
    Lobby,
    InGame,
    Pause,
    Lose,
    Victory
}

public class GameManager : Singleton<GameManager>
{
    #region GameStates
    public GameStates currentState { get; private set; }

    private GameStates previousState;

    #endregion

    #region Player variable
    [Header("Player Variable")]
    public bool isPlayerDoInteraction;
    public bool isPlayerCanInput;
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
        currentState = GameStates.Lobby;
        previousState = currentState;
        #endregion
        
        #region Player variable
        isPlayerDoInteraction = false;
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
        KeyAction = null;
        #endregion

        SetTimeScale(1);
    }

    public void SetTimeScale(float timeScale)
    {
        curTimeScale = timeScale;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        if(Input.GetKey(restartKey))
            Reset();

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
        switch (currentState)
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
        }
    }
    public void ChangeGameState(GameStates newState)
    {
        previousState = currentState;
        currentState = newState;
        UpdateGameState();
    }

    public void ReturnPreviousState()
    {
        currentState = previousState;
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
        //LoadScene(Scenes.InGame,LoadSceneMode.Single);
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
        if (currentState == GameStates.Lobby) return;
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
}
