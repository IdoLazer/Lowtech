using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioClip song;
    public AudioSource audioSource;

    public void OnAwake()
    {
        audioSource.clip = song;
    }

    public void Play()
    {
        audioSource.Play();
    }

}
