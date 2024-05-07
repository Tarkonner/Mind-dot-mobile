using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    private DG.Tweening.Sequence pulseSequence;
    [SerializeField] float pulseAnimationTime = 1;
    [SerializeField] float pulseAnimationScale = .2f;

    // Start is called before the first frame update
    void Start()
    {
        //Pulse animation
        pulseSequence = DOTween.Sequence();
        pulseSequence.Append(transform.DOScale(1 + pulseAnimationScale, pulseAnimationTime / 2));
        pulseSequence.Append(transform.DOScale(1, pulseAnimationTime / 2));
        pulseSequence.SetLoops(-1, LoopType.Restart);
        pulseSequence.Play();
    }
}
