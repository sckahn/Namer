using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogText : MonoBehaviour
{
    [SerializeField] float activeTime;
    public float curTime;

    public void SetTime()
    {
        curTime = activeTime;
    }

    void OnEnable()
    {
        SetTime();
    }

    void Update()
    {
        curTime -= Time.unscaledDeltaTime;
        if (curTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
