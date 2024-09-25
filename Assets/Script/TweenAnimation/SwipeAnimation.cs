using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeAnimation : MonoBehaviour
{
    [SerializeField] GameObject hand;
    Sequence swipeAnimation;

    
    [Header("Animation")]
    [SerializeField] float scaleTime = .1f;
    [SerializeField] float rotaionDegree = -40;
    [SerializeField] float rotationTime = 2f;

    private void Start()
    {
        hand.transform.localScale = Vector3.zero;

        //Make animation
        swipeAnimation = DOTween.Sequence();
        swipeAnimation.Append(hand.transform.DOScale(1, scaleTime));
        swipeAnimation.Append(hand.transform.DORotate(new Vector3(0, 0, rotaionDegree), rotationTime));
        swipeAnimation.Append(hand.transform.DOScale(0, scaleTime));
    }

    public void PlayAnimation()
    {
        if(!swipeAnimation.IsPlaying())
        {
            swipeAnimation.Play();
            swipeAnimation.OnComplete(() => { hand.transform.localEulerAngles = Vector3.zero; });
        }
    }
}
