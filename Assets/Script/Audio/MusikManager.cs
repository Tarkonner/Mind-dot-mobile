using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusikManager : MonoBehaviour
{
    private AudioSource musicSource;

    [SerializeField] private AudioClip[] musicClips;
    private AudioClip[] lastPlayed = new AudioClip[2];
    private List<AudioClip> playList = new List<AudioClip>();
    private int musicPlayingIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();


        AudioClip[] tempMusic = ShuffleElements.ShuffleWithReturn(musicClips);
        for (int i = 0; i < musicClips.Length; i++)
            playList.Add(tempMusic[i]);

        PlayMusic();
    }

    public void PlayMusic()
    {
        musicSource.clip = musicClips[musicPlayingIndex];
        musicSource.Play();

        musicPlayingIndex++;

        //Randomish music
        if(musicPlayingIndex >= musicClips.Length)
        {
            musicPlayingIndex = 0;
            ShuffleElements.ReshuffleWithWight(musicClips, 2);
        }            

        StartCoroutine(NextMusicNumber(musicSource.clip.length));
    }

    IEnumerator NextMusicNumber(float musicTime)
    {
        Debug.Log("Next music");
        yield return new WaitForSeconds(musicTime);
        PlayMusic();
    }
}
