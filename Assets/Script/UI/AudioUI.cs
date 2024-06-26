using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private GameObject soundHolder;
    Image soundImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    private bool soundOn = true;
    [Header("Music")]
    [SerializeField] private GameObject musicHolder;
    private Image musicImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    private bool musicOn = true;

    private void Awake()
    {
        soundImage = soundHolder.GetComponent<Image>();
        musicImage = musicHolder.GetComponent<Image>();
    }

    public void MusicClick()
    {
        if(musicOn)
        {
            musicImage.sprite = musicOffSprite;
            musicOn = false;
        }
        else
        {
            musicImage.sprite = musicOnSprite;
            musicOn = true;
        }

        AudioManager.Instance.MusicOnOff();
    }

    public void SoundClickI()
    {
        if (soundOn)
        {
            soundImage.sprite = soundOffSprite;
            soundOn = false;
        }
        else
        {
            soundImage.sprite = soundOnSprite;
            soundOn = true;
        }

        AudioManager.Instance.SoundOnOff();
    }
}
