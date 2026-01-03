using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
public class InputHandlerSetting : MonoBehaviour
{
    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }
}
