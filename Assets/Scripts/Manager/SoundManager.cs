using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audiomixer; 
    public AudioSource bgmSound;
    public AudioSource sfxSound;

    public Slider[] sliders;
    //public Slider sfxSlider;
    
    public List<AudioClip> effectClips = new List<AudioClip> ();
    public List<AudioClip> bgmClips = new List<AudioClip> ();

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

    public void SetBgmVolume()
    {
        FindSlider();
        audiomixer.SetFloat("BGM", Mathf.Log10(sliders[0].value) * 20);
    }   
    
    public void SetSfxVolume()
    {
        FindSlider();
        audiomixer.SetFloat("SFX", Mathf.Log10(sliders[1].value) * 20);
    }
    [ContextMenu("soundoff")]
    public void SoundOff()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
    [ContextMenu("soundon")]
    public void SoundOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    void FindSlider()
    {
        sliders = GameObject.Find("IngameCanvas").transform.GetComponentsInChildren<Slider>();
    }
}
