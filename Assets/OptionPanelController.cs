using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    public Dropdown maxFrameDropdown;
    public Toggle fullscreenToggle;

    void Start()
    {
        ResolutionInit();
        MaxFrameInit();
        fullscreenToggle.isOn = Screen.fullScreen;
        withOutBorderfullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
    }

    private void MaxFrameInit()
    {
        maxFrameDropdown.RefreshShownValue();
        maxFrameDropdown.value =
            GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.maxFrame;
    }

    private void ResolutionInit()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetMaxFrame(int maxFramIndex)
    {

    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public Toggle withOutBorderfullscreenToggle;


     
    public void SetWithOutBorderFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
