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

    private Vector2 touchPosition;

    [SerializeField] private GameObject TestCirkle;

    private void Awake()
    {
        TestCirkle = Instantiate(TestCirkle);
    }

    private void OnEnable()
    {
        tapAction.started += Click;
    }

    private void OnDisable()
    {
        tapAction.started -= Click;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (positionAction.WasPerformedThisFrame())
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); ;
            Drag();
        }
    }

    private void Click(InputAction.CallbackContext context)
    {
        Debug.Log("Cliked");

    }

    private void Drag()
    {
        var result = Camera.main.ScreenToWorldPoint(touchPosition);
        result.z = 0;

        TestCirkle.transform.position = result;
    }
}
