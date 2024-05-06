using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiniarMovement : MonoBehaviour
{
    [SerializeField] Vector2 moveDirection = Vector2.zero;
    [SerializeField] float animationTime = .5f;
    private Sequence sequence;

    void Start()
    {
        Vector2 startPosition = transform.localPosition;

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(startPosition + moveDirection, animationTime));
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Play();
    }
}
