using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource sound;
    
    public List<AudioClip> effectClips = new List<AudioClip> ();
    public List<AudioClip> bgmClips = new List<AudioClip> ();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            BgmPlay();
        }
        else Destroy(this.gameObject);
    }

    public void BgmPlay()
    {
        sound.loop = true;
        sound.volume = 0.3f;
        sound.Play(); 
    }

    public void Play(AudioClip clip)
    {
        //sound.clip = clip;
        Debug.Log(clip.name);
        sound.PlayOneShot(clip);
    }
}
