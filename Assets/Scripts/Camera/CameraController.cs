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
    CinemachineComponentBase normalcamOption;
    CinemachineComponentBase topcamOption;

    // 카메라들 프리팹에서 넣어놓기
    [Header("Cams")]
    [SerializeField] CinemachineVirtualCamera playerTopViewCam;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCam;
    [SerializeField] CinemachineVirtualCamera targetCam;

    [Header("Zoom settings")]
    [SerializeField] float originDistance = 10f;
    [SerializeField][Range(1f, 10f)] float scrollSpeed = 2.5f;
    [SerializeField][Range(0f, 100f)] float maxZoomIn = 5f;
    [SerializeField][Range(0f, 100f)] float maxZoomOut = 20f;

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

        // 카메라 distance init
        normalcamOption = playerNormalViewCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        topcamOption = playerTopViewCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        (normalcamOption as CinemachineFramingTransposer).m_CameraDistance = originDistance;
        (topcamOption as CinemachineFramingTransposer).m_CameraDistance = originDistance;

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

    void Update()
    {
        if (GameManager.GetInstance.currentState != GameStates.InGame) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        if (scroll != 0)
        {
            if (scroll < 0)
            {
                if (normalcamOption is CinemachineFramingTransposer)
                {
                    float curScroll = (normalcamOption as CinemachineFramingTransposer).m_CameraDistance;
                    (normalcamOption as CinemachineFramingTransposer).m_CameraDistance = (curScroll + scroll >= maxZoomIn) ? (curScroll + scroll) : maxZoomIn;
                }

                if (topcamOption is CinemachineFramingTransposer)
                {
                    float curScroll = (topcamOption as CinemachineFramingTransposer).m_CameraDistance;
                    (topcamOption as CinemachineFramingTransposer).m_CameraDistance = (curScroll + scroll >= maxZoomIn) ? (curScroll + scroll) : maxZoomIn;
                }
            }
            else
            {
                if (normalcamOption is CinemachineFramingTransposer)
                {
                    float curScroll = (normalcamOption as CinemachineFramingTransposer).m_CameraDistance;
                    (normalcamOption as CinemachineFramingTransposer).m_CameraDistance = (curScroll + scroll <= maxZoomOut) ? (curScroll + scroll) : maxZoomOut;
                }

                if (topcamOption is CinemachineFramingTransposer)
                {
                    float curScroll = (topcamOption as CinemachineFramingTransposer).m_CameraDistance;
                    (topcamOption as CinemachineFramingTransposer).m_CameraDistance = (curScroll + scroll <= maxZoomOut) ? (curScroll + scroll) : maxZoomOut;
                }
            }
        }
    }
}
