using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerInMeshRenderer : MonoBehaviour
{
    [SerializeField] string sortingLayerName;
    [SerializeField] int sortingOrder;

    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        mesh.sortingLayerName = sortingLayerName;
        mesh.sortingOrder = sortingOrder;
    }

}
