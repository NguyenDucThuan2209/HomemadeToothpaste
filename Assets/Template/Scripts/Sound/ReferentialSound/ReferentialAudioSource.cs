using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
[RequireComponent(typeof(AudioSource))]
public class ReferentialAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource Source => audioSource;
    [SerializeField] private ReferentialSoundClip clip;
    public ReferentialSoundClip Clip {
        set
        {
            clip = value;
            audioSource.clip = clip.Clip;
            if(audioSource.isPlaying)
                audioSource.Play();
        }
        get => clip;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (clip != null) audioSource.clip = clip.Clip;
        if(audioSource.playOnAwake)
            audioSource.Play();
    }
}
}