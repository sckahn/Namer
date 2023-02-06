using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatRose : MonoBehaviour
{
    public GameObject highLight;
    Vector3 startPos;
    float runningTime;
    // Start is called before the first frame update

    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (highLight.activeInHierarchy)
        {
            runningTime += Time.deltaTime;
            transform.localPosition = startPos + new Vector3(0, Mathf.Sin(runningTime) * 0.1f, 0);
        }

        else
        {
            transform.localPosition = startPos;
            runningTime = 0;
        }
    }
}
