using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
  Lobby,
  InGame
}

public static class SceneBehaviorManager 
{
  private static int currentScene;
  private static bool isAbleToLoadScene = true;

  static void OnLoadedScene(Scene scene, LoadSceneMode loadSceneMode)
  {
    // currentScene = GetSceneIndex((Scenes)scene.buildIndex);
    SceneManager.sceneLoaded -= OnLoadedScene;
    SceneManager.SetActiveScene(scene);
    isAbleToLoadScene = true;
  }

  static IEnumerator AsyncLoadScene(Scenes scene, LoadSceneMode sceneMode)
  {
    if (!isAbleToLoadScene)
    {
      yield break;
    }
    isAbleToLoadScene = false;
    SceneManager.sceneLoaded += OnLoadedScene;
    var process = SceneManager.LoadSceneAsync(GetSceneIndex(scene), sceneMode);
    currentScene = GetSceneIndex(scene);
    
    yield return new WaitUntil(() => process.isDone);
  }

  static int GetSceneIndex(Scenes scene)
  {
    return (int)scene;
  }
  
  public static void LoadScene(Scenes scene, LoadSceneMode sceneMode)
  {
    GameManager.GetInstance.StartCoroutine(AsyncLoadScene(scene, sceneMode));
  }
  
  public static void ResetScene()
  {
    GameManager.GetInstance.StartCoroutine(AsyncLoadScene((Scenes)currentScene, LoadSceneMode.Single));
  }
}
