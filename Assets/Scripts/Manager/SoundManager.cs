using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer audiomixer; 
    public AudioSource bgmSound;
    public AudioSource sfxSound;

    public Slider bgmSlider;
    public Slider sfxSlider;
    
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
        audiomixer.SetFloat("BGM", Mathf.Log10(bgmSlider.value) * 20);
    }   
    
    public void SetSfxVolume()
    {
        audiomixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
    }
}
