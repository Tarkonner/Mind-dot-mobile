using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField, Range(0, 1)] private float minPitch = 0.8f;
    [SerializeField, Range(0, 1)] private float minVolume = 0.8f;

    [SerializeField] private int initialPoolSize = 3;

    private List<AudioSource> audioSources = new();
    private bool musicOn = true;
    private bool soundOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Pre-initialize pool of AudioSources
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        audioSources.Add(source);
        return source;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var src in audioSources)
        {
            if (!src.isPlaying)
                return src;
        }
        // If all sources are busy, create a new one
        return CreateNewAudioSource();
    }

    public void PlayAudioclip(AudioClip clip)
    {
        if (!soundOn || clip == null)
            return;

        var src = GetAvailableAudioSource();
        src.clip = clip;
        src.volume = 1f;
        src.pitch = 1f;
        src.Play();
    }

    public void PlayAudioclip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("No sound in array");
            return;
        }
        PlayAudioclip(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayWithEffects(AudioClip clip)
    {
        if (!soundOn || clip == null)
            return;

        var src = GetAvailableAudioSource();
        src.clip = clip;
        src.volume = Random.Range(minVolume, 1f);
        src.pitch = Random.Range(minPitch, 1f);
        src.Play();
    }

    public void PlayWithEffects(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return;

        PlayWithEffects(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayWithVolume(AudioClip clip, float volume)
    {
        if (!soundOn || clip == null)
            return;

        var src = GetAvailableAudioSource();
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = 1f;
        src.Play();
    }

    public void MusicOnOff()
    {
        musicOn = !musicOn;
        audioMixer.SetFloat("MusicVolume", musicOn ? 0 : -80);
        if (musicOn) MusikManager.Instance.PlayMusic();
        else MusikManager.Instance.StopMusic();
    }

    public void SoundOnOff()
    {
        soundOn = !soundOn;
        audioMixer.SetFloat("SoundVolume", soundOn ? 0 : -80);
    }
}
