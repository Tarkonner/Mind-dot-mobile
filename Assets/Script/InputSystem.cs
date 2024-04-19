using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public static InputSystem instance;

    //Input
    private PlayerInput playerInput;

    private InputAction tapAction;
    private InputAction positionAction;
    private InputAction secoundTap;

    public Vector2 touchPosition { get; private set; }
    private bool hasRotated = false;

    [Header("Refences")]
    [SerializeField] private Board board;

    [Header("Pieces")]
    [SerializeField] private GameObject movingPiecesHolder;
    [SerializeField] Transform piecesHolder;
    private Piece holdingPiece;
    private RectTransform holdingPieceRect;

    [Header("Raycasting")]
    [SerializeField] GraphicRaycaster boardRaycast;
    [SerializeField] GraphicRaycaster piecesRaycast;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float distanceBeforeSwipe = 50;
    [SerializeField] float secoundTouchTimeBetweenTouch = .2f;
    [SerializeField] float touchOffsetY = 50;
    [SerializeField] RectTransform rotateLine;
    private Vector2 contactPosition;
    private bool fromBoard = false;

    [Header("Animations")]
    [SerializeField] float pieceSnapPositionSpeed = 50;
    private float pieceSnapCalculation;

    //Event
    public delegate void OnDotsChange();
    public static event OnDotsChange onDotChange;

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
    }

    private void OnEnable()
    {
        tapAction.started += BeginDrag;
        tapAction.canceled += Release;

        secoundTap.started += Tap;
        secoundTap.canceled += LiftTap;
    }
    private void OnDisable()
    {
        tapAction.started -= BeginDrag;
        tapAction.canceled -= Release;

        secoundTap.started -= Tap;
        secoundTap.canceled -= LiftTap;
    }

    private void Update()
    {
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
            if(holdingPieceRect == null)
                holdingPieceRect = holdingPiece.gameObject.GetComponent<RectTransform>();

            if (secoundTap.WasPressedThisFrame())
                Tap();

            //Calculate position
            if(pieceSnapCalculation < 1)
            {
                pieceSnapCalculation += Time.deltaTime * pieceSnapPositionSpeed;
                if(pieceSnapCalculation > 1)
                    pieceSnapCalculation = 1;
            }
            Vector2 targetPosition = touchPosition;
            if(!fromBoard)
                targetPosition += new Vector2(0, touchOffsetY + Mathf.RoundToInt(Mathf.Abs(holdingPiece.pieceCenter.y) / 2) * holdingPiece.DotSpacing);
            Vector2 calPosition = Vector2.Lerp(touchPosition, targetPosition, pieceSnapCalculation);

            //Set posotion
            holdingPieceRect.position = calPosition;
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
        if (holdingPiece != null)
            return;

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        //Save where first touching
        contactPosition = touchPosition;

        //Raycast Pieceholder
        List<RaycastResult> piecesDeteced = HitDetection(touchPosition, piecesRaycast);
        foreach (RaycastResult result in piecesDeteced)
        {
            if(result.gameObject.TryGetComponent(out Piece targetPiece))
            {
                //Set piece to moving
                holdingPiece = targetPiece;
                holdingPiece.transform.SetParent(movingPiecesHolder.transform);
                targetPiece.ChangeState(Piece.pieceStats.transparent);
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
                        fromBoard = true;
                    }
                }
            }
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (holdingPiece == null)
            return;

        //Tap or swipe
        if (Vector2.Distance(contactPosition, touchPosition) < distanceBeforeSwipe)
        {
            //Tap();
            ReturnPiece();
        }
        else
        {
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
        }

        //Reset snap
        pieceSnapCalculation = 0;

        //Offset turn on and off
        fromBoard = false;

        //Rotation
        hasRotated = false;
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
        if(holdingPiece != null)
        {
            holdingPiece.RotateWithAnimation();
            hasRotated = true;
        }
    }
    private void Tap(InputAction.CallbackContext context)
    {
        if(!hasRotated) 
        {
            hasRotated = true;
            Tap();
        }
    }
    private void LiftTap(InputAction.CallbackContext context)
    {
        if(!hasRotated)
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
