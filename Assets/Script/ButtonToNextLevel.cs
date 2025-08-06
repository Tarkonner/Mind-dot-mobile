using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonToNextLevel : MonoBehaviour
{
    [SerializeField] AudioClip winSound;

    [Header("Animation")]
    // Animation settings
    [SerializeField] private float pulseAnimationTime = 1f;
    [SerializeField] private float pulseAnimationScale = 0.2f;
    [SerializeField] float scaleAnimatiomTime = 1.2f;

    // Store references
    private Sequence pulseSequence;
    private Vector3 originalScale;
    private bool isAnimating = false;

    private MultipulParticalController multipulParticalController;

    private void Start()
    {
        multipulParticalController = Object.FindFirstObjectByType<MultipulParticalController>();
    }

    private void OnEnable()
    {     
        PlayEffects();
    }

    public void PlayEffects()
    {
        MultipulParticalController.Instance.PlayParticles();
        AudioManager.Instance.PlayAudioclip(winSound);
    }
}
