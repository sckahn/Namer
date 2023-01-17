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
    [SerializeField]private float growingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;



    [ContextMenu("objScaling")]
    public void ObjectScaling()
    {
        bool flag = CheckGrowable();
        print(flag);
        if (flag)
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
        else
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
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

  
    private bool CheckGrowable()
    {
        var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(gameObject, 1f);
        print(neighbors.Count);
        foreach (var tt in neighbors)
        {
            print(tt.Value.Length);
        }
        var gameObjects = neighbors[ObjDirection.Up];
        print(gameObjects.Length);
        foreach (var neighborObj in gameObjects)
        {
            if (neighborObj.tag == "InteractObj")
            {
                return false;
            }
        }
        
        return true;
    }



    IEnumerator WrapperCoroutine(bool isGrow)
    {
        if (isGrow)
        {
            SetGrowScale();
            yield return StartCoroutine(ScaleObj());
        }
        else
        {
            SetGrowScale();
            yield return StartCoroutine(ScaleObj());
            SetShrinkScale();
            yield return StartCoroutine(ScaleObj());
        }
    }

   

    IEnumerator ScaleObj()
    {
        currentTime = 0;
        Vector3 startScale = transform.localScale;
        while (currentTime < growingSpeed)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / growingSpeed);
            yield return null;
           
        }
    }
}
