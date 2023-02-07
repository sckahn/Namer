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

    Vector3 mainCamPos;

    // 둘 다 게임 매니저에서 관리 필요
    bool topViewOn = false;
    int normalCamPirority;
    Transform player;

    // 키 값도 인풋 매니저나 게임 매니저에서 관리 필요 -> 후에 컨트롤러 설정에서 쉽게 변경하도록 하기 
    [SerializeField] KeyCode cameraKey;

    // 카메라들 프리팹에서 넣어놓기 
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
        mainCamPos = Camera.main.transform.position;
        player = GameObject.Find("Player").transform;
        playerNormalViewCam.Follow = player;
        playerTopViewCam.Follow = player;
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

        if (mainCamPos != Camera.main.transform.position)
        {
            mainCamPos = Camera.main.transform.position;
            //CardManager.GetInstance.CardAlignment(0f);
        }

        // 토글로 탑뷰 하기
        if (Input.GetKeyDown(cameraKey))
        {
            topViewOn = !topViewOn;
            playerTopViewCam.Priority = (topViewOn ? (int)PriorityOrder.FrontByNormal : (int)PriorityOrder.BehingByNormal);
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
