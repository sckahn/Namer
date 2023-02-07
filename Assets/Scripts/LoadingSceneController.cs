using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;
    public AsyncOperation op;
    [SerializeField] Image progressBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if(op.progress < 0.5f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.5f, 1f, timer/2);
                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield return null;
                }
            }
        }
        
        Destroy(this.gameObject);
        yield return null;
    }

    private void OnDestroy()
    {
        UIManager.GetInstance.ingameCanvas = GameObject.Find("IngameCanvas");
        UIManager.GetInstance.pauseUIPanel = UIManager.GetInstance.ingameCanvas.transform.GetChild(0).gameObject;
        GameManager.GetInstance.ChangeGameState(GameStates.InGame);
    }
}