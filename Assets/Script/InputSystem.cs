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
    private bool holding = false;


    private Camera mainCam;
    [SerializeField] private GameObject TestCirkle;

    private void Awake()
    {
        //Input
        playerInput = GetComponent<PlayerInput>();
        positionAction = playerInput.actions["TouchPosition"];
        tapAction = playerInput.actions["Tap"];
        pressScreen = playerInput.actions["PrimaryContract"];

        mainCam = Camera.main;

        //Testing
        TestCirkle = Instantiate(TestCirkle);
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
            holding = true;

            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Drag();
        }
    }

    private void Release(InputAction.CallbackContext context)
    {

        Debug.Log("Release");

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        RaycastHit2D hit = Physics2D.Raycast(touchPosition, transform.forward);
        if (hit.collider != null)
        {
            GameObject target = hit.collider.gameObject;
            Debug.Log(target);
            if (TryGetComponent(out Cell cell))
            {
                Debug.Log(cell.gridPos);
            }
        }
    }

    private void Click(InputAction.CallbackContext context)
    {
        Debug.Log("Cliked");

    }

    private void Drag()
    {
        var result = mainCam.ScreenToWorldPoint(touchPosition);
        result.z = 0;

        TestCirkle.transform.position = result;
    }
}
