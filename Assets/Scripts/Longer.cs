using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longer : MonoBehaviour
{
    private int growScale = 2;
    private float goalScale;
    private float currentHeight;
    [ContextMenu("GrowLonger")]
    public void GrowLonger()
    {
        SetGoalScale();
        StartCoroutine(Grow());
        
    }

    public void SetGoalScale()
    {
        goalScale = transform.localScale.y * growScale;
        currentHeight = transform.localScale.y;
    }

    IEnumerator Grow()
    {
        while (currentHeight <= goalScale)
        {
            currentHeight += 0.1f;
            Vector3 newScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
            transform.localScale = newScale;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
