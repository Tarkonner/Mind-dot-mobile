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

    public void PlayAnimation()
    {
        if(!hand.gameObject.activeSelf)
        {
            hand.gameObject.SetActive(true);
            hand.DORotate(new Vector3(0, 0, rotationDegree), rotationDuration).OnComplete(() =>
            {
                hand.localEulerAngles = Vector3.zero;
                hand.gameObject.SetActive(false);
            });
        }
    }
}
