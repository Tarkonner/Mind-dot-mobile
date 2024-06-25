using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    [SerializeField, Range(0, 1)] private float minPitch = .8f;
    [SerializeField, Range(0, 1)] private float minVolume = .8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioclip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayWithEffects(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.volume = Random.Range(minVolume, 1);
        audioSource.pitch = Random.Range(minPitch, 1);
        audioSource.Play();
    }
}
