using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableRose : MonoBehaviour
{
    public GameObject highLight;
    public GameObject fireEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (highLight.activeInHierarchy)
        {
            fireEffect.SetActive(true);
        }

        else
        {
            fireEffect.SetActive(false);
        }
    }
}
