using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public static InputSystem instance;

    //Input
    private PlayerInput playerInput;
    private InputAction tapAction;
    private InputAction positionAction;
    private InputAction secoundTap;
    private InputAction swipeAction;
    private InputAction secondSwipeAction;

    public Vector2 touchPosition { get; private set; }
    private bool hasRotated = false;

    [Header("Refences")]
    [SerializeField] private Board board;

    [Header("Pieces")]
    [SerializeField] private GameObject movingPiecesHolder;
    [SerializeField] Transform piecesHolder;
    private Piece holdingPiece;
    private RectTransform holdingPieceRect;
    private bool activeTouch = true;

    [Header("Raycast")]
    [SerializeField] GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float distanceBeforeSwipe = 50;
    [SerializeField] float touchOffsetY = 50;
    private bool calledSwipe = false;
    private Vector2 swipeStartPos = Vector2.zero;
    private Vector2 secendSwipeStartPos = Vector2.zero;
    [SerializeField] float swipeDeadZone = 1;
    [SerializeField] int maxMisclicks = 2;
    private int currentMusclicks = 0;
    [SerializeField] float disableInputAfterReleasePieceTime = .1f;
    private Vector2 primeCollectedSwipe;

    [Header("Animations")]
    [SerializeField] float pieceSnapPositionSpeed = 50;
    [SerializeField] SwipeAnimation swipeAnimation;
    private float pieceSnapCalculation;

    [Header("Sounds")]
    [SerializeField] AudioClip notRotateSound;
    [SerializeField] AudioClip[] rotateSounds;
    [SerializeField] AudioClip dropSound;
    [SerializeField] AudioClip pickupSound;

    //Event
    public delegate void OnDotsChange();
    public static event OnDotsChange onDotChange;
    public static event Action<bool> onSwipe;

    //Rotation bug
    [SerializeField] float stopRotate = .1f;
    private float stopRotateTimer;

    private void Awake()
    {
        instance = this;

        //Raycast
        eventSystem = GetComponent<EventSystem>();
        pointerEventData = new PointerEventData(eventSystem);

        //Input
        playerInput = GetComponent<PlayerInput>();
        positionAction = playerInput.actions["TouchPosition"];
        tapAction = playerInput.actions["Tap"];
        secoundTap = playerInput.actions["SecendFinger"];
        swipeAction = playerInput.actions["Swipe"];
        secondSwipeAction = playerInput.actions["SecendSwipe"];
    }

    private void OnEnable()
    {
        //Input
        tapAction.started += BeginDrag;
        tapAction.canceled += Release;
        secoundTap.started += Tap;
        secoundTap.canceled += LiftTap;

        //Turn touch input on
        LevelManager.onLoadLevel += () => activeTouch = true;
        //Turn touch input off
        LevelManager.onLevelComplete += () => activeTouch = false;
    }


    private void OnDisable()
    {
        //Input
        tapAction.started -= BeginDrag;
        tapAction.canceled -= Release;
        secoundTap.started -= Tap;
        secoundTap.canceled -= LiftTap;

        //Turn touch input on
        LevelManager.onLoadLevel -= () => activeTouch = true;
        //Turn touch input off
        LevelManager.onLevelComplete -= () => activeTouch = false;
    }

    private void Update()
    {
        stopRotateTimer += Time.deltaTime;

        if (!activeTouch)
            return;

        //Drag
        //Get input
        touchPosition = positionAction.ReadValue<Vector2>();

        if (holdingPiece != null)
        {
            holdingPiece.OnDrag(pointerEventData); //Not doing anything right now

            //Move
            if (holdingPieceRect == null)
                holdingPieceRect = holdingPiece.gameObject.GetComponent<RectTransform>();

            //Look for swipe
            if (stopRotateTimer < stopRotate) //Rotate bug fix
                return;

            Vector2 swipeInfo = secondSwipeAction.ReadValue<Vector2>();
            if (!calledSwipe && swipeInfo.magnitude >= distanceBeforeSwipe)
            {
                //Sound
                if (!holdingPiece.currentlyRotation)
                {
                    if (holdingPiece.rotatable)
                        AudioManager.Instance.PlayWithEffects(rotateSounds);
                    else
                        AudioManager.Instance.PlayWithEffects(notRotateSound);
                }


                bool rightFromStart = false;
                if (secendSwipeStartPos.x < swipeInfo.x)
                    rightFromStart = true;

                holdingPiece.RotateWithAnimation(rightFromStart);
                calledSwipe = true;
            }
            else if(swipeInfo.magnitude < distanceBeforeSwipe && swipeInfo.magnitude > swipeDeadZone) //Look for short swipe
            {
                swipeAnimation.PlayAnimation();
            }

            //Calculate position
            if (pieceSnapCalculation < 1)
            {
                pieceSnapCalculation += Time.deltaTime * pieceSnapPositionSpeed;
                if (pieceSnapCalculation > 1)
                    pieceSnapCalculation = 1;
            }

            Vector2 targetPosition = touchPosition + new Vector2(0, touchOffsetY + Mathf.RoundToInt(Mathf.Abs(holdingPiece.pieceCenter.y) / 2) * holdingPiece.DotSpacing);
            Vector2 calPosition = Vector2.Lerp(touchPosition, targetPosition, pieceSnapCalculation);

            //Set posotion
            holdingPieceRect.position = calPosition;
        }
        else
        {
            //Look for swipe
            if (stopRotateTimer < stopRotate) //Rotate bug fix
                return;

            Vector2 tickSwipe = swipeAction.ReadValue<Vector2>();
            if (tickSwipe == Vector2.zero && primeCollectedSwipe.magnitude > 0)
            {               
                if (!calledSwipe && primeCollectedSwipe.magnitude >= distanceBeforeSwipe)
                {
                    //Rotate piece
                    bool rightFromStart = false;
                    if (swipeStartPos.x < primeCollectedSwipe.x)
                        rightFromStart = true;

                    onSwipe?.Invoke(rightFromStart);
                    calledSwipe = true;

                    //Sound
                    AudioManager.Instance.PlayWithEffects(rotateSounds);
                }
                else if (primeCollectedSwipe.magnitude < distanceBeforeSwipe
                    && primeCollectedSwipe.magnitude > swipeDeadZone) //Look for short swipe
                {
                    //Play short swipe animation
                    Debug.Log(primeCollectedSwipe.magnitude);
                    Debug.Log("Short swipe one fingur");
                    swipeAnimation.PlayAnimation();
                }

                primeCollectedSwipe = Vector2.zero;
            }
            else
                primeCollectedSwipe += tickSwipe;
        }
    }

    List<RaycastResult> HitDetection(Vector2 inputPosition, GraphicRaycaster raycaster)
    {
        // Create a pointer event data with the current input position        
        pointerEventData.position = inputPosition;

        // Create a list to store the raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the GraphicRaycaster
        raycaster.Raycast(pointerEventData, results);

        return results;
    }

    private void BeginDrag(InputAction.CallbackContext context)
    {
        stopRotateTimer = 0;

        //Swipe
        calledSwipe = false;
        swipeStartPos = swipeAction.ReadValue<Vector2>();


        if (holdingPiece != null || !activeTouch)
            return;

        //Rotation
        hasRotated = true;

        //Look if the user only clicks || misclicks
        bool foundAction = false;

        //Raycast Pieceholder
        touchPosition = positionAction.ReadValue<Vector2>();
        List<RaycastResult> objDeteced = HitDetection(touchPosition, graphicRaycaster);
        foreach (RaycastResult result in objDeteced)
        {
            //Find Piece
            Piece targetPiece = result.gameObject.GetComponentInChildren<Piece>();

            if (targetPiece != null && !targetPiece.animationActice)
            {
                //Set piece to moving
                holdingPiece = targetPiece;
                holdingPiece.transform.SetParent(movingPiecesHolder.transform);
                holdingPiece.ChangeState(Piece.pieceStats.transparent);
                holdingPiece.onBoard = false;

                //Sound
                AudioManager.Instance.PlayWithEffects(pickupSound);

                //Misclick
                foundAction = true;

                break;
            }
        }

        //Raycast Board
        foreach (RaycastResult result in objDeteced)
        {
            if (result.gameObject.TryGetComponent(out Cell targetCell))
            {
                if (targetCell.occupying is Dot targetDot)
                {
                    if (targetDot.parentPiece != null)
                    {
                        holdingPiece = targetDot.parentPiece;
                        holdingPiece.transform.SetParent(movingPiecesHolder.transform);
                        board.PickupPiece(holdingPiece);
                        holdingPiece.GetComponent<Image>().raycastTarget = true;
                        CheckGoals();

                        targetDot.parentPiece.ChangeState(Piece.pieceStats.transparent);

                        holdingPiece.onBoard = false;

                        //Sound
                        AudioManager.Instance.PlayWithEffects(pickupSound);

                        //Misclick
                        foundAction = true;

                        break;
                    }
                }
            }
        }

        //Look if clicked button
        if (!foundAction && !holdingPiece)
        {
            foreach (RaycastResult result in objDeteced)
            {
                if(result.gameObject.GetComponent<Button>())
                {
                    foundAction = true; break;
                }
            }
        }

        //if(!foundAction && !holdingPiece)
        //{
        //    Debug.Log("No action found");
        //    currentMusclicks++;

        //    if(currentMusclicks == maxMisclicks)
        //    {
        //        currentMusclicks = 0;
        //        swipeAnimation.PlayAnimation();
        //    }
        //}
        //else
        //    currentMusclicks = 0;
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (holdingPiece == null)
            return;

        //Misclick reset
        currentMusclicks = 0;
        StartCoroutine(TempDisableControls(disableInputAfterReleasePieceTime));

        //Rotation
        hasRotated = false;

        //Tap or swipe
        bool canPlacePiece = false;

        //Raycast
        List<RaycastResult> deteced = HitDetection(holdingPiece.firstDot.GetComponent<RectTransform>().position, graphicRaycaster);
        foreach (RaycastResult result in deteced)
        {
            //See if we hit a cell
            if (result.gameObject.TryGetComponent(out Cell cell))
            {
                holdingPiece.transform.SetParent(board.transform);
                bool placeResult = board.PlacePiece(cell.gridPos, holdingPiece);

                if (placeResult)
                {
                    holdingPiece.onBoard = true; //To not rotate then swipe
                    //Place piece on board
                    holdingPiece.ChangeState(Piece.pieceStats.normal);
                    holdingPiece.GetComponent<Image>().raycastTarget = false;
                    RemoveHoldingPiece();
                    canPlacePiece = true;

                    CheckGoals();
                    break;
                }
            }
        }

        if (!canPlacePiece)
            ReturnPiece();
        else
            RemoveHoldingPiece();

        //Reset snap
        pieceSnapCalculation = 0;
    }

    private void ReturnPiece()
    {
        holdingPiece.ReturnToHolder();
        RemoveHoldingPiece();
    }

    private void RemoveHoldingPiece()
    {
        holdingPiece = null;
        holdingPieceRect = null;

        //Sound
        AudioManager.Instance.PlayWithEffects(dropSound);
    }

    private void Tap()
    {
        TapFunc();
    }
    private void Tap(InputAction.CallbackContext context)
    {
        TapFunc();
    }

    private void TapFunc()
    {
        calledSwipe = false;
        secendSwipeStartPos = secondSwipeAction.ReadValue<Vector2>();
    }

    private void LiftTap(InputAction.CallbackContext context)
    {
        if (!hasRotated)
        {
            Tap();
        }
        hasRotated = false;        
    }

    private void CheckGoals()
    {
        onDotChange?.Invoke();
    }

    public void TogglePlayerInput()
    {
        activeTouch = !activeTouch;
    }

    private IEnumerator TempDisableControls(float disableTime)
    {
        activeTouch = false;
        yield return new WaitForSeconds(disableTime);
        activeTouch = true;
    }
}
