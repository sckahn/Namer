using UnityEngine;

public class CardRotate : MonoBehaviour
{

    [Tooltip("Horizontal Rotation Speed")]
    [Range(-1, 1)]
    [SerializeField] float rotationSpeedH = 0.8f;

    [Tooltip("Horizontal Rotation Speed")]
    [Range(-1, 1)]
    [SerializeField] float rotationSpeedV = 0.6f;

    [Tooltip("Maximum Horizontal Angle")]
    [Range(0, 60)]
    [SerializeField] float angleH = 5;

    [Tooltip("Maximum Vertical Angle")]
    [Range(0, 60)]
    [SerializeField] float angleV = 0;

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
