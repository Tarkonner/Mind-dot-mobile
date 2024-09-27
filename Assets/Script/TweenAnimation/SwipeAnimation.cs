using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeAnimation : MonoBehaviour
{
    [SerializeField] private Transform hand;
    private Sequence swipeAnimation;
    

    [Header("Animation")]
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private float rotationDegree = -40f;
    [SerializeField] private float timeBetweenAnimation = 4;
    private bool animationHasPlayed = false;

    private void Start()
    {
        swipeAnimation = DOTween.Sequence();
        swipeAnimation.Append(hand.DOScale(hand.transform.localScale, scaleTime));
        swipeAnimation.Append(hand.DORotate(new Vector3(0, 0, rotationDegree), rotationDuration));
        swipeAnimation.Append(hand.DOScale(0, scaleTime));

        hand.localScale = Vector3.zero;
    }

    public void PlayAnimation()
    {
        if(animationHasPlayed) return;

        if(!hand.gameObject.activeSelf)
        {
            hand.gameObject.SetActive(true);
            
            swipeAnimation.Play();
            swipeAnimation.OnComplete(() =>
            {
                hand.localEulerAngles = Vector3.zero;
                swipeAnimation.Restart();
                hand.gameObject.SetActive(false);

            });
            //hand.DORotate(new Vector3(0, 0, rotationDegree), rotationDuration).OnComplete(() =>
            //{
            //    hand.localEulerAngles = Vector3.zero;
            //    hand.gameObject.SetActive(false);
            //});
        }
    }

    IEnumerator AnimationTimer()
    {
        animationHasPlayed = true;
        yield return new WaitForSeconds(timeBetweenAnimation);
        animationHasPlayed = false;
    }
}
