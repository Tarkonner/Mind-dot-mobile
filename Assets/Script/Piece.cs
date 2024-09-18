using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IDragHandler
{
    RectTransform rectTransform;

    [HideInInspector] public Vector2[] gridPosArray;
    [HideInInspector] public Dot[] dotsArray;
    [HideInInspector] public Vector2 pivotPoint;

    [Header("Rotation")]
    [SerializeField] private bool defultRotateDirection = false;
    public bool rotatable { get; private set; } = true;
    private int rotationInt = 0;
    [HideInInspector] public bool onBoard = false;

    [Header("Lines")]
    [SerializeField] private float dotSpacing;
    private const float ADJACENT_THRESHOLD = 130f;
    private const float DIAGONAL_THRESHOLD = 180f;

    public float DotSpacing { get { return dotSpacing; } }
    private List<UILine> connections = new List<UILine>();
    private Transform pieceHolder;
    private GameObject lineHolder;
    public float lineWidth = 10;

    private Vector2Int savedCenterCoordinats;
    public Vector2Int pieceCenter { get; private set; }

    Dictionary<GameObject, Vector2> dotsPosition = new Dictionary<GameObject, Vector2>();

    //Raycast from dot
    public GameObject firstDot { get; private set; }

    //Stats
    public enum pieceStats { small, transparent, normal };
    private pieceStats currentState;

    [HideInInspector] public float smallPieceSize = .35f;

    [Header("Animation")]
    [SerializeField] float rotationTime = .2f;
    [SerializeField] float scaleAnimation = .2f;
    [HideInInspector] public bool currentlyRotation = false;
    [SerializeField] float pulseAnimationTime = 1;
    [SerializeField] float pulseAnimationScale = .2f;
    private DG.Tweening.Sequence pulseSequence;
    private DG.Tweening.Sequence wigleSequence;
    [SerializeField] float wigleRotation = 15;
    [SerializeField] float wigleTime = 1f;
    [SerializeField] float returnToHolderTime = .5f;
    [SerializeField] float scaleToSmallTime = .25f;
    public bool animationActice { get; private set; } = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        //Pulse animation
        pulseSequence = DOTween.Sequence();
        pulseSequence.Append(transform.DOScale(smallPieceSize + pulseAnimationScale, pulseAnimationTime / 2));
        pulseSequence.Append(transform.DOScale(smallPieceSize, pulseAnimationTime / 2));
        pulseSequence.SetLoops(-1, LoopType.Restart);

        pieceHolder = transform.parent;

        ChangeState(pieceStats.small);
    }

    private void OnEnable()
    {
        InputSystem.onSwipe += RotatioOnSwipe;
    }

    private void OnDisable()
    {
        InputSystem.onSwipe -= RotatioOnSwipe;
    }

    public void ChangeState(pieceStats targetState)
    {
        //Stop animation
        if(currentState == pieceStats.small)
            pulseSequence.Pause();

        currentState = targetState;

        switch (currentState)
        {
            case pieceStats.small:
                //transform.localScale = new Vector3(smallPieceSize, smallPieceSize, smallPieceSize);
                transform.DOScale(new Vector3(smallPieceSize, smallPieceSize, smallPieceSize), scaleToSmallTime).OnComplete(() =>
                {
                    SetAplha(1);   
                    pulseSequence.Play();
                });                
                break;
            case pieceStats.transparent:
                transform.DOScale(Vector3.one, scaleAnimation);
                SetAplha(.5f);
                break;
            case pieceStats.normal:
                transform.localScale = Vector3.one;
                SetAplha(1);
                break;
            default:
                break;
        }
    }

    public void LoadPiece(LevelPiece targetPiece)
    {
        rotatable = targetPiece.rotatable;

        //Lines
        lineHolder = new GameObject();
        lineHolder.transform.SetParent(transform, false);

        //Set cordinats
        gridPosArray = new Vector2[targetPiece.dotPositions.Length];
        for (int i = 0; i < gridPosArray.Length; i++)
            gridPosArray[i] = targetPiece.dotPositions[i];

        //Setup dots
        dotsArray = new Dot[targetPiece.dotPositions.Length];
        for (int i = 0; i < targetPiece.dotPositions.Length; i++)
        {
            //Make object
            GameObject spawn = DotPool.instance.GetDot(targetPiece.dotTypes[i], this);
            spawn.transform.parent = transform;
            spawn.transform.localScale = Vector3.one;
            RectTransform rect = spawn.GetComponent<RectTransform>();

            Vector2 offset = new Vector2((targetPiece.pieceSize.x - 1) * 0.5f, (targetPiece.pieceSize.y - 1) * 0.5f);

            Vector2 calPosition = new Vector2(gridPosArray[i].x * dotSpacing - offset.x * dotSpacing, (gridPosArray[i].y * dotSpacing - offset.y * dotSpacing) * -1);
            rect.anchoredPosition = calPosition;
            dotsPosition.Add(spawn, calPosition);

            //Line
            dotsArray[i] = spawn.GetComponent<Dot>();

            if (i == 0)
                firstDot = spawn;
        }


        //Connection lines
        TwoKeyDictionary twoKeyDictionary = new TwoKeyDictionary();
        Dictionary<Dot, int> makedConnectionCount = new Dictionary<Dot, int>();
        for (int i = 0; i < dotsArray.Length; i++)
        {
            int straightLinesMax = 2;

            for (int j = 0; j < dotsArray.Length; j++)
            {
                if (i == j) continue;

                Dot currentDot = dotsArray[i];
                Dot checkingDot = dotsArray[j];

                if(!makedConnectionCount.ContainsKey(currentDot))
                    makedConnectionCount.Add(currentDot, 0);
                if (!makedConnectionCount.ContainsKey(checkingDot))
                    makedConnectionCount.Add(checkingDot, 0);

                if (!twoKeyDictionary.HaveElement(currentDot, checkingDot))
                {
                    float distance = Vector2.Distance(currentDot.GetComponent<RectTransform>().localPosition, checkingDot.GetComponent<RectTransform>().localPosition);

                    if(distance < ADJACENT_THRESHOLD)
                    {
                        twoKeyDictionary.AddElement(currentDot, checkingDot);
                        makedConnectionCount[currentDot] = makedConnectionCount[currentDot] + 1;
                        makedConnectionCount[checkingDot] = makedConnectionCount[checkingDot] + 1;
                    }
                }
            }

            if (makedConnectionCount[dotsArray[i]] >= straightLinesMax)
                continue;


            for (int j = 0; j < dotsArray.Length; j++)
            {
                if (i == j) continue;

                Dot currentPosRec = dotsArray[i];
                Dot checkingPosRec = dotsArray[j];

                if (!twoKeyDictionary.HaveElement(currentPosRec, checkingPosRec))
                {
                    float distance = Vector2.Distance(currentPosRec.GetComponent<RectTransform>().localPosition, checkingPosRec.GetComponent<RectTransform>().localPosition);

                    if (distance < DIAGONAL_THRESHOLD)
                        twoKeyDictionary.AddElement(currentPosRec, checkingPosRec);
                }
            }


        }
        //Make lines
        foreach (var item in twoKeyDictionary.keyValues)
        {
            var k = item.Key.ToValueTuple();
            CreateLine(k.Item1, k.Item2);
        }

        //Find center
        savedCenterCoordinats.x = (int)Mathf.Floor(targetPiece.pieceSize.x / 2);
        savedCenterCoordinats.y = (int)Mathf.Floor(targetPiece.pieceSize.y / 2);
        if (targetPiece.pieceSize.x % 2 == 0) //Even
        {
            savedCenterCoordinats.x = (int)Mathf.Floor(targetPiece.pieceSize.x / 2);
        }
        else //Odd
        {
            savedCenterCoordinats.x = (int)(targetPiece.pieceSize.x / 2);
        }
        if (targetPiece.pieceSize.y % 2 == 0) //Even
        {
            savedCenterCoordinats.y = (int)Mathf.Floor(targetPiece.pieceSize.y / 2);
        }
        else //Odd
        {
            savedCenterCoordinats.y = (int)(targetPiece.pieceSize.y / 2);
        }
        CenterCalculation();

        //Rotation
        for (int i = 0; i < targetPiece.startRotation; i++)
            Rotate();
    }

    public void EnforceDotPositions()
    {
        for (int i = 0; i < dotsArray.Length; i++)
        {
            RectTransform rect = dotsArray[i].GetComponent<RectTransform>();
            //Set position
            rect.anchoredPosition = dotsPosition[dotsArray[i].gameObject];
            //Set rotation
            SetRotation();
        }
    }

    #region Rotation
    void RotationCalculation(bool rightRotation)
    {
        if (!rotatable)
            return;


        if (rightRotation)
        {
            //Right rotation
            for (int i = 0; i < gridPosArray.Length; i++)
            {
                //Rotation math for vector rotation around a point. (x,y)->(x',y')=(a+(x-a)cos(t)-(y-?)sin(t),b+(x-a)sin(t)+(y-b)cos(t))
                //(x,y) is the point that is to be rotated. (a,b) is the pivot point of the rotation.
                //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
                gridPosArray[i] = new Vector2(
                    pivotPoint.x - (gridPosArray[i].y - pivotPoint.y),
                    pivotPoint.y + (gridPosArray[i].x - pivotPoint.y)
                );
            }
            rotationInt = (rotationInt - 1) % 4;
        }
        else
        {
            //Left rotation
            for (int i = 0; i < gridPosArray.Length; i++)
            {
                //Rotation math for vector rotation around a point. (x,y)->(x',y')=(a+(x-a)cos(t)-(y-?)sin(t),b+(x-a)sin(t)+(y-b)cos(t))
                //(x,y) is the point that is to be rotated. (a,b) is the pivot point of the rotation.
                //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
                gridPosArray[i] = new Vector2(
                    pivotPoint.x + (gridPosArray[i].y - pivotPoint.y),
                    pivotPoint.y - (gridPosArray[i].x - pivotPoint.y)
                );
            }
            rotationInt = (rotationInt + 1) % 4;
        }



        CenterCalculation();
    }
    public void Rotate()
    {
        if (!rotatable)
            return;

        RotationCalculation(defultRotateDirection);

        //Set rotation
        SetRotation();
    }
    public void RotateWithAnimation(bool rotateRight)
    {
        if (currentlyRotation)
            return;

        //Woble if not rotadebul
        if(rotatable)
        {
            RotationCalculation(rotateRight);

            //Rotation Animation
            rectTransform.DORotate(new Vector3(0, 0, rotationInt * 90), rotationTime);
            StartCoroutine(RotateTimer(rotationTime));
        }
        else
        {
            //Wigle animation
            Vector3 startRotation = transform.eulerAngles;
            Vector3 highRotation = new Vector3(startRotation.x, startRotation.y, startRotation.y + wigleRotation);
            Vector3 lowRotation = new Vector3(startRotation.x, startRotation.y, startRotation.y - wigleRotation);

            //Don't get called again timer
            StartCoroutine(RotateTimer(wigleTime));

            //Wiggle animation
            wigleSequence = DOTween.Sequence();
            wigleSequence.Append(transform.DORotate(highRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(startRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(lowRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(startRotation, wigleTime / 4));
            wigleSequence.Play();
        }
    }
    IEnumerator RotateTimer(float timeBeforeRationStops)
    {
        currentlyRotation = true;
        yield return new WaitForSeconds(timeBeforeRationStops);
        currentlyRotation = false;
    }

    private void RotatioOnSwipe(bool rotaRight)
    {
        if (!onBoard)
        {
            if(rotaRight)
                RotateWithAnimation(true);
            else
                RotateWithAnimation(false);
        }
    }
    #endregion

    //void MakeLine(TwoKeyDictionary dictionary, Dot dot1, Dot dot2)
    //{
    //    Vector2 posOne = dot1.GetComponent<RectTransform>().localPosition;
    //    Vector2 posTwo = dot2.GetComponent<RectTransform>().localPosition;
    //    bool result = dictionary.HaveElement(posOne, posTwo);

    //    if(!result) 
    //    {
    //        dictionary.AddElement(posOne, posTwo);
    //        CreateLine(dot1, dot2);
    //    }
    //}

    public void CreateLine(Dot dot1, Dot dot2)
    {
        //Get values
        RectTransform dot1Rect = dot1.GetComponent<RectTransform>();
        RectTransform dot2Rect = dot2.GetComponent<RectTransform>();

        //Line
        UILine uiLine = LinePool.instance.GetLine(dot1Rect, dot2Rect, lineWidth, rotatable);
        uiLine.GetComponent<RectTransform>().SetParent(lineHolder.transform, false);
        connections.Add(uiLine);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.position = InputSystem.instance.touchPosition;
    }

    public void ReturnToHolder()
    {
        //Scale
        transform.DOKill();

        animationActice = true;
        gameObject.transform.SetParent(pieceHolder);
        transform.DOLocalMove(Vector3.zero, returnToHolderTime).OnComplete(() =>
        {
            animationActice = false;
        });
        ChangeState(pieceStats.small);
    }

    private void SetAplha(float alphaValue)
    {
        //Lines
        foreach (UILine item in connections)
        {
            Color targetColor = item.color;
            targetColor.a = alphaValue;
            item.color = targetColor;
        }

        //Dots
        foreach (var item in dotsArray)
        {
            Image image = item.gameObject.GetComponent<Image>();
            Color targetColor = image.color;
            targetColor.a = alphaValue;
            image.color = targetColor;
        }
    }

    private void SetRotation() => GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationInt * 90);

    private void CenterCalculation()
    {
        switch (rotationInt)
        {
            case 0:
                pieceCenter = new Vector2Int(-savedCenterCoordinats.x, -savedCenterCoordinats.y);
                break;
            case 1:
                pieceCenter = new Vector2Int(-savedCenterCoordinats.y, savedCenterCoordinats.x);
                break;
            case 2:
                pieceCenter = new Vector2Int(savedCenterCoordinats.x, savedCenterCoordinats.y);
                break;
            case 3:
                pieceCenter = new Vector2Int(savedCenterCoordinats.y, -savedCenterCoordinats.x);
                break;
        }
    }

    private void CheckConnections(Dot currentDot, Dot[] allDots, List<Dot> connectedDots)
    {
        foreach (Dot dot in allDots)
        {
            if(dot ==  currentDot) 
                continue;

            if (!connectedDots.Contains(dot))
            {
                float distance = Mathf.Sqrt(Mathf.Pow(
                    currentDot.transform.position.x - dot.transform.position.x, 2) 
                    + Mathf.Pow(currentDot.transform.position.y - dot.transform.position.y, 2));
                if (distance <= ADJACENT_THRESHOLD || distance <= DIAGONAL_THRESHOLD)
                {
                    connectedDots.Add(dot);

                    CheckConnections(dot, allDots, connectedDots);
                }
            }
        }
    }
}
