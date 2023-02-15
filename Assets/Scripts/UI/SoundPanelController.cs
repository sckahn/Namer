using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundPanelController : MonoBehaviour
{
    AudioMixer audioMixer;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle bgToggle;

    void Start()
    {
        Init();   
    }

    private void Init()
    {
        SoundManager.GetInstance.FindToggle();
        if(SoundManager.GetInstance.isMuteToggleOn)
        {
            muteToggle.isOn = true;
            SoundManager.GetInstance.bgmSound.mute = true;
            SoundManager.GetInstance.sfxSound.mute = true;
        }
        bgToggle.isOn = SoundManager.GetInstance.isBgToggleOn;
        audioMixer = SoundManager.GetInstance.audioMixer;
        float mValue, bValue, sValue;

        audioMixer.GetFloat("Master", out mValue);
        audioMixer.GetFloat("BGM", out bValue);
        audioMixer.GetFloat("SFX", out sValue);

        masterSlider.value = MathF.Pow(10, mValue / 20);
        BGMSlider.value = MathF.Pow(10, bValue / 20);
        SFXSlider.value = MathF.Pow(10, sValue / 20);


    }

    void Update()
    {
        
    }
}
