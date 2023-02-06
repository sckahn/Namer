using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    enum PriorityOrder
    {
        BehindAtAll = 8,
        BehingByNormal,
        normal,
        FrontByNormal,
        FrontAtAll
    }

    bool isOn = false;
    int normalCamPirority;

    [SerializeField] KeyCode cameraKey;
    [SerializeField] CinemachineVirtualCamera playerTopViewCam;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCam;
    [SerializeField] CinemachineVirtualCamera targetCam;

    // test
    [SerializeField] Transform targetObj;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (cameraKey == KeyCode.None) cameraKey = KeyCode.Q;
        playerNormalViewCam.Priority = (int)PriorityOrder.normal;
        playerTopViewCam.Priority = (int)PriorityOrder.BehingByNormal;
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        normalCamPirority = playerNormalViewCam.Priority;
    }

    public void FocusOn(Transform target)
    {
        targetCam.LookAt = target;
        targetCam.Follow = target;
        targetCam.Priority = (int)PriorityOrder.FrontAtAll;
    }

    public void FocusOff()
    {
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        targetCam.LookAt = null;
        targetCam.Follow = null;
    }

    void Update()
    {
        // 누르고 있는 동안 탑뷰로 하기 
        //if (isOn != Input.GetKey(cameraKey))
        //{
        //    isOn = Input.GetKey(cameraKey);
        //    playerTopViewCam.Priority = (isOn ? (int)PriorityOrder.FrontByNormal : (int)PriorityOrder.BehingByNormal);
        //}

        // 토글로 탑뷰 하기
        if (Input.GetKeyDown(cameraKey))
        {
            isOn = !isOn;
            playerTopViewCam.Priority = (isOn ? (int)PriorityOrder.FrontByNormal : (int)PriorityOrder.BehingByNormal);
        }
    }

#region Test
    [ContextMenu("FocusOn")]
    public void TestFocusOn()
    {
        if (targetObj == null) return;
        FocusOn(targetObj);
    }

    [ContextMenu("FocusOff")]
    public void TestFocusOff()
    {
        FocusOff();
    }
    #endregion
}
