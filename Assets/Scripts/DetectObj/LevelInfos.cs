using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfos : MonoBehaviour
{
    [SerializeField] bool isCreateMode;
    [SerializeField] string levelName;

    // test
    [SerializeField] public GameObject target;
    [SerializeField] public Dir ECheckDir;
    [SerializeField] public Vector3 block1;
    [SerializeField] public Vector3 block2;
    [SerializeField] public GameObject newValue;

    public bool IsCreateMode { get { return isCreateMode; } }
    public string LevelName { get { return levelName; } }

    private void Awake()
    {
        DetectSurroundingHS.GetInstance.Init(this);
    }
}
