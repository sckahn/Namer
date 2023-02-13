using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EncyclopediaController : MonoBehaviour
{
    [SerializeField] GameObject layoutGroup;
    [SerializeField] float wheelSpeed = 0.1f;
    [SerializeField] float maxHeight = 1f;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] UnityEngine.UI.Button returnBtn;
    GameObject[] pediaCards;
    IngameCanvasController canvasController;

    GameDataManager gameDataManager;

    private void Start()
    {
       Init();
    }

    void Update()
    {
        ScrollWheel();
    }

    private void Init()
    {
        gameDataManager = GameDataManager.GetInstance;
        gameDataManager.GetCardData();
        gameDataManager.GetUserAndLevelData();
        EncyclopediaInit();

        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            canvasController = GameObject.Find("IngameCanvas").gameObject.GetComponent<IngameCanvasController>();
            returnBtn.onClick.AddListener(canvasController.EncyclopediaClose);
        }
    }

    private void EncyclopediaInit()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            pediaCards = GameDataManager.GetInstance.GetMainCardEncyclopedia("000000");
        }
        else
        {
            pediaCards = GameDataManager.GetInstance.GetIngameCardEncyclopedia(GameManager.GetInstance.Level);
        }

        maxHeight = 0.5f + (float) 0.5 * (pediaCards.Length / 4);

        for (int i = 0; i < pediaCards.Length; i++)
        {
            var cardObject = (GameObject)Instantiate(pediaCards[i], new Vector3(0, 0, 0), Quaternion.identity);
            cardObject.transform.parent = GameObject.Find("LayoutCards").transform;
            cardObject.transform.localPosition =
                new Vector3(-0.9f + (0.6f * (i % 4)),
                -0.7f * (int) (i / 4), 0);
            cardObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        }
    }

    public void SyncScrollBar()
    {
        layoutGroup.transform.localPosition =
            new Vector3(0f, scrollbar.value * maxHeight, 0f);
    }

    private void ScrollWheel()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            layoutGroup.transform.localPosition -= new Vector3(0, wheelSpeed, 0);
            scrollbar.value = layoutGroup.transform.localPosition.y / maxHeight;
            if (layoutGroup.transform.localPosition.y <= 0f)
            {
                layoutGroup.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        else if (wheelInput < 0)
        {
            layoutGroup.transform.localPosition += new Vector3(0, wheelSpeed, 0);
            scrollbar.value = layoutGroup.transform.localPosition.y / maxHeight;
            if (layoutGroup.transform.localPosition.y >= maxHeight)
            {
                layoutGroup.transform.localPosition = new Vector3(0, maxHeight, 0);
            }
        }
    }
}
