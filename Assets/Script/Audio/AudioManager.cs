using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioMixer audioMixer;

    private AudioSource audioSource;

    [SerializeField, Range(0, 1)] private float minPitch = .8f;
    [SerializeField, Range(0, 1)] private float minVolume = .8f;

    private bool musicOn = true;
    private bool soundOn = true;

    private void Awake()
    {
        //Singelton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //Compoment
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioclip(AudioClip audioClip)
    {
        if(audioClip == null)
        {
            Debug.LogError("No sound in call");
            return;
        }

        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void PlayAudioclip(AudioClip[] audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogError("No sound in call");
            return;
        }

        PlayAudioclip(audioClip[Random.Range(0, audioClip.Length)]);
    }

    public void PlayWithEffects(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.volume = Random.Range(minVolume, 1);
        audioSource.pitch = Random.Range(minPitch, 1);
        audioSource.Play();
    }
    public void PlayWithEffects(AudioClip[] audioClip)
    {
        PlayWithEffects(audioClip[Random.Range(0, audioClip.Length)]);
    }

    public void MusicOnOff()
    {       
        if(musicOn)
        {
            audioMixer.SetFloat("MusicVolume", 0);
            musicOn = false;
            MusikManager.Instance.StopMusic();
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", 1);
            musicOn = true;
            MusikManager.Instance.PlayMusic();
        }
    }

    public void SoundOnOff()
    {
        if (soundOn)
        {
            audioMixer.SetFloat("SoundVolume", 0);
            soundOn = false;
        }
        else
        {
            audioMixer.SetFloat("SoundVolume", 1);
            soundOn= true;
        }
    }
}
