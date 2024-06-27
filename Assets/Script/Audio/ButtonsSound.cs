using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsSound : MonoBehaviour
{
    [SerializeField] AudioClip[] soundEffects;

    public void PlaySound()
    {
        AudioManager.Instance.PlayWithEffects(soundEffects);
    }
}
