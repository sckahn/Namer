using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject pauseUIPanel;

    public bool isPause;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UIOnOff();
    }

    void UIOnOff()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPause)
        {
            pauseUIPanel.SetActive(true);
            isPause = true;
            GameManager.GetInstance.SetTimeScale(0);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPause )
        {
            pauseUIPanel.SetActive(false);
            isPause = false;
            GameManager.GetInstance.SetTimeScale(1);
        }
    }
}
