using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRotate : MonoBehaviour
{
    [Tooltip("Material Shader Mask Number")]
    public int maskNumber = 1;

    [Tooltip("Horizontal Rotation Speed")]
    [Range(-1, 1)]
    public float rotationSpeedH = 0.8f;

    [Tooltip("Horizontal Rotation Speed")]
    [Range(-1, 1)]
    public float rotationSpeedV = 0.6f;

    [Tooltip("Maximum Horizontal Angle")]
    [Range(0, 60)]
    public float angleH = 5;

    [Tooltip("Maximum Vertical Angle")]
    [Range(0, 60)]
    public float angleV = 0;

    private float rotationCounter = 0;

    Vector3 originRotation;

    private void OnEnable()
    {
        originRotation = this.gameObject.transform.rotation.eulerAngles;
    }

    void Update()
    {
        rotationCounter += Time.deltaTime;
        transform.eulerAngles = originRotation + 
             new Vector3(Mathf.Sin(rotationCounter * rotationSpeedV) * angleV, Mathf.Sin(rotationCounter * rotationSpeedH) * angleH, 0);
    }
}
