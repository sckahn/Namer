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
    [SerializeField]private float ScalingSpeed = 1f;
    private float currentTime;
    private Vector3 targetScale;



    [ContextMenu("objScaling")]
    public void ObjectScaling()
    {
        bool flag = CheckGrowable();
        if (flag)
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
        else
        {
            StartCoroutine(WrapperCoroutine(flag));
        }
    }
    public void GrowLonger()
    {
        SetGrowScale();
        StartCoroutine(ScaleObj());

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
        // Debug.DrawLine(transform.position,new Vector3(transform.position.x,transform.position.y+transform.lossyScale.y+0.5f,transform.position.z),Color.red);
    }

    // public Vector3 getTopVerticies()
    // {
    //     Vector3 topVertices;
    //     MeshFilter meshFilter = GetComponent<MeshFilter>();
    //     
    //     // Get the Mesh object from the MeshFilter
    //     Mesh mesh = meshFilter.mesh;
    //     
    //     // Get the array of vertices from the Mesh
    //     Vector3[] vertices = mesh.vertices;
    //     
    //     // Get the array of normals from the Mesh
    //     Vector3[] normals = mesh.normals;
    //
    //     // Find the top vertices
    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         if (normals[i].y > 0.9f)
    //         {
    //             // This vertex is considered as top, you can do what you want with it
    //             Debug.Log("Top vertex found at: " + vertices[i]);
    //             topVertices = vertices[i];
    //         }
    //     }
    //     return topVertices;
    // }
    public bool CheckGrowable()
    {
        RaycastHit raycastHit;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y+transform.lossyScale.y+ .5f, transform.position.z);
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y+transform.lossyScale.y, transform.position.z);
        Ray ray = new Ray(startPos, Vector3.up);
        if (Physics.Raycast(ray, out raycastHit, .5f))
        {
            if (raycastHit.transform.gameObject.tag == "InteractObj")
            {
                return false;
            }
        }
        return true;
    }

    // public void grow()
    // {
    //     Vector3 startScale = transform.localScale;
    //     Vector3 vel;
    //     // transform.localScale = new Vector3(startScale.x,Mathf.SmoothDamp(startScale.y, targetScale.y, ref vel.y, smoothTime),startScale.z);
    // }

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
        while (currentTime < ScalingSpeed)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / ScalingSpeed);
            yield return null;
            // currentHeight += 0.1f;
            // Vector3 newScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
            // transform.localScale = newScale;
            // yield return new WaitForSeconds(0.1f);
        }
    }
}
