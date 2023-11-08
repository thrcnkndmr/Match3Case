using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip backgroundMusic;

    private AudioSource _audioSource;
    private AudioSource _musicSource;

    private void Awake()
    {
        var  sources = GetComponents<AudioSource>();
        _audioSource = sources[0];
        _musicSource = sources[1];
        PlayMusic(backgroundMusic);
    }

    public void PlayMatchSound()
    {
        _audioSource.PlayOneShot(matchSound);
    }

    public void PlayWinSound()
    {
        _audioSource.PlayOneShot(winSound);
    }

    public void PlayLoseSound()
    {
        _audioSource.PlayOneShot(loseSound);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        _musicSource.clip = musicClip;
        _musicSource.loop = true;
        _musicSource.Play();
    }
}
