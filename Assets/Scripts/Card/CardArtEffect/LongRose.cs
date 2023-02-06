using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRose : MonoBehaviour
{
    public GameObject highLight;
    Vector3 startPos;
    Vector3 startScale;
    Transform parentObj;
    // Start is called before the first frame update
    void Start()
    {
        parentObj = transform.parent;
        startPos = this.transform.localPosition;
        startScale = parentObj.localScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (highLight.activeInHierarchy)
        {
            //transform.localPosition = new Vector3(0, 0, -0.1f);
            //transform.localRotation = Quaternion.Euler(0, 0, 45);
            StartCoroutine(growRose());
                       
        }

        else
        {
            transform.localPosition = startPos;
            parentObj.localScale = startScale;
            
            //currentTime=0;
        }
    }

    IEnumerator growRose()
    {
        while (parentObj.localScale.y < 1.7f)
        {
            parentObj.localScale += new Vector3(0, 0.001f, 0);

            yield return null;
        }
        
    }
}
