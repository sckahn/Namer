using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longer : MonoBehaviour
{
    private int growScale = 2;
    private float shrinkScale = 0.5f;
    private float goalScale;
    private float currentHeight;
    private float smoothTime = 2f;
    private float currentTime;
    private Vector3 targetScale;

    private bool canGrow;



    [ContextMenu("objScaling")]
    public void ObjectScaling()
    {
        bool flag = CheckGrowable();
        print(flag);
        if (CheckGrowable())
        {
            GrowLonger();
        }
        else
        {
            GrowLonger();
            ShrinkObj();
        }
    }
    public void GrowLonger()
    {
        SetGrowScale();
        var scale =StartCoroutine(ScaleObj());

    }
    public void ShrinkObj()
    {
        SetShrinkScale();
        StartCoroutine(ScaleObj());
    }

    private void SetGrowScale()
    {
        goalScale = transform.localScale.y * growScale;
        targetScale = new Vector3(transform.localScale.x, goalScale, transform.localScale.z);
        currentHeight = transform.localScale.y;
    }

    private void SetShrinkScale()
    {
        goalScale = transform.localScale.y * shrinkScale;
        targetScale = new Vector3(transform.localScale.x, goalScale, transform.localScale.z);
        currentHeight = transform.localScale.y;
    }

    private void Update()
    {
        Debug.DrawLine(transform.position,new Vector3(transform.position.x,transform.position.y+1.5f,transform.position.z),Color.red);
    }

    public bool CheckGrowable()
    {
        RaycastHit raycastHit;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        Ray ray = new Ray(transform.position, Vector3.up);
        if (Physics.Raycast(ray, out raycastHit, transform.position.y+1.5f))
        {
            print("hey");
            if (raycastHit.transform.gameObject.tag == "InteractObj")
            {
                print("yo");
                return false;
            }
        }
        return true;
    }

   

    IEnumerator ScaleObj()
    {
        currentTime = 0;
        Vector3 startScale = transform.localScale;
        while (currentTime < smoothTime)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / smoothTime);
            yield return null;
            // currentHeight += 0.1f;
            // Vector3 newScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
            // transform.localScale = newScale;
            // yield return new WaitForSeconds(0.1f);
        }
    }
}
