using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyRose : MonoBehaviour
{
    public float bounceSpeed = 3.0f;
    public float gravity = 9.8f;

    private Vector3 velocity = Vector3.zero;
    Vector3 startPos;

    public GameObject highLight;

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
            velocity.y -= gravity * Time.deltaTime;
            transform.localPosition += velocity * Time.deltaTime;

            if (transform.localPosition.y < 0)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                velocity.y = bounceSpeed;
            }
        }

        else
        {
            transform.localPosition = startPos;
        }
    }
}
