using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputSystem : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction tapAction;
    private InputAction positionAction;
    private InputAction pressScreen;

    private Vector2 touchPosition;


    private Camera mainCam;
    private Piece holdingPiece;

    [Header("Pieces")]
    [SerializeField] private float pieceSizeWhileHolding = .2f;

    private void Awake()
    {
        //Input
        playerInput = GetComponent<PlayerInput>();
        positionAction = playerInput.actions["TouchPosition"];
        tapAction = playerInput.actions["Tap"];
        pressScreen = playerInput.actions["PrimaryContract"];

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

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector2 pos = mainCam.ScreenToWorldPoint(touchPosition);


        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null)
        {
            //See if we hit a cell
            if (hit.transform.TryGetComponent(out Cell cell))
            {
                bool result = Board.Instance.PlacePiece(cell.gridPos, holdingPiece);

                if (result)
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

        var result = mainCam.ScreenToWorldPoint(touchPosition);
        result.z = 0;

        holdingPiece.transform.position = result;
    }
}
