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
    [SerializeField] private GameObject moveingPiecesHolder;
    [SerializeField] Transform piecesHolder;
    [SerializeField] private float pieceSizeWhileHolding = .2f;
    private Piece holdingPiece;
    private Transform takenFrom;

    [Header("Raycating")]
    [SerializeField] GraphicRaycaster boardRaycast;
    [SerializeField] GraphicRaycaster piecesRaycast;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float distanceBeforeSwipe = 50;
    [SerializeField] RectTransform rotateLine;
    private Vector2 contactPosition;

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
            holdingPiece.OnDrag(pointerEventData);
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

        //Raycast Piecehodler
        List<RaycastResult> piecesDeteced = HitDetection(touchPosition, piecesRaycast);
        foreach (RaycastResult result in piecesDeteced)
        {
            if(result.gameObject.TryGetComponent(out Piece targetPiece))
            {
                //Set piece to moveing
                takenFrom = targetPiece.transform;
                holdingPiece = targetPiece;
                holdingPiece.transform.SetParent(moveingPiecesHolder.transform);
                break;
            }
        }

        //Board detection
        List<RaycastResult> boardDection = HitDetection(touchPosition, boardRaycast);
        foreach (RaycastResult result in boardDection)
        {
            if (result.gameObject.TryGetComponent(out Cell targetCell))
            {
                Dot targetDot = null;
                if (targetCell.occupying is Dot)
                {
                    targetDot = (Dot)targetCell.occupying;
                    if (targetDot.parentPiece != null)
                    {
                        holdingPiece = targetDot.parentPiece;
                        holdingPiece.transform.SetParent(moveingPiecesHolder.transform);
                        board.PickupPiece(holdingPiece);
                        holdingPiece.GetComponent<Image>().raycastTarget = true;
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
            Debug.Log("Tap");
            Tap();
            holdingPiece.ReturnToHolder();
            holdingPiece = null;
        }
        else
        {
            Debug.Log("Place");
            bool canPlacePiece = false;

            //Raycast
            List<RaycastResult> deteced = HitDetection(touchPosition, boardRaycast);
            foreach (RaycastResult result in deteced)
            {
                Debug.Log(result.gameObject.name);

                //See if we hit a cell
                if (result.gameObject.TryGetComponent(out Cell cell))
                {
                    bool placeResult = board.PlacePiece(cell.gridPos, holdingPiece);

                    if (placeResult)
                    {
                        //Place piece on board
                        holdingPiece.transform.SetParent(board.transform);
                        holdingPiece.GetComponent<Image>().raycastTarget = false;
                        holdingPiece = null;
                        canPlacePiece = true;
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
            Debug.Log($"Tocuh positon y: {touchPosition.y}");
            holdingPiece.Rotate();
        }
    }
}
