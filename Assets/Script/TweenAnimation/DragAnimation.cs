using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAnimation : MonoBehaviour
{
    [SerializeField] GameObject mainHand;
    [SerializeField] GameObject secondHand;
    [SerializeField] GameObject pieceImage;

    [SerializeField] float beforeRestart = .5f;
    [Header("Main hand")]
    [SerializeField] Vector3 mainHandMovement = new Vector3(0, 0, 100);
    [SerializeField] float mainHandAnimationTime = 1f;
    [Header("Second hand")]
    [SerializeField] float secondHandScale = 0.2f;
    [SerializeField] float secondHandScaleTime = .2f;
    private float startScale_SH;
    [Header("Rotation")]
    [SerializeField] float rotationTime = .2f;

    private DG.Tweening.Sequence dragSequence;
    private DG.Tweening.Sequence rotationSequence;

    void Start()
    {
        startScale_SH = secondHand.transform.localScale.y;
        float targetScale_SH = startScale_SH - secondHandScale;

        dragSequence = DOTween.Sequence();
        rotationSequence = DOTween.Sequence();

        //Rotate
        rotationSequence.Append(pieceImage.transform.DORotate(pieceImage.transform.localEulerAngles + new Vector3(0, 0, -90), rotationTime));
        dragSequence.Append(mainHand.transform.DORotate(pieceImage.transform.localEulerAngles + new Vector3(0, 0, -40), rotationTime));

        //dragSequence.Append(mainHand.transform.DOMove(mainHand.transform.position + mainHandMovement, mainHandAnimationTime));
        //dragSequence.Append(secondHand.transform.DOScale(new Vector3(-targetScale_SH, targetScale_SH, targetScale_SH), secondHandScaleTime / 2));
        //dragSequence.Append(rotationSequence.Play());
        //dragSequence.Append(secondHand.transform.DOScale(new Vector3(-startScale_SH, startScale_SH, startScale_SH), secondHandScaleTime / 2));
        //dragSequence.Append(pieceImage.transform.DOScale(Vector3.one * 1.5f, beforeRestart));
        //dragSequence.Append(mainHand.transform.DOMove(mainHand.transform.position, 0));


        
        dragSequence.SetLoops(-1, LoopType.Restart);
        dragSequence.Play();
        rotationSequence.SetLoops(-1, LoopType.Restart);
        rotationSequence.Play();
    }
}
