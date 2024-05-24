using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IDragHandler
{
    [SerializeField] private GameObject dotPrefab;
    RectTransform rectTransform;

    [HideInInspector] public Vector2[] gridPosArray;
    [HideInInspector] public Dot[] dotsArray;
    [HideInInspector] public Vector2 pivotPoint;

    //Rotation
    private bool rotatable = true;
    private int rotationInt = 0;

    [Header("Lines")]
    [SerializeField] private float dotSpacing;
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

    public void ChangeState(pieceStats targetState)
    {
        //Stop animation
        if(currentState == pieceStats.small)
            pulseSequence.Pause();

        currentState = targetState;

        switch (currentState)
        {
            case pieceStats.small:
                transform.localScale = new Vector3(smallPieceSize, smallPieceSize, smallPieceSize);
                SetAplha(1);                
                pulseSequence.Play();
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
            GameObject spawn = Instantiate(dotPrefab, transform);
            Dot targetDot = spawn.GetComponent<Dot>();
            RectTransform rect = spawn.GetComponent<RectTransform>();

            Vector2 offset = new Vector2((targetPiece.pieceSize.x - 1) * 0.5f, (targetPiece.pieceSize.y - 1) * 0.5f);

            Vector2 calPosition = new Vector2(gridPosArray[i].x * dotSpacing - offset.x * dotSpacing, (gridPosArray[i].y * dotSpacing - offset.y * dotSpacing) * -1);
            rect.anchoredPosition = calPosition;
            dotsPosition.Add(spawn, calPosition);

            dotsArray[i] = targetDot;
            targetDot.Setup(targetPiece.dotTypes[i], this);

            if (i == 0)
                firstDot = spawn;
        }

        TwoKeyDictionary keyDictionary = new TwoKeyDictionary();

        //Goes through each dot and measures grid distance to each other dot.
        // Distance is used to differentiate adjacent and diagonal dot connections. 
        for (int i = 0; i < gridPosArray.Length; i++)
        {
            List<int> diagonalList = new List<int>();
            bool foundAdjacent = false;
            for (int j = 0; j < gridPosArray.Length; j++)
            {
                if (i == j) continue;

                float val = Vector2.Distance(gridPosArray[i], gridPosArray[j]);
                if (val > 1.5f)
                    continue;
                else if (val > 1.2f && !foundAdjacent)
                {
                    diagonalList.Add(j);
                }
                else if (val == 1)
                {
                    foundAdjacent = true;
                    MakeLine(keyDictionary, dotsArray[i], dotsArray[j]);
                }
            }
            if (!foundAdjacent)
            {
                foreach (int j in diagonalList)
                {
                    MakeLine(keyDictionary, dotsArray[i], dotsArray[j]);
                }
            }
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
    void RotationCalculation()
    {
        if (!rotatable)
            return;

        for (int i = 0; i < gridPosArray.Length; i++)
        {
            //Rotation math for vector rotation around a point. (x,y)->(x',y')=(a+(x-a)cos(t)-(y-?)sin(t),b+(x-a)sin(t)+(y-b)cos(t))
            //(x,y) is the point that is to be rotated. (a,b) is the pivot point of the rotation.
            //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
            gridPosArray[i] = new Vector2((pivotPoint.x + (gridPosArray[i].y - pivotPoint.y)),
               (pivotPoint.y - (gridPosArray[i].x - pivotPoint.y)));
        }

        rotationInt = (rotationInt + 1) % 4;
        CenterCalculation();
    }
    public void Rotate()
    {
        if (!rotatable)
            return;

        RotationCalculation();

        //Set rotation
        SetRotation();
    }
    public void RotateWithAnimation()
    {
        if (currentlyRotation)
            return;

        //Woble if not rotadebul
        if(rotatable)
        {
            RotationCalculation();

            //Rotation Animation
            rectTransform.DORotate(new Vector3(0, 0, rotationInt * 90), rotationTime);
            StartCoroutine(RotateTimer());
        }
        else
        {
            //Wigle animation
            Vector3 startRotation = transform.eulerAngles;
            Vector3 highRotation = new Vector3(startRotation.x, startRotation.y, startRotation.y + wigleRotation);
            Vector3 lowRotation = new Vector3(startRotation.x, startRotation.y, startRotation.y - wigleRotation);

            wigleSequence = DOTween.Sequence();
            wigleSequence.Append(transform.DORotate(highRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(startRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(lowRotation, wigleTime / 4));
            wigleSequence.Append(transform.DORotate(startRotation, wigleTime / 4));
            wigleSequence.Play();
        }
    }
    IEnumerator RotateTimer()
    {
        currentlyRotation = true;
        yield return new WaitForSeconds(rotationTime);
        currentlyRotation = false;
    }
    #endregion

    void MakeLine(TwoKeyDictionary dictionary, Dot dot1, Dot dot2)
    {
        Vector2 posOne = dot1.GetComponent<RectTransform>().localPosition;
        Vector2 posTwo = dot2.GetComponent<RectTransform>().localPosition;
        bool result = dictionary.HaveElement(posOne, posTwo);

        if(!result) 
        {
            dictionary.AddElement(posOne, posTwo);
            CreateLine(dot1, dot2);
        }
    }

    public void CreateLine(Dot dot1, Dot dot2)
    {
        //Setup
        GameObject newObject = new GameObject();
        newObject.AddComponent<CanvasRenderer>();
        RectTransform newRectTrans = newObject.AddComponent<RectTransform>();

        //Get values
        newRectTrans.SetParent(lineHolder.transform, false);
        RectTransform dot1Rect = dot1.GetComponent<RectTransform>();
        RectTransform dot2Rect = dot2.GetComponent<RectTransform>();

        //Line
        UILine uiLine = newObject.AddComponent<UILine>();
        connections.Add(uiLine);
        uiLine.Initialzie(dot1Rect, dot2Rect, lineWidth, rotatable);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.position = InputSystem.instance.touchPosition;
    }

    public void ReturnToHolder()
    {
        //Scale
        transform.DOKill();

        gameObject.transform.SetParent(pieceHolder);
        ChangeState(pieceStats.small);
        rectTransform.anchoredPosition = Vector2.zero;
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
}
