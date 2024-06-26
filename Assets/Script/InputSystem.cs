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
    private InputAction pressScreen;

    public Vector2 touchPosition { get; private set; }

    [Header("Refences")]
    [SerializeField] private Board board;

    [Header("Pieces")]
    [SerializeField] private GameObject movingPiecesHolder;
    [SerializeField] Transform piecesHolder;
    private Piece holdingPiece;
    private RectTransform holdingPieceRect;
    private Transform takenFrom;

    [Header("Raycasting")]
    [SerializeField] GraphicRaycaster boardRaycast;
    [SerializeField] GraphicRaycaster piecesRaycast;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float distanceBeforeSwipe = 50;
    [SerializeField] RectTransform rotateLine;
    private Vector2 contactPosition;
    [SerializeField] float touchOffsetY = 50;

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
        pressScreen = playerInput.actions["PrimaryContact"];
    }

    private void OnEnable()
    {
        pressScreen.started += BeginDrag;
        pressScreen.canceled += Release;
    }

    private void OnDisable()
    {
        pressScreen.started -= BeginDrag;
        pressScreen.canceled -= Release;
    }

    private void Update()
    {
        //Drag
        if (positionAction.WasPerformedThisFrame())
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        if (holdingPiece != null)
        {
            holdingPiece.OnDrag(pointerEventData); //Not doing anything right now
            
            //Move
            if(holdingPieceRect == null)
                holdingPieceRect = holdingPiece.gameObject.GetComponent<RectTransform>();
            Vector2 targetPosition = touchPosition + new Vector2(0, 
                touchOffsetY + Mathf.RoundToInt(Mathf.Abs(holdingPiece.pieceCenter.y) / 2) * holdingPiece.DotSpacing);
            holdingPieceRect.position = targetPosition;
        }
        else
            holdingPieceRect = null;
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
                takenFrom = targetPiece.transform;
                holdingPiece = targetPiece;
                holdingPiece.transform.SetParent(movingPiecesHolder.transform);
                targetPiece.ChangeState(Piece.pieceStats.transparent);
                break;
            }
        }

        
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
                    }
                }
            }
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (holdingPiece == null)
            return;

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        //Tap or swipe
        if (Vector2.Distance(contactPosition, touchPosition) < distanceBeforeSwipe)
        {
            Tap();
            holdingPiece.ReturnToHolder();
            holdingPiece = null;
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
                    Debug.Log(cell.gridPos);

                    holdingPiece.transform.SetParent(board.transform);
                    bool placeResult = board.PlacePiece(cell.gridPos, holdingPiece);

                    if (placeResult)
                    {
                        //Place piece on board
                        holdingPiece.ChangeState(Piece.pieceStats.normal);
                        holdingPiece.GetComponent<Image>().raycastTarget = false;
                        holdingPiece = null;
                        canPlacePiece = true;

                        CheckGoals();
                        break;
                    }
                }
            }            

            if (!canPlacePiece)
            {
                holdingPiece.ReturnToHolder();
                holdingPiece = null;
            }
        }
    }

    private void Tap()
    {
        if(holdingPiece != null && touchPosition.y < rotateLine.position.y)
        {
            holdingPiece.Rotate();
        }
    }
    private void CheckGoals()
    {
        onDotChange?.Invoke();
    }
}
