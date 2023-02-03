using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfos : MonoBehaviour
{
    [Header("MODE")]
    [SerializeField] bool isCreateMode;
    [Range(-1, 10)][SerializeField] int levelNumber;

    // test
    [Header("TEST")]
    [SerializeField] public GameObject target;
    [SerializeField] public Dir ECheckDir;
    [SerializeField] public Vector3 block1;
    [SerializeField] public Vector3 block2;
    [SerializeField] public GameObject newValue;
    [SerializeField] public Vector3 changeScale;
    [SerializeField] public bool isStretched;

    public bool IsCreateMode { get { return isCreateMode; } }
     public int LevelNumber { get { return levelNumber; } set { levelNumber = value; } }

    private void Awake()
    {
        DetectManager.GetInstance.Init(levelNumber);
    }
}
