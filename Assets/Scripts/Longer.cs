using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longer : MonoBehaviour
{
    [ContextMenu("GrowLonger")]
    public void GrowLonger()
    {
        Vector3 currentScale = transform.localScale;
        float newHeight = currentScale.y * 2;
        Vector3 newScale = new Vector3(currentScale.x, newHeight, currentScale.z);
        transform.localScale = newScale;
    }
}
