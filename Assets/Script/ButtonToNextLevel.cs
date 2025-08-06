using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonToNextLevel : MonoBehaviour
{
    [SerializeField] AudioClip winSound;
    [SerializeField] GameObject button;
    Button buttonComponent;

    [Header("Animation")]
    // Animation settings
    [SerializeField] private float pulseAnimationTime = 1f;
    [SerializeField] private float pulseAnimationScale = 0.2f;
    [SerializeField] float scaleAnimatiomTime = 1.2f;

    // Store references
    private Sequence pulseSequence;
    private Vector3 originalScale;
    private bool isAnimating = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Clean up any existing sequences
        if (pulseSequence != null && pulseSequence.IsActive())
        {
            pulseSequence.Kill();
        }

        buttonComponent = button.GetComponent<Button>();
        buttonComponent.SetEnabled(true);

        // Reset scale and state
        transform.localScale = Vector3.zero;
        isAnimating = false;

        // Convert UI position to world position
        Vector3 worldPosition = Camera.main.WorldToScreenPoint(
            transform.position,
            Camera.MonoOrStereoscopicEye.Mono
        );

        // Start animations
        PlayEffects();
        transform.DOScale(originalScale, scaleAnimatiomTime);
    }

    private void OnDisable()
    {
        // Ensure we clean up properly when disabling
        if (pulseSequence != null)
        {
            pulseSequence.Kill();
        }
        isAnimating = false;
    }

    //private void StartPulsing()
    //{
    //    if (isAnimating || !gameObject.activeSelf) return;

    //    Vector3 scaledUp = originalScale * (1 + pulseAnimationScale);
    //    Vector3 scaledDown = originalScale;

    //    pulseSequence = DOTween.Sequence();
    //    pulseSequence.Append(transform.DOScale(scaledUp, pulseAnimationTime / 2));
    //    pulseSequence.Append(transform.DOScale(scaledDown, pulseAnimationTime / 2));
    //    pulseSequence.SetLoops(-1, LoopType.Restart);

    //    isAnimating = true;
    //}

    public void DeactivateAnimation()
    {
        if (!isAnimating || !gameObject.activeSelf) return;

        buttonComponent.SetEnabled(false);
        //if (pulseSequence != null)
        //{
        //    pulseSequence.Kill();
        //}

        transform.DOScale(Vector3.zero, scaleAnimatiomTime)
                .OnComplete(() =>
                {
                    isAnimating = false;
                    gameObject.SetActive(false);
                });
    }

    public void PlayEffects()
    {
        MultipulParticalController.Instance.PlayParticles();
        AudioManager.Instance.PlayAudioclip(winSound);
    }
}
