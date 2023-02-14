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
    CinemachineVirtualCamera[] playerTopCams;
    CinemachineVirtualCamera[] playerNormalCams;
    [SerializeField] CinemachineVirtualCamera playerTopViewCam;
    [SerializeField] CinemachineVirtualCamera playerTopViewCamZoomIn;
    [SerializeField] CinemachineVirtualCamera playerTopViewCamZoomOut;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCam;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCamZoomIn;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCamZoomOut;
    [SerializeField] CinemachineVirtualCamera targetCam;

    CinemachineVirtualCamera curCam;

    [Header("Zoom settings")]
    [SerializeField][Range(0, 2)] int zoomValue = 1;
    bool canZoom = true;

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
        playerNormalViewCam.Priority = (int)PriorityOrder.BehingByNormal;
        playerNormalViewCamZoomIn.Priority = (int)PriorityOrder.BehingByNormal;
        playerNormalViewCamZoomOut.Priority = (int)PriorityOrder.BehingByNormal;
        playerTopViewCam.Priority = (int)PriorityOrder.BehingByNormal;
        playerTopViewCamZoomIn.Priority = (int)PriorityOrder.BehingByNormal;
        playerTopViewCamZoomOut.Priority = (int)PriorityOrder.BehingByNormal;
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        normalCamPirority = playerNormalViewCam.Priority;

        playerTopCams = new CinemachineVirtualCamera[] { playerTopViewCamZoomOut, playerTopViewCam, playerTopViewCamZoomIn };
        playerNormalCams = new CinemachineVirtualCamera[] { playerNormalViewCamZoomOut, playerNormalViewCam, playerNormalViewCamZoomIn };

        SetPriority();

        // 모든 팔로우 캠이 플레이어를 따라다니도록 설정 
        player = GameObject.Find("Player").transform;
        playerNormalViewCam.Follow = player;
        playerTopViewCam.Follow = player;

        isTopView = false;
        canZoom = true;

        FocusOff();
    }

    public void FocusOn(Transform target, bool canMove = true)
    {
        targetCam.LookAt = target;
        targetCam.Follow = target;
        targetCam.Priority = (int)PriorityOrder.FrontAtAll;

        // zoom in 상태에서는 카드가 안 보이도록 함 
        CardManager.GetInstance.CardsHide();

        if (!canMove)
        {
            GameManager.GetInstance.isPlayerCanInput = false;
            GameManager.GetInstance.localPlayerEntity.ChangeState(PlayerStates.Idle);
        }

        isFocused = true;
    }

    public void FocusOff()
    {
        targetCam.Priority = (int)PriorityOrder.BehindAtAll;
        targetCam.LookAt = null;
        targetCam.Follow = null;

        // zoom in 상태에서는 카드가 안 보이도록 함 
        CardManager.GetInstance.CardsReveal();

        GameManager.GetInstance.isPlayerCanInput = true;

        isFocused = false;
    }

    private void SetPriority()
    {
        curCam = isTopView ? playerTopCams[zoomValue] : playerNormalCams[zoomValue];
        curCam.Priority = (int)PriorityOrder.normal;
    }

    private void CheckCameraSwitch()
    {
        if (Input.GetKeyDown(GameManager.GetInstance.cameraKey) && GameManager.GetInstance.CurrentState != GameStates.Encyclopedia)
        {
            curCam.Priority = (int)PriorityOrder.BehingByNormal;
            isTopView = !isTopView;
            SetPriority();
        }
    }

    private IEnumerator ZoomInOut()
    {
        canZoom = false;
        curCam.Priority = (int)PriorityOrder.BehingByNormal;
        SetPriority();
        yield return new WaitForSecondsRealtime(0.3f);
        canZoom = true;
    }

    void Update()
    {
        if (GameManager.GetInstance.CurrentState != GameStates.InGame) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0 || !canZoom) return;
        if (scroll < 0) // zoom in
        {
            zoomValue = zoomValue >= 2 ? 2 : zoomValue + 1;
            StartCoroutine(ZoomInOut());
        }
        else            // zoom out
        {
            zoomValue = zoomValue <= 0 ? 0 : zoomValue - 1;
            StartCoroutine(ZoomInOut());
        }
    }
}
