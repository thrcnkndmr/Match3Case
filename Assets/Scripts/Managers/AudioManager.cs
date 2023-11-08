using thrcnkndmr;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip backgroundMusic;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        var sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        musicSource = sources[1];
        PlayMusic(backgroundMusic);
    }

    private void OnEnable()
    {
        EventManager.OnFindMatch += OnFindMatch;
        EventManager.OnLevelFail += OnLevelFail;
        EventManager.OnLevelSuccess += OnLevelSuccess;
    }

    private void OnLevelSuccess()
    {
        PlayWinSound();
    }

    private void OnLevelFail()
    {
        PlayLoseSound();
    }

    private void OnFindMatch()
    {
        PlayMatchSound();
    }

    private void PlayMatchSound()
    {
        audioSource.PlayOneShot(matchSound);
    }

    private void PlayWinSound()
    {
        audioSource.PlayOneShot(winSound);
    }

    private void PlayLoseSound()
    {
        audioSource.PlayOneShot(loseSound);
    }

    private void PlayMusic(AudioClip musicClip)
    {
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void OnDisable()
    {
        EventManager.OnFindMatch -= OnFindMatch;
        EventManager.OnLevelFail -= OnLevelFail;
        EventManager.OnLevelSuccess -= OnLevelSuccess;
    }
}