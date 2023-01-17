using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    Lobby,
    InGame,
    Lose,
    Victory
}


public class GameManager : Singleton<GameManager>
{
    private GameStates state;
    public bool isTapDown = false;
    private CheckSurrounding checkSurrounding;

    public CheckSurrounding GetCheckSurrounding
    {
        get
        {
            if (gameObject.GetComponent<CheckSurrounding>() == null)
            {
                gameObject.AddComponent<CheckSurrounding>();
                checkSurrounding = gameObject.GetComponent<CheckSurrounding>();
            }
            return checkSurrounding;
        }
    }

    private void Start()
    {
        this.gameObject.AddComponent<StateMachineRunner>();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.R))
            Reset();
        TapKeyCheck();
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
        throw new NotImplementedException();
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTapDown = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isTapDown = false;
        }
    }

}
