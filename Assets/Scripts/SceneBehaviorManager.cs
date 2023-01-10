using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
  Lobby,
  InGame
}

public class SceneBehaviorManager : MonoBehaviour
{
  private static SceneBehaviorManager _sceneBehaviorManagerInstance;
  public static SceneBehaviorManager GetSceneBehaviorManagerInstance { get { return _sceneBehaviorManagerInstance; } }
  private int currentScene;

  private void Awake()
  {
    _sceneBehaviorManagerInstance = this;
    DontDestroyOnLoad(this);
  }
  public void LoadScene(Scenes scene)
  {
    int sceneIndex = (int)scene;
    SceneManager.LoadScene(sceneIndex);
  }

  public void ResetScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
