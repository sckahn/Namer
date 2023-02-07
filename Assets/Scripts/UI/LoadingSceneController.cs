using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;
    [SerializeField] Image progressBar;
    [SerializeField] float fakeLoadTime = 0.35f;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");

    }

    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (progressBar.fillAmount < 0.85f)
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0f, 0.85f, timer * fakeLoadTime);
            }
            else if (progressBar.fillAmount < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else if (progressBar.fillAmount == 1f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
            else if (op.progress == 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer * fakeLoadTime);
            }
        }
    }
}