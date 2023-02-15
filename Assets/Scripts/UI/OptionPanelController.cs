using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    Resolution[] resolutions;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown maxFrameDropdown;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle bgToggle;

    SGameSetting sGameSetting = new SGameSetting();

    void OnEnable()
    {
        ResolutionInit();
        MaxFrameInit();
        Init();

    }

    void Init()
    {
        print(GameManager.GetInstance.userId);

        string userID = GameManager.GetInstance.userId;
        foreach (var userData in GameDataManager.GetInstance.UserDataDic)
        {
            print(userData.Value.userID);
            print(userData.Value.gameSetting);
        }

        fullscreenToggle.isOn = Screen.fullScreen;
        withOutBorderfullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        muteToggle.isOn = GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.isMute;
        bgToggle.isOn = !GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.isMuteInBackground;
        masterSlider.value = GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.volume;
        BGMSlider.value = GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.backgroundVolume;
        SFXSlider.value = GameDataManager.GetInstance.
            UserDataDic[GameManager.GetInstance.userId].
            gameSetting.soundEffects;
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
        sGameSetting = GameDataManager.GetInstance.UserDataDic[GameManager.GetInstance.userId].gameSetting;
        sGameSetting.resolution = resolutionIndex;
        GameDataManager.GetInstance.UpdateUserData(false, sGameSetting);
    }

    public void SetMaxFrame(int maxFramIndex)
    {
        sGameSetting = GameDataManager.GetInstance.UserDataDic[GameManager.GetInstance.userId].gameSetting;
        sGameSetting.maxFrame = maxFramIndex;
        GameDataManager.GetInstance.UpdateUserData(false, sGameSetting);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        sGameSetting = GameDataManager.GetInstance.UserDataDic[GameManager.GetInstance.userId].gameSetting;
        sGameSetting.isfullScreen = isFullscreen;
        GameDataManager.GetInstance.UpdateUserData(false, sGameSetting);
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

        sGameSetting = GameDataManager.GetInstance.UserDataDic[GameManager.GetInstance.userId].gameSetting;
        sGameSetting.isborderlessFullScreen = isFullscreen;
        GameDataManager.GetInstance.UpdateUserData(false, sGameSetting);
    }

    public void OptionPanelClose()
    {
        print("Close");
        GameDataManager.GetInstance.UpdateUserData(true);
        this.gameObject.SetActive(false);
    }
}
