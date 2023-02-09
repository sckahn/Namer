using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour
{
    [SerializeField] bool existXButton;
    [SerializeField] float activeTime;
    [SerializeField] GameObject button;
    public float curTime;

    public void SetTime()
    {
        curTime = activeTime;
    }

    void Start()
    {
        if (button == null || !existXButton)
        {
            return;
        }
        button.SetActive(true);
        button.GetComponent<Button>().onClick.AddListener(() => this.gameObject.SetActive(false));
    }

    void OnEnable()
    {
        SetTime();
    }

    void Update()
    {
        if (existXButton) return;

        curTime -= Time.unscaledDeltaTime;
        if (curTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
