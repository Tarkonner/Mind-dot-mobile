using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    private DG.Tweening.Sequence pulseSequence;
    [SerializeField] float pulseAnimationTime = 1;
    [SerializeField] float pulseAnimationScale = .2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // Target scales based on original scale
        Vector3 scaledUp = originalScale * (1 + pulseAnimationScale);
        Vector3 scaledDown = originalScale;

        pulseSequence = DOTween.Sequence();
        pulseSequence.Append(transform.DOScale(scaledUp, pulseAnimationTime / 2));
        pulseSequence.Append(transform.DOScale(scaledDown, pulseAnimationTime / 2));
        pulseSequence.SetLoops(-1, LoopType.Restart);
        pulseSequence.Play();
    }
}
