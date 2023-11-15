using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    //Input
    private PlayerInput playerInput;

    private InputAction tapAction;
    private InputAction positionAction;
    private InputAction pressScreen;

    private Vector2 touchPosition;


    private Camera mainCam;

    [Header("Pieces")]
    [SerializeField] private float pieceSizeWhileHolding = .2f;
    private Piece holdingPiece;

    //UI Raycast
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    private void Awake()
    {
        //Raycast
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();

        //Input
        playerInput = GetComponent<PlayerInput>();
        positionAction = playerInput.actions["TouchPosition"];
        tapAction = playerInput.actions["Tap"];
        pressScreen = playerInput.actions["PrimaryContact"];

        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        tapAction.started += Click;
        pressScreen.canceled += Release;
    }

    private void OnDisable()
    {
        tapAction.started -= Click;
        pressScreen.canceled -= Release;
    }

    private void Update()
    {
        //Drag
        if (positionAction.WasPerformedThisFrame())
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Drag();
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        Debug.Log("Release");

        //Touch position
        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        // Create a pointer event data with the current input position
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touchPosition;

        // Create a list to store the raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the GraphicRaycaster
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);

            //See if we hit a cell
            if (result.gameObject.TryGetComponent(out Cell cell))
            {
                bool placeResult = Board.Instance.PlacePiece(cell.gridPos, holdingPiece);

                if (placeResult)
                    holdingPiece = null;
                else
                {
                    //Return to UI
                }
            }
        }    
    }

    private void Click(InputAction.CallbackContext context)
    {
        Debug.Log("Cliked");

    }

    private void Drag()
    {
        if (holdingPiece == null)
            return;

        //var result = mainCam.ScreenToWorldPoint(touchPosition);
        //result.z = 0;

        holdingPiece.transform.position = touchPosition;
    }
}
