using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    bool isOn = false;
    int normalCamPirority;
    [SerializeField] KeyCode cameraKey;
    [SerializeField] CinemachineVirtualCamera playerTopViewCam;
    [SerializeField] CinemachineVirtualCamera playerNormalViewCam;

    void Awake()
    {
        if (cameraKey == KeyCode.None) cameraKey = KeyCode.Q;
        normalCamPirority = playerNormalViewCam.Priority;
    }

    void Update()
    {
        // 누르고 있는 동안 탑뷰로 하기 
        //if (isOn != Input.GetKey(cameraKey))
        //{
        //    isOn = Input.GetKey(cameraKey);
        //    playerTopViewCam.Priority = normalCamPirority + (isOn ? 1 : -1);
        //}

        // 토글로 탑뷰 하기
        if (Input.GetKeyDown(cameraKey))
        {
            isOn = !isOn;
            playerTopViewCam.Priority = normalCamPirority + (isOn ? 1 : -1);
        }
    }
}
