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

    // 둘 다 게임 매니저에서 관리 필요
    bool isTopView = false;
    public bool isFocused = false;
    int normalCamPirority;
    Transform player;

    // 카메라들 프리팹에서 넣어놓기 
    [SerializeField] CinemachineVirtualCamera playerTopViewCam;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCam;
    [SerializeField] CinemachineVirtualCamera targetCam;

    // test
    [SerializeField] Transform targetObj;

    void Awake()
    {
        GameManager.GetInstance.KeyAction += CheckCameraSwitch;
        if (GameManager.GetInstance.cameraController == null)
            GameManager.GetInstance.cameraController = GameObject.Find("Cameras").GetComponent<CameraController>();
        Init();
    }

    public void Init()
    {
        // 각 캠의 우선순위 설정 
        playerNormalViewCam.Priority = (int)PriorityOrder.normal;
        playerTopViewCam.Priority = (int)PriorityOrder.BehingByNormal;
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        normalCamPirority = playerNormalViewCam.Priority;

        // 모든 팔로우 캠이 플레이어를 따라다니도록 설정 
        player = GameObject.Find("Player").transform;
        playerNormalViewCam.Follow = player;
        playerTopViewCam.Follow = player;

        isTopView = false;

        FocusOff();
    }

    public void FocusOn(Transform target, bool canMove = true)
    {
        targetCam.LookAt = target;
        targetCam.Follow = target;
        targetCam.Priority = (int)PriorityOrder.FrontAtAll;

        // todo 카드가 잠시 안 보이도록 변경

        if (!canMove)
        {
            GameManager.GetInstance.isPlayerCanInput = false;
        }

        isFocused = true;
    }

    public void FocusOff()
    {
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        targetCam.LookAt = null;
        targetCam.Follow = null;
        GameManager.GetInstance.isPlayerCanInput = true;

        isFocused = false;
    }

    private void CheckCameraSwitch()
    {
        if (Input.GetKeyDown(GameManager.GetInstance.cameraKey) && GameManager.GetInstance.currentState != GameStates.Encyclopedia)
        {
            isTopView = !isTopView;
            playerTopViewCam.Priority = (isTopView ? (int)PriorityOrder.FrontByNormal : (int)PriorityOrder.BehingByNormal);
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
