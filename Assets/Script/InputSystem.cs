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

    [Header("Pieces")]
    [SerializeField] private GameObject moveingPiecesHolder;
    [SerializeField] Transform piecesHolder;
    [SerializeField] private float pieceSizeWhileHolding = .2f;
    private Piece holdingPiece;
    private Piece pieceWhereWasPointetAd;

    [Header("Raycating")]
    [SerializeField] GraphicRaycaster boardRaycast;
    [SerializeField] GraphicRaycaster piecesRaycast;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Header("Input")]
    [SerializeField] float timeBeforeDragging = .15f;
    private float beforeDragTimer;
    private bool activeDraggingTimer;

    private void Awake()
    {
        instance = this;

        //Raycast
        eventSystem = GetComponent<EventSystem>();

        //Input
        playerInput = GetComponent<PlayerInput>();
        positionAction = playerInput.actions["TouchPosition"];
        tapAction = playerInput.actions["Tap"];
        pressScreen = playerInput.actions["PrimaryContact"];
    }

    private void OnEnable()
    {
        //Input
        tapAction.started += Click;
        
        pressScreen.started += BeginDrag;
        pressScreen.canceled += Release;

        //Timer
        pressScreen.started += TurnOnTimer;
        pressScreen.canceled += TurnOffTimer;
    }

    private void OnDisable()
    {
        //Input
        tapAction.started -= Click;

        pressScreen.started -= BeginDrag;
        pressScreen.canceled -= Release;

        //Timer
        pressScreen.started -= TurnOnTimer;
        pressScreen.canceled -= TurnOffTimer;
    }

    private void Update()
    {
        if(activeDraggingTimer && pieceWhereWasPointetAd != null)
        {
            timeBeforeDragging += Time.deltaTime;

            if(timeBeforeDragging > beforeDragTimer)
            {
                timeBeforeDragging = 0;

                holdingPiece = pieceWhereWasPointetAd;
                
                holdingPiece.transform.SetParent(moveingPiecesHolder.transform);
            }
        }

        //Drag
        if(positionAction.WasPerformedThisFrame())
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        if (holdingPiece != null)
            holdingPiece.OnDrag(pointerEventData);
    }

    List<RaycastResult> HitDetection(Vector2 inputPosition, GraphicRaycaster raycaster)
    {
        // Create a pointer event data with the current input position
        pointerEventData = new PointerEventData(eventSystem);
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
        List<RaycastResult> deteced = HitDetection(touchPosition, piecesRaycast);

        foreach (RaycastResult result in deteced)
        {
            if(result.gameObject.TryGetComponent(out Piece targetPiece))
            {
                pieceWhereWasPointetAd = targetPiece;
            }
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        if (holdingPiece == null)
            return;

        Debug.Log("Release");

        bool canPlacePiece = false;

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        List<RaycastResult> deteced = HitDetection(touchPosition, boardRaycast);

        foreach (RaycastResult result in deteced)
        {
            Debug.Log(result.gameObject.name);

            //See if we hit a cell
            if (result.gameObject.TryGetComponent(out Cell cell))
            {
                bool placeResult = Board.Instance.PlacePiece(cell.gridPos, holdingPiece);

                if (placeResult)
                {
                    //Place piece on board
                    holdingPiece = null;
                    canPlacePiece = true;
                }
            }
        }    

        if(!canPlacePiece)
        {
            //Return to Holder
            holdingPiece.transform.SetParent(piecesHolder);
            holdingPiece.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            holdingPiece = null;
        }
    }

    private void TurnOnTimer(InputAction.CallbackContext context) => activeDraggingTimer = true;
    private void TurnOffTimer(InputAction.CallbackContext context) => activeDraggingTimer = false;

    private void Click(InputAction.CallbackContext context)
    {
        if(pieceWhereWasPointetAd != null)
        {
            Debug.Log("C");
            pieceWhereWasPointetAd.Rotate();
        }
    }
}
