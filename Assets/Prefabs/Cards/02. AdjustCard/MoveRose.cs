using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRose : MonoBehaviour
{
    Vector3 startPos;
    Vector3 destination;
    private float currentTime;
    public GameObject highLight;

    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(0,0,0);
        destination = startPos + new Vector3(0.2f,0,0);        
    }

    private void Update()
    {
        if (highLight.activeInHierarchy)
        {
            currentTime+=Time.deltaTime;

            transform.localPosition = Vector3.Lerp(new Vector3(-0.5f,0,0), destination, currentTime);
        }

        else
        {
            transform.localPosition = startPos;
            currentTime=0;
        }
            
    }
}
