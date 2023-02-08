using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRose : MonoBehaviour
{
    Vector3 startPos;
    Vector3 destination;
    Vector3 curVel = Vector3.zero;
    bool check;
    public GameObject highLight;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        destination = startPos + new Vector3(0.2f,0,0);
        check = true;
    }

    private void Update()
    {
        if (highLight.activeInHierarchy)
        {
            this.gameObject.layer = 5;
            //currentTime+=Time.deltaTime;
            if (check)
            {
                transform.localPosition = new Vector3(-0.3f, 0, 0);
                check = false;
            }
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, destination, ref curVel, 1f);
        }

        else
        {
            this.gameObject.layer = 0;
            transform.localPosition = startPos;
            check = true;
            //currentTime=0;
        }
            
    }
}
