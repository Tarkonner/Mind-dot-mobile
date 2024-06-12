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

    [Header("Raycasting")]
    [SerializeField] GraphicRaycaster boardRaycast;
    [SerializeField] GraphicRaycaster piecesRaycast;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float distanceBeforeSwipe = 50;
    [SerializeField] float touchOffsetY = 50;
    private bool calledSwipe = false;

    [Header("Animations")]
    [SerializeField] float pieceSnapPositionSpeed = 50;
    private float pieceSnapCalculation;

    //Event
    public delegate void OnDotsChange();
    public static event OnDotsChange onDotChange;
    public delegate void OnSwipe();
    public static event OnSwipe onSwipe;

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
        if (!activeTouch)
            return;

        //Drag
        //Get input
        Vector2 primeTouchPosition = positionAction.ReadValue<Vector2>();
        if (primeTouchPosition != Vector2.zero)
        {
            touchPosition = primeTouchPosition;
        }

        if (holdingPiece != null)
        {
            holdingPiece.OnDrag(pointerEventData); //Not doing anything right now

            //Move
            if (holdingPieceRect == null)
                holdingPieceRect = holdingPiece.gameObject.GetComponent<RectTransform>();

            //Second touch
            if (secoundTap.WasPressedThisFrame())
                Tap();

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
            Vector2 swipeLenght = swipeAction.ReadValue<Vector2>();
            if (!calledSwipe && swipeLenght.magnitude >= distanceBeforeSwipe)
            {
                onSwipe?.Invoke();
                calledSwipe = true;
            }
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
        calledSwipe = false;

        if (holdingPiece != null || !activeTouch)
            return;

        //Rotation
        hasRotated = true;

        touchPosition = positionAction.ReadValue<Vector2>();

        //Raycast Pieceholder
        List<RaycastResult> piecesDeteced = HitDetection(touchPosition, piecesRaycast);
        foreach (RaycastResult result in piecesDeteced)
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
                break;
            }
        }

        //Raycast Board
        List<RaycastResult> boardDection = HitDetection(touchPosition, boardRaycast);
        foreach (RaycastResult result in boardDection)
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
                    }
                }
            }
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (holdingPiece == null)
            return;

        //Rotation
        hasRotated = false;

        //Tap or swipe
        bool canPlacePiece = false;

        //Raycast
        List<RaycastResult> deteced = HitDetection(holdingPiece.firstDot.GetComponent<RectTransform>().position, boardRaycast);
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
    }

    private void Tap()
    {
        if (holdingPiece != null)
        {
            holdingPiece.RotateWithAnimation();
            hasRotated = true;
        }
    }
    private void Tap(InputAction.CallbackContext context)
    {
        if (!hasRotated)
        {
            hasRotated = true;
            Tap();
        }
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
}
