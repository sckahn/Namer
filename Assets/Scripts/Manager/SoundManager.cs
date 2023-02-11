using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource bgmSound;
    public AudioSource sfxSound;

    public Slider[] sliders;
    Toggle muteToggle;
    //public Slider sfxSlider;
    
    public List<AudioClip> effectClips = new List<AudioClip> ();
    public List<AudioClip> bgmClips = new List<AudioClip> ();

    public bool isMuted;

    private void Awake()
    {
        BgmPlay();
    }

    public void BgmPlay()
    {
        bgmSound.loop = true;
        bgmSound.volume = 1;
        bgmSound.Play(); 
    }

    public void Play(AudioClip clip)
    {
        sfxSound.PlayOneShot(clip);
    }

    public void SetMasterVolume()
    {
        if (isMuted) return;
        FindSlider();
        audioMixer.SetFloat("Master", Mathf.Log10(sliders[0].value) * 20);
    }

    public void SetBgmVolume()
    {
        if (isMuted) return;
        FindSlider();
        audioMixer.SetFloat("BGM", Mathf.Log10(sliders[1].value) * 20);
    }   
    
    public void SetSfxVolume()
    {
        if (isMuted) return;
        FindSlider();
        audioMixer.SetFloat("SFX", Mathf.Log10(sliders[2].value) * 20);
    }

    //public void SetSound()
    //{
    //    if (isMuted)
    //    {
    //        audioMixer.SetFloat("Master", 0);
    //        isMuted = !isMuted;
    //    }
    //    else
    //    {
    //        audioMixer.SetFloat("Master", -80);
    //        isMuted = !isMuted;
    //    }
    //}
    [ContextMenu("Mute")]
    public void SetSound()
    {
        //if (isMuted)
        //{
        //    audioMixer.SetFloat("Master", 0);
        //    isMuted = !isMuted;
        //}
        //else 
        //{
        //    audioMixer.SetFloat("Master", -80);
        //    isMuted = !isMuted;
        //}
        Debug.Log("mute");
        Debug.Log(bgmSound.mute);
        Debug.Log(sfxSound.mute);
        bgmSound.mute = !bgmSound.mute;
        sfxSound.mute = !sfxSound.mute;
    }

    void FindSlider()
    {
        sliders = GameObject.Find("IngameCanvas").transform.GetComponentsInChildren<Slider>();
    }

    public void FindToggle()
    {
        muteToggle = GameObject.Find("IngameCanvas").transform.GetComponentInChildren<Toggle>(true);
        muteToggle.onValueChanged.AddListener(delegate {
            SetSound();
            });
    }
}
