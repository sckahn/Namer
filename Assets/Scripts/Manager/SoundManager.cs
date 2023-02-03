using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource sound;
    
    public List<AudioClip> effectClips = new List<AudioClip> ();
    public List<AudioClip> bgmClips = new List<AudioClip> ();

    private void Awake()
    {
        BgmPlay();
    }

    public void BgmPlay()
    {
        sound.loop = true;
        sound.volume = 0.3f;
        sound.Play(); 
    }

    public void Play(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }
}
